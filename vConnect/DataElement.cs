﻿using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using NCalc;

namespace vConnect
{
    /// <summary>
    /// This class describes a data element received from the car via BT.
    /// </summary>
    public class DataElement
    {
        private string obdPID = "";
        private string name = "";
        private int returnDataSize = 0;
        private byte[] returnData;
        private string valueToSend = "";
        private string equation = "";
        private int[] equVals = new int[10];
        private bool noDataCheck = false;

        // NOTE: These are only used for testing purposes.
        private bool testOBDII = false;
        private bool testVehicleData = false;

        // This connection gets passed from the caller. It is the current connection.
        public BluetoothConnectionHandler BTConnection;

        // This defines the largest potential size of return data from the car. It should never
        //  really even get close to this value
        private int MAX_DATA_SIZE= 30;

        /// <summary>
        /// Constructor that defines name and PID and number of bytes explicitly 
        /// </summary>
        /// <param name="elementName">name of the element to be collected</param>
        /// <param name="PID">OBD PID to send to the car to get this value</param>
        /// <param name="numberBytesReturned">Number of bytes the car will return</param>
        /// <param name="btconnection">Current BT ConnectionHandler object.</param>
        public DataElement(string elementName, string PID, int numberBytesReturned, string eqn, BluetoothConnectionHandler btconnection)
        {
            name = elementName;
            obdPID = PID;
            returnData = new byte[MAX_DATA_SIZE];
            ReturnDataSize = numberBytesReturned;
            equation = eqn;
            BTConnection = btconnection;
        }

        /// <summary>
        /// Default constructor if no parameters passed
        /// </summary>
        public DataElement()
        {
            returnData = new byte[MAX_DATA_SIZE];
        }

        /// <summary>
        /// Writes OBDII code to OBDII module, and parses the return bytes according the the code.
        /// </summary>
        /// <returns>
        /// true - ODBII codes was successfully requested and corresponding data was read successfully.  
        /// False - Connection to the BT Device was lost. 
        /// </returns>
        public bool RequestDataFromCar()
        {
            
            string writeString;
            string hexLiteral;
            string hexLiteral2;
            
            if (BTConnection.Client.Connected)
            {
                Stream peerStream = BTConnection.Client.GetStream();
                
                // Creates proper string to write to the OBDII module to request data. 
                if (name == "vin")
                {
                    writeString = "09" + ObdPID + "\r"; 
                    returnData = new byte[50];
                }

                else { writeString = "01" + ObdPID + "\r"; }

                try
                {
                    // Encode the writeString, then write it to the OBDII module. 
                    byte[] writeCode = System.Text.Encoding.ASCII.GetBytes(writeString);
                    peerStream.Write(writeCode, 0, writeCode.Length);

                    // Wait 10 seconds for the OBDII module to process the code request
                    System.Threading.Thread.Sleep(10000);

                    // Read the OBDII code data from the OBDII module.
                    peerStream.Read(returnData, 0, returnData.Length);
                }

                // Attempt to reconnect to OBDII device.
                // If connection is true, recall RequestDataFromCar()
                catch (Exception ex)
                {
                    if (BTConnection.EstablishBTConnection())
                        RequestDataFromCar();
                    else
                    {
                        var msg = "failed to connect to BT Device. ERROR: " + ex;
                        MessageBox.Show(msg);
                        BTConnection.SendWindowsErrorMessage();
                        return false;
                    }

                }

                if (System.Text.Encoding.ASCII.GetString(returnData).Contains("NO DATA"))
                    noDataCheck = true;    

                // Will insert the vin into valueToSend, therefore we will not call formatData for
                // VIN data elements.
                // have to format correctly. 
                else if (name == "vin")
                    valueToSend = System.Text.Encoding.ASCII.GetString(returnData, 5, 40);
                
                else
                {
                    //Skips repeated bytes.
                    if (ReturnDataSize == 1)
                    {
                      hexLiteral = System.Text.Encoding.ASCII.GetString(returnData, 11, 1) + System.Text.Encoding.ASCII.GetString(returnData, 12, 1);
                      equVals[0] = Convert.ToInt32(hexLiteral, 16);
                    }
                    else if (ReturnDataSize == 2)
                   {
                          hexLiteral = System.Text.Encoding.ASCII.GetString(returnData, 11, 1) + System.Text.Encoding.ASCII.GetString(returnData, 12, 1);
                          hexLiteral2 =  System.Text.Encoding.ASCII.GetString(returnData, 14, 1) + System.Text.Encoding.ASCII.GetString(returnData, 15, 1);
                          equVals[0] = Convert.ToInt32(hexLiteral, 16);
                          equVals[1] = Convert.ToInt32(hexLiteral2, 16);
                   }
                }
            }
            else 
            {
                MessageBox.Show("Lost BT Connection", "My Application",
                 MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                return false;
            }
                     
            return true;
        }


        public void FormatData()
        {
            // Create an expression with the equation specified.
            Expression expr = new Expression(Equation);

            // Send empty string if no data was read from OBDII device.
            if (noDataCheck == true)
                valueToSend = "";
            
            // Compute values to store in the data cache.
            else
            {
                // If the equation only has an "A"
                if (ReturnDataSize == 1)
                {
                    expr.Parameters["A"] = equVals[0];
                }

                // If the equation has A and B...
                else if (ReturnDataSize == 2)
                {
                    expr.Parameters["A"] = equVals[0];
                    expr.Parameters["B"] = equVals[1];
                }

                // Should we go further?
                // else if...

                // evaluate the expression with the variables
                object answerToExpression = expr.Evaluate();

                // Store the formatted answer in the valueToSend variable.
                ValueToSend = answerToExpression.ToString();

            }
            return;
        }

        public bool SendEmergencyCode()
        {
            return true;
        }

        // The following are C#'s way of using accessors (get and set). Call them with
        // Set:
        //      object.ObdPID = "02"
        // Get:
        //      object.ObdPID

        public string ObdPID { get { return obdPID; } set { obdPID = value; } }

        public string Name { get { return name; } set { name = value; } }

        public int ReturnDataSize { get { return returnDataSize; } set { returnDataSize = value; } }

        public byte[] ReturnData { get { return returnData; } set { returnData = value; } }

        public string ValueToSend { get { return valueToSend; } set { valueToSend = value; } }

        public string Equation { get { return equation; } set { equation = value; } }

        public bool TestOBDII { get { return testOBDII; } set { testOBDII = value; } }

        public bool TestVehicleData { get { return testVehicleData; } set { testVehicleData = value; } }

    }
}