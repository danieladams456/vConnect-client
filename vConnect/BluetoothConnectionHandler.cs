using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace vConnect
{
    /// <summary>
    /// This class manages the BT connection.
    /// </summary>
    public class BluetoothConnectionHandler
    {
        private string deviceID = "";
        private bool bTConnectionStatus = false;
        private string errorMessageToUI = "";
        private BluetoothAddress bluetoothAddress;
        private BluetoothEndPoint endpoint;
        private Guid serviceClass;
        private BluetoothClient client;
        private int connectLoop = 0;

        // can keep this static for our purposes, but should probably implement 
        // a method for user specified PIN just in case.
        private string PIN = "1234";

        // Number of times vConnect with attempt to connect to the application. 
        private int connectionAttempts = 5;

  
        public bool EstablishBTConnection()
        {
            
            serviceClass = BluetoothService.SerialPort;
            endpoint = new BluetoothEndPoint(bluetoothAddress, serviceClass);
            client = new BluetoothClient();
            
             client.SetPin(PIN);
            
            // Tries to connect, catches exception is connection fails,
            // and then will try to connect 5 more times before giving it up.
             try { client.Connect(endpoint); }

             catch ( Exception ex)
             {
                 if (connectLoop < connectionAttempts)
                 {
                     connectLoop++;
                     EstablishBTConnection();
                 }
                 else
                 {
                     var msg = "failed to connect to BT Device. ERROR: " + ex;
                     MessageBox.Show(msg);
                     SendWindowsErrorMessage();
                     return false;

                 }

             }
            
            if (client.Connected)
            {
                bTConnectionStatus = true;
                connectLoop = 0;
                return true;
            }

            return false;
        }



        /// <summary>
        /// Closes the current BT Connection, if one exists. 
        /// </summary>
        /// <returns></returns>
        public bool CloseBTConnection()
        {
            if (client.Connected) { client.Close(); return true; }
            else
            {
                MessageBox.Show("No connection to close.", "My Application",
                  MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                return false;
            }

        }

        public bool SendWindowsErrorMessage()
        {

        // Code should work for sending error message to event log. However, admin must either run this program
        // or, more simply, have the admin privileges during installation and register a event log source 
        // for this program. Answer from 
            // http://stackoverflow.com/questions/9564420/the-source-was-not-found-but-some-or-all-event-logs-could-not-be-searched
        /*
            string msg = "vConnect failed to connect to the BT device with address" + bluetoothAddress;
            EventLog vConnectLog = new EventLog();
            EventLog.CreateEventSource("vConnect", "vConnect");
            vConnectLog.Source = "vConnect";
            vConnectLog.WriteEntry(msg);

          */  
            return true;
        }

        // C# Style Accessors. Refer to DataElement.cs for example usage.

        public string DeviceID { get { return deviceID; } set { deviceID = value; } }
        public bool BTConnectionStatus { get { return bTConnectionStatus; } set { bTConnectionStatus = value; } }
        public string ErorrMessageToUI { get { return errorMessageToUI; } set { errorMessageToUI = value; } }
        public BluetoothAddress BluetoothAddress { get { return bluetoothAddress; } set { bluetoothAddress = value; } }

        public BluetoothClient Client { get { return client; } set { client = value; } }


    }
}
