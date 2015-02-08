using System;
using System.IO;

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

        // can keep this static for our purposes, but should probably implement 
        // a method for user specified PIN just in case.
        private string PIN = "1234";

        public bool EstablishBTConnection()
        {
            serviceClass = BluetoothService.SerialPort;
            endpoint = new BluetoothEndPoint(bluetoothAddress, serviceClass);
            client = new BluetoothClient();
            
             
            
            client.SetPin(PIN);
            client.Connect(endpoint);

            // Lil message for testing. 
            if (client.Connected)
            {
                MessageBox.Show("We're connected!", "My Application",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
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
            return true;
        }

        // C# Style Accessors. Refer to DataElement.cs for example usage.

        public string DeviceID { get { return deviceID; } set { deviceID = value; } }
        public bool BTConnectionStatus { get { return bTConnectionStatus; } set { bTConnectionStatus = value; } }
        public string ErorrMessageToUI { get { return errorMessageToUI; } set { errorMessageToUI = value; } }
        public BluetoothAddress BluetoothAddress { get { return bluetoothAddress; } set { bluetoothAddress = value; } }


    }
}
