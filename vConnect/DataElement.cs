using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        // This connection gets passed from the caller. It is the current connection.
        public BluetoothConnectionHandler BTConnection;

        // This defines the largest potential size of return data from the car. It should never
        //  really even get close to this value
        private int MAX_DATA_SIZE= 20;

        /// <summary>
        /// Constructor that defines name and PID and number of bytes explicitly 
        /// </summary>
        /// <param name="elementName">name of the element to be collected</param>
        /// <param name="PID">OBD PID to send to the car to get this value</param>
        /// <param name="numberBytesReturned">Number of bytes the car will return</param>
        /// <param name="btconnection">Current BT ConnectionHandler object.</param>
        public DataElement(string elementName, string PID, int numberBytesReturned, BluetoothConnectionHandler btconnection)
        {
            name = elementName;
            obdPID = PID;
            returnData = new byte[numberBytesReturned];
            BTConnection = btconnection;
        }

        /// <summary>
        /// Default constructor if no parameters passed
        /// </summary>
        public DataElement()
        {
            returnData = new byte[MAX_DATA_SIZE];
        }

        public bool RequestDataFromCar()
        {
            if (BTConnection.Client.Connected)
            {
                Stream peerStream = BTConnection.Client.GetStream();
                string writeString = "01"+ ObdPID + "\r";

                byte[] writeCode = System.Text.Encoding.ASCII.GetBytes(writeString);
                peerStream.Write(writeCode, 0, writeCode.Length);

                System.Threading.Thread.Sleep(10000);

                
                byte[] rawTest =  new byte[20];
                
                
                peerStream.Read(rawTest, 0, rawTest.Length);

                byte[] test = new byte[20];
                Buffer.BlockCopy(rawTest, 2, test, 0, 15);
                string codeString = System.Text.Encoding.ASCII.GetString(test);

                MessageBox.Show(codeString, "My Application",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
            }
             else 
              {
                MessageBox.Show("Something bad happened", "My Application",
                 MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
              
              }
             

        
            /*
           
               
			
			
			    // Since the data received back repeats the mode (e.g. Mode 01) and the PID (e.g. "0D"), we will have a 
			    // 	temporary holding space for the response.
			    // 	For example, the response will be like "410D17".
			    int recievedRaw = 0;
			    recievedRaw = listenForResponse();

			    // Depending on the size of the actual return data we are looking for, shift the raw response the given
			    // 	number of bytes and store it in returnData. We do not want the first two bytes (e.g. 410D)
			    // 	See Maciek's answer on http://stackoverflow.com/questions/1318933/c-sharp-int-to-byte for more details.
			    returnData = (byte)(recievedRaw >> 8*getReturnDataSize());	
		
		    }
		   
            */

            return true;
            
        }


        public void FormatData()
        {
            //
            // Place code here to package/format the data and add it to the cache.
            //
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


    }
}
