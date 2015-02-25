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
        private string obdPID = "";
        private string obdMode = "";
        private string name = "";
        private string dataType = "";
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
        //  really even get close to this value.
        private const int MAX_DATA_SIZE= 30;

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
                
                // If this is the vin data element, then do the following reads in order to get all 
                // of the bytes relating to the VIN.
                if (name == "VIN")
                {
                    writeString = "09" + ObdPID + "\r";
                    returnData = new byte[200];
                    byte[] vin1 = new byte[50];
                    byte[] vin2 = new byte[50];
                    byte[] vin3 = new byte[50];
                    byte[] vin4 = new byte[50]; 

                    // Create the code to request VIN.
                    byte[] writeCode = System.Text.Encoding.ASCII.GetBytes(writeString);

                    // Write the code to the OBDII module.
                    peerStream.Write(writeCode, 0, writeCode.Length);

                    // Must retrieve the VIN in three different reads. 
                    System.Threading.Thread.Sleep(7000);
                    peerStream.Read(vin1, 0, vin1.Length);
                    System.Threading.Thread.Sleep(7000);
                    peerStream.Read(vin2, 0, vin2.Length);
                    System.Threading.Thread.Sleep(7000);
                    peerStream.Read(vin3, 0, vin3.Length);
                  
                 
                    // Format the raw data read from the OBDII module.
                    valueToSend = System.Text.Encoding.ASCII.GetString(vin1)
                        + System.Text.Encoding.ASCII.GetString(vin2)
                        + System.Text.Encoding.ASCII.GetString(vin3);
                   // valueToSend = valueToSend.Substring(4, valueToSend.Length - 4);
                    valueToSend = Regex.Replace(valueToSend, @"0902", "");
                    valueToSend = Regex.Replace(valueToSend, @" ", "");
                    valueToSend = Regex.Replace(valueToSend, @"\r", "");
                    valueToSend = Regex.Replace(valueToSend, @">", "");

                    valueToSend = Regex.Replace(valueToSend, @"\0", "");

                   
                    string res = String.Empty;

                    for (int a = 0; a < valueToSend.Length; a = a + 2)
                    {
                        string Char2Convert = valueToSend.Substring(a, 2);
                        int n = Convert.ToInt32(Char2Convert, 16);
                        char c = (char)n;

                        res += c.ToString();

                    }

                        

                    res = Regex.Replace(res, @"I", "");
                    valueToSend = res;
                    valueToSend = Regex.Replace(valueToSend, @"\0", "");
                    valueToSend = Regex.Replace(valueToSend, @" ", "");
                    valueToSend = Regex.Replace(valueToSend, @"[^a-zA-Z0-9]", "");
                }
                
                // If the data element is for something other than the VIN, then use the following code
                // to poll for data.
                else
                {
                    writeString = "01" + obdPID + "\r";


                    try
                    {
                        // Encode the writeString, then write it to the OBDII module. 
                        byte[] writeCode = System.Text.Encoding.ASCII.GetBytes(writeString);
                        peerStream.Write(writeCode, 0, writeCode.Length);

                        // Wait 10 seconds for the OBDII module to process the code request
                        System.Threading.Thread.Sleep(10000);

                        // Read the OBDII code data from the OBDII module.
                        peerStream.Read(returnData, 0, returnData.Length);
                        MessageBox.Show(System.Text.Encoding.ASCII.GetString(returnData));
                    }


                    // Attempt to reconnect to OBDII device.
                    // If connection is true, recall RequestDataFromCar(), starting the process over.
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

                    // If the vehicle doesn't support this particular vehicle code, it will return "NO DATA",
                    // which we are handling by sending an empty string to the server.
                    if (System.Text.Encoding.ASCII.GetString(returnData).Contains("NO DATA"))
                    {
                        valueToSend = "";
                        noDataCheck = true;
                    }
                    else
                    {

                        // Parse the actual vehicle data from the bytes returned.
                        if (returnDataSize == 1)
                        {
                            hexLiteral = System.Text.Encoding.ASCII.GetString(returnData, 11, 1) + System.Text.Encoding.ASCII.GetString(returnData, 12, 1);
                            equVals[0] = Convert.ToInt32(hexLiteral, 16);
                        }
                        else if (returnDataSize == 2)
                        {
                            hexLiteral = System.Text.Encoding.ASCII.GetString(returnData, 11, 1) + System.Text.Encoding.ASCII.GetString(returnData, 12, 1);
                            hexLiteral2 = System.Text.Encoding.ASCII.GetString(returnData, 14, 1) + System.Text.Encoding.ASCII.GetString(returnData, 15, 1);
                            equVals[0] = Convert.ToInt32(hexLiteral, 16);
                            equVals[1] = Convert.ToInt32(hexLiteral2, 16);
                        }
                    }
                    
                }
            }

            // If connection is lost, print to screen.
            else 
            {
                MessageBox.Show("Lost BT Connection", "My Application",
                 MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                return false;
            }
                     
            return true;
        }


        /// <summary>
        /// Format the data correctly, using the specified equations for each type of data element.
        /// </summary>
        public void FormatData()
        {
            // If the data element is for the VIN, then its already formatted, proceed if otherwise.
            if (name != "VIN")
            {
                MessageBox.Show(name);
                // Create an expression with the equation specified.
                Expression expr = new Expression(equation);

                // Send empty string if no data was read from OBDII device.
                if (noDataCheck == true)
                    valueToSend = "";

                // Compute values to store in the data cache.
                else
                {
                    // If the equation only has an "A"
                    if (returnDataSize == 1)
                    {
                        //expr.Parameters["A"] = byteList[1].ToString();

                        expr.Parameters["A"] = equVals[0];
                    }


                    // If the equation has A and B...
                    else if (ReturnDataSize == 2)
                    {
                        expr.Parameters["A"] = equVals[0];
                        expr.Parameters["B"] = equVals[1];
                    }

                    // evaluate the expression with the variables
                    object answerToExpression = expr.Evaluate();

                    // Store the formatted answer in the valueToSend variable.
                    ValueToSend = answerToExpression.ToString();

                }
            }
            return;
        }

        // Still use??
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

        public string ObdMode { get { return obdMode; } set { obdMode = value; } }

        public string Name { get { return name; } set { name = value; } }

        public string DataType { get { return dataType; } set { dataType = value; } }

        public int ReturnDataSize { get { return returnDataSize; } set { returnDataSize = value; } }

        public byte[] ReturnData { get { return returnData; } set { returnData = value; } }

        public string ValueToSend { get { return valueToSend; } set { valueToSend = value; } }

        public string Equation { get { return equation; } set { equation = value; } }

        public bool TestOBDII { get { return testOBDII; } set { testOBDII = value; } }

        public bool TestVehicleData { get { return testVehicleData; } set { testVehicleData = value; } }

    }
}
