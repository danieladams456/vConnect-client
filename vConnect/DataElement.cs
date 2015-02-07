﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vConnect
{
    class DataElement
    {
        /// <summary>
        /// This class describes a data element received from the car via BT.
        /// </summary>
        private string obdPID = "";
        private string name = "";
        private int returnDataSize = 0;
        private byte[] returnData;

        // This connection gets passed from the caller. It is the current connection.
        BluetoothConnectionHandler BTConnection;

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
            /*
            if (BT_Connection_Open)
            {
                //Craft the message to send to the car according to 
			    // http://stackoverflow.com/questions/20477092/how-can-i-read-write-data-to-and-from-an-obd-ii-adapter-with-windows-phone-8
			    string messageToSend = "01" + getobdPID() + "\r";
			
			    // Send the message to the car.
			    //	For example, send "010D\r".
			    WhateverBTCommandsAreNeededToPushDataToCar_Here(messageToSend);
			
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
		    else 
		    {
    			AttemptToReestalblishBTConnection
			    requestDataFromCar(obdPID,returnDataSize);
		    }
             * */

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

        public string getobdPID()
        {
            return obdPID;
        }

        public int getReturnDataSize()
        {
            return returnDataSize;
        }
    }
}
