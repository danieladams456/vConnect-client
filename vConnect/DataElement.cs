﻿/* DataElement.cs - vConnect (Liberty University CSCI Capstone Project)
 * Written by Troy Cosner and Charlie Snyder in February-March, 2015. 
 * 
 * This class embodies an element of data to be read from a car using OBDII codes
 *  sent over a Bluetooth (BT) connection. At instantiation, an object will hold
 *  the information necessary for retrieving its associated data point from the car.
 *  After reading the data from the car, the associated element's data will be ready
 *  to be added to a cache (refer to the DataCache class), then sent to the server. * 
 */
using System;
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
        private string obdPID = "";       // PID to be sent to the car to retrive data
        private string obdMode = "";       // Mode on which to send the PID.
        private string name = "";       // Name of the data element
        private string dataType = "";       // Data type of the element (string, number, etc.)
        private int returnDataSize = 0;     // Number of bytes that the element will receive from car
        private byte[] returnData;             // Holds the data returned from the car
        private string valueToSend = "";       // Holds the value to send to the server for the element
        private string equation = "";       // The equation to calculate a human-readable value from 
                                            //  the bytes returned by the car.
       
        
        private int[] equVals = new int[2]; // Array that will contain raw values from the OBDII module 
                                             // to be used in calculating formatted data values. 

        private bool noDataCheck = false; // Bool that will be switched to true if a data element
                                          // doesn't need to be formatted. 


        // This connection gets passed from the caller. It is the current connection.
        public BluetoothConnectionHandler BTConnection;

        // This defines the largest potential size of return data from the car. 
        private const int MAX_DATA_SIZE = 50;

        /// <summary>
        /// Constructor that defines name and PID and number of bytes explicitly 
        /// </summary>
        /// <param name="elementName">name of the element to be collected</param>
        /// <param name="PID">OBD PID to send to the car to get this value</param>
        /// <param name="numberBytesReturned">Number of bytes the car will return</param>
        /// <param name="btconnection">Current BT ConnectionHandler object.</param>
        public DataElement(string elementName, string mode, string PID, string type, int numberBytesReturned, string eqn, BluetoothConnectionHandler btconnection)
        {
            name = elementName;
            obdMode = mode;
            obdPID = PID;
            dataType = type;
            returnData = new byte[MAX_DATA_SIZE];
            returnDataSize = numberBytesReturned;
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
        /// False - OBDII codes were not successfully read and converted into values to be used in 
        /// computing the requested vehicle information.
        /// </returns>
        public bool RequestDataFromCar()
        {
            string writeString; // String that will contain the request to be send to the OBDII module.
            string hexLiteral;  // String that will contain a raw hex value returned from the OBDII module.
            string hexLiteral2; // String that will contain a raw hex value returned from the OBDII module. 
            
            // If BT device is connected. 
            if (BTConnection.Client.Connected)
            {
                
                // If this is the vin data element, then do the following reads in order to get all 
                // of the bytes relating to the VIN.
              
                if (name == "VIN")
                {
                    writeString = "09" + obdPID + "\r";// Write string for VIN.
                   // writeString = obdMode + obdPID + "\r";// Write string for VIN.
                    
                    // Byte arrays to read in VIN.
                    byte[] vin1 = new byte[50];           
                    byte[] vin2 = new byte[50];


                    // Create the code to request VIN.
                    byte[] writeCode = System.Text.Encoding.ASCII.GetBytes(writeString);
                    try
                    {
                        // Write the code to the OBDII module.
                        Form1.peerStream.Write(writeCode, 0, writeCode.Length);
                        
                        // Must retrieve the VIN in two different reads. 
                        // Thread pauses are to give the OBDII module time to process
                        // requests and write to the buffer. 
                        System.Threading.Thread.Sleep(150);

                        // Read the first part of the VIN
                        Form1.peerStream.Read(vin1, 0, vin1.Length);
                        
                        // Pause to let the output buffer fill up. 
                        System.Threading.Thread.Sleep(100);

                        // Error if VIN returns NO DATA.
                        if (System.Text.Encoding.ASCII.GetString(vin1).Contains("NO DATA"))
                        {
                            Form1.LogMessageToFile("error", "RequestDataFromCar()", "VIN returned " + System.Text.Encoding.ASCII.GetString(vin1));
                            return false;
                        }

                        // Read second part of VIN from OBDII module.
                        Form1.peerStream.Read(vin2, 0, vin2.Length);

                        // If polling has stopped, exit.
                        if (!Form1.pollingData)
                            return false;
                    }

                    // Exception will handle loss of BT connection with OBDII module
                    // during read/write.
                    catch (Exception ex)
                    {
                        var msg = "Lost BT connection to Device. ERROR: " + ex;
                        Form1.LogMessageToFile("error","Bt Connection Error", msg);
                        return false;
                    }

                    // Format the raw data read from the OBDII module.
                    valueToSend = System.Text.Encoding.ASCII.GetString(vin1)
                        + System.Text.Encoding.ASCII.GetString(vin2);
                    Form1.LogMessageToFile("event", "RequestDataFromCar()", "Raw data to send.");

                    // REGEX Parses to clear extra characters from VIN string.
                    valueToSend = Regex.Replace(valueToSend, @".:", "");
                    valueToSend = Regex.Replace(valueToSend, @"^...", "");
                    valueToSend = Regex.Replace(valueToSend, @"490201", "");
                 

                    valueToSend = Regex.Replace(valueToSend, @"ELM327v1.4", "");
                    valueToSend = Regex.Replace(valueToSend, @" ", "");
                    valueToSend = Regex.Replace(valueToSend, @"\r", "");
                    valueToSend = Regex.Replace(valueToSend, @">", "");
                    valueToSend = Regex.Replace(valueToSend, @"\?", "");

                    valueToSend = Regex.Replace(valueToSend, @"\0", "");
                    valueToSend = Regex.Replace(valueToSend, @"\.", "");

                    string res = String.Empty;

                    // Convert Hex encoded string to ASCII. 
                    try
                    {
                        for (int a = 0; a < valueToSend.Length; a = a + 2)
                        {
                            string Char2Convert = valueToSend.Substring(a, 2);
                            int n = Convert.ToInt32(Char2Convert, 16);
                            char c = (char)n;

                            res += c.ToString();

                        }
                    }
                    // Catch  error caused by corrupted data sent by OBDII module that causes
                    // Parser above to fail.
                    catch (Exception e)
                    {
                        Form1.LogMessageToFile("error","VIN Parser Error", e.Message);
                        return false;
                    }

                    // Clear out filler characters from VIN (I and 0 never occur in VINs, so they are
                    // used as new line delimiters and filler)
                    res = Regex.Replace(res, @"I", "");
                    valueToSend = res;
                    valueToSend = Regex.Replace(valueToSend, @"\0", "");
                    valueToSend = Regex.Replace(valueToSend, @"[^a-zA-Z0-9]", "");
                    
                    // Log the message to send.
                    Form1.LogMessageToFile("event", "Log thing", "Log to send: " + valueToSend);
                    return true;
                }

                // If the data element is for something other than the VIN, then use the following code
                // to poll for data.
                else
                {
                    // OBDII request string. 
                    writeString = "01" + obdPID + "\r";
                    // OBDII request string. 
                  //  writeString = obdMode + obdPID + "\r";
                    try
                    {
                        // Encode the writeString, then write it to the OBDII module. 
                        byte[] writeCode = System.Text.Encoding.ASCII.GetBytes(writeString);
                        Form1.peerStream.Write(writeCode, 0, writeCode.Length);
                        
                        // Wait 0.15 seconds for the OBDII module to process the code request
                        System.Threading.Thread.Sleep(150);

                        // Read the OBDII code data from the OBDII module.
                        Form1.peerStream.Read(returnData, 0, returnData.Length);
                        // If polling has stopped, return. 
                        
                        if (!Form1.pollingData)
                            return false;
                    }

                    // Catch exception caused my loss of BT connection during read/write. 
                    catch (Exception ex)
                    {
                        var msg = "Lost BT connection with Device.  ";
                        Form1.LogMessageToFile("error","BT Connection Error", msg + ex);
                        return false;
                    }
                   
                    // If the vehicle doesn't support this particular vehicle code, it will return "NO DATA",
                    // which we are handling by sending an empty string to the server.
                    if (System.Text.Encoding.ASCII.GetString(returnData).Contains("NO DATA"))
                    {
                        valueToSend = "Not supported";
                        noDataCheck = true;
                    }
                    else
                    {
                        try
                        {
                            
                            // Get the Hex Literals representing the values to be used in the equations that 
                            // calculate the vehicle info values. Some OBDII codes require one hex literal, others two. 
                            // Converst those literals into integers.
                            if (returnDataSize == 1)
                            {
                               
                                hexLiteral = System.Text.Encoding.ASCII.GetString(returnData, 4, 1) + System.Text.Encoding.ASCII.GetString(returnData, 5, 1);
                                equVals[0] = Convert.ToInt32(hexLiteral, 16);

                            }
                            else if (returnDataSize == 2)
                            {
                                hexLiteral = System.Text.Encoding.ASCII.GetString(returnData, 4, 1) + System.Text.Encoding.ASCII.GetString(returnData, 5, 1);
                                hexLiteral2 = System.Text.Encoding.ASCII.GetString(returnData, 6, 1) + System.Text.Encoding.ASCII.GetString(returnData, 7, 1);
                                equVals[0] = Convert.ToInt32(hexLiteral, 16);
                                equVals[1] = Convert.ToInt32(hexLiteral2, 16);
                            }
                        }
                         
                        // Exception that catches an errors that are the result of corrupted data
                        // sent from the OBDII module.d 
                        catch(Exception e)
                        {
                            var msg = "Polled data was corrupted.";
                            Form1.LogMessageToFile("error","BT Connection Error", msg + e);
                            return false;
                        }
                    }

                }
                return true;
            }

            // If no connection, record in error log.
            else
            {
                Form1.LogMessageToFile("error","DataElement", "Lost BT Connection");                 
                return false;                
            }
        }


        /// <summary>
        /// Calculates the requested vehicle information using the integer values retrieved via OBDII module.
        /// </summary>
        public void FormatData()
        {
            try
            {
                // If the data element is for the VIN, then its already formatted, proceed if otherwise.
                if (name != "VIN")
                {
                    // Create an expression with the equation specified.
                    Expression expr = new Expression(equation);

                    // Send empty string if no data was read from OBDII device.
                    if (noDataCheck == true)
                        valueToSend = "Not supported";

                    // Compute values to store in the data cache.
                    else
                    {
                        // If the equation only has an "A"
                        if (returnDataSize == 1)
                        {

                            expr.Parameters["A"] = equVals[0];
                        }


                        // If the equation has A and B
                        else if (ReturnDataSize == 2)
                        {
                            expr.Parameters["A"] = equVals[0];
                            expr.Parameters["B"] = equVals[1];
                        }

                        // evaluate the expression with the variables
                        object answerToExpression = expr.Evaluate();

                        // Store the formatted answer in the valueToSend variable,
                        // rounding any decimal value down to the nearest integer. 
                        ValueToSend = answerToExpression.ToString();
                        float temp = float.Parse(valueToSend);
                        ValueToSend = ((int)temp).ToString();
                    }
                }
            }
            // Catch any unexpected error. 
            catch (Exception e)
            {
                Form1.LogMessageToFile("error","Format Data", e.ToString());
            }
        }



        // The following are C#'s way of using accessors (get and set). Call them with
        // Set:
        //      object.ObdPID = "02"
        // Get:
        //      object.ObdPID

        public string ObdPID { get { return obdPID; } set { obdPID = value; } }

        public string ObdMode { get { return obdMode; } set { obdMode = value; } }

        public string Name { get { return name; } set { name = value; } }

        public string DataType { get { return dataType; } set { dataType = value; } }

        public int ReturnDataSize { get { return returnDataSize; } set { returnDataSize = value; } }

        public byte[] ReturnData { get { return returnData; } set { returnData = value; } }

        public string ValueToSend { get { return valueToSend; } set { valueToSend = value; } }

        public string Equation { get { return equation; } set { equation = value; } }
    }
}
