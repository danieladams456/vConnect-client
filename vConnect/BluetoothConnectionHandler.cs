/* BluetoothConnectionHandler.cs - vConnect (Liberty University CSCI Capstone Project)
 * Written by Troy Cosner and Charlie Snyder in February-March, 2015. 
 * Contains methods that manage BT connections with OBDII devices, including connecting,
 * disconnecting, and automatic reconnection attempts.
 * 
 * 
 */
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

        private string deviceID = "";                   // Name of the BT device that is connected. 
        private bool bTConnectionStatus = false;        // Keeps track of whether or not any BT 
        //      connection is currently ongoing.
        private string errorMessageToUI = "";           // Error Message to be used.
        private BluetoothAddress bluetoothAddress;      // Class object that contains formatted BT address.
        private BluetoothEndPoint endpoint;             // Class object that contains data for BT endpoint.
        private Guid serviceClass;                      // Process ID used for BT connection.
        private int connectLoop = 0;                    // Integer used to keep track of automatic 
        //      reconnect attempts. 
        private BluetoothDeviceInfo deviceInfo = null;
        private BluetoothClient client = null;          // Class object that contains client info.

        // can keep this static for our purposes, but should probably implement 
        // a method for user specified PIN just in case.
        private string pIN = "0";

        // Number of times vConnect with attempt to connect to the application. 
        private int connectionAttempts = 3;



        /// <summary>
        /// This constructor exists to ensure that the application is running on a computer with a Bluetooth stack
        ///     supported by 32Feet, the Bluetooth Library used by this application. If a supported stack is not found,
        ///     this constructor presents the error, logs it, then quietly exits.
        ///     
        ///     Note: If the client's Bluetooth Stack is not supported, the application will never proceed.
        /// </summary>
        public BluetoothConnectionHandler()
        {
            try
            {
                //  Class object that contains client info.
                client = new BluetoothClient();
            }
            catch (PlatformNotSupportedException)
            {
                // Write the error to the log.
                Form1.LogMessageToFile("error","Bluetooth Failure", "The application does not support the client's Bluetooth Stack.");

                // Inform the user why the application will not run
                MessageBox.Show("vConnect does not support the client's Bluetooth Stack.\n\nConsult the documentation for supported ones.", "Quitting");

                // Exit the application with a (1) for error.
                Environment.Exit(1);
            }
        }



        /// <summary>
        /// Function that attempts to connect to the BT device with address bluetoothAddress. 
        /// will close any current BT connection. 
        /// </summary>
        /// <returns>
        /// True - If connection is successfully established.
        /// False - If connection is not established. 
        /// </returns>
        public bool EstablishBTConnection()
        {
            // Close connection if already established. 
            // Not sure if we want to try and auto-connect to previous. 
  //          if (client.Connected)
    //        {
      //          MessageBox.Show("Close current BT connection before selecting another device.");
        //        return true;
         //   }
            //if (client.Connected)
              //  client.Dispose();
            //            if (deviceID != "OBDLink LX")
            //           {
            //              MessageBox.Show("ERROR: attempted to connect to a non-OBDII device");
            //             deviceID = null;
            //            return false;
            //       }
            pIN = "631434";
            client = new BluetoothClient();
            // Initialize serviceClass and endpoint.
           // serviceClass = BluetoothService.SerialPort;
            serviceClass = new Guid("{00112233-4455-6677-8899-aabbccddeeff}");
            endpoint = new BluetoothEndPoint(bluetoothAddress, serviceClass);
            // Set the PIN to be used in the connection attempt.
            client.SetPin(pIN);
            // Tries to connect, catches exception is connection fails,
            // and then will try to connect seven more times before giving it up.
            Form1.LogMessageToFile("event","BTCONNECTION", "BEfore try to connect");
            Form1.LogMessageToFile("event","BTCONNECTION", bluetoothAddress.ToString());
            Form1.LogMessageToFile("event","BTCONNECTION", serviceClass.ToString());
            try { client.Connect(endpoint); }

            catch (Exception ex)
            {
                if (connectLoop < connectionAttempts)
                {
                    connectLoop++;
                   return EstablishBTConnection();
                }
                // If connection cannot be established after seven attempts, send Windows Error Message,
                // and print message to the screen.
                else
                {
                    var msg = "failed to connect to BT Device. ERROR:\n\n " + ex;
                    Form1.LogMessageToFile("error","BT Connection ERROR", msg);
                    connectLoop = 0;
                    return false;
                }
            }

            // If connection is established, set the check  bool value to true, and save the OBDII device's
            // Name and BT address. 
            if (client.Connected)
            {
                Form1.LogMessageToFile("event","BTCONNECTION", "in IF STSTEMETN");


                Form1.peerStream = client.GetStream();
                byte[] test = System.Text.Encoding.ASCII.GetBytes("010D\r");
                Form1.peerStream.Write(test, 0, test.Length);
                System.Threading.Thread.Sleep(500);
                byte[] testRead = new byte[30];
                Form1.peerStream.Read(testRead,0,testRead.Length);

                string check = System.Text.Encoding.ASCII.GetString(testRead);
              
                if (check.Contains("SEARCHING") || check.Contains("BUS INIT") || check.Contains("UNABLE TO CONNECT") || check.Contains("ERROR"))
                {
                    MessageBox.Show("Invalid PIN number or vehicle is off.");
                    CloseBTConnection();
                    return false;
                }
                byte[] first= System.Text.Encoding.ASCII.GetBytes("AT D\r");
                byte[] second= System.Text.Encoding.ASCII.GetBytes("AT Z\r");

                byte[] third= System.Text.Encoding.ASCII.GetBytes("AT E0\r");

                byte[] fourth= System.Text.Encoding.ASCII.GetBytes("AT L0\r");

                byte[] fifth= System.Text.Encoding.ASCII.GetBytes("AT S0\r");

                byte[] sixth= System.Text.Encoding.ASCII.GetBytes("AT H0\r");

                byte[] seventh= System.Text.Encoding.ASCII.GetBytes("AT SP 0\r");

                Form1.peerStream.Write(first, 0, first.Length);
                System.Threading.Thread.Sleep(500);

//                Form1.peerStream.Write(second, 0, second.Length);
                System.Threading.Thread.Sleep(500);

                Form1.peerStream.Write(third, 0, third.Length);
                System.Threading.Thread.Sleep(500);

                Form1.peerStream.Write(fourth, 0, fourth.Length);
                System.Threading.Thread.Sleep(500);

                Form1.peerStream.Write(fifth, 0, fifth.Length);
                System.Threading.Thread.Sleep(500);

                Form1.peerStream.Write(sixth, 0, sixth.Length);
                System.Threading.Thread.Sleep(500);

            //    Form1.peerStream.Write(seventh, 0, seventh.Length);
                System.Threading.Thread.Sleep(500);

                byte[] read = new byte[100];
                Form1.peerStream.Read(read, 0, read.Length);
                bTConnectionStatus = true;
                Properties.Settings.Default.BTAddress = bluetoothAddress.ToString();
                Properties.Settings.Default.Save();
                connectLoop = 0;
                return true;
            }

            return false;
        }



        /// <summary>
        /// Closes the current BT Connection, if one exists. 
        /// </summary>
        /// <returns>
        /// True - The BT Connection was successfully closed.
        /// False - No BT Connection was closed.
        /// </returns>
        public bool CloseBTConnection()
        {
            // If connected to a BT device, close the connection and clear saved BT device name and address.
            if (client.Connected)
            {
                Properties.Settings.Default.BTAddress = null;
                Properties.Settings.Default.BTDeviceName = null;
                Form1.peerStream.Close();
                client.Dispose();
                Form1.LogMessageToFile("event","CLOSER", "REached this point");
                return true;
            }
            // If there is no connection to close, print to the screen, and print to screen.
            else
            {
             
                return false;
            }

        }

        /// <summary>
        /// Send an error message to the Windows Event Log.
        /// </summary>
        /// <returns>
        /// True - A Error message was sucessfully logged to the Windows Event Log
        /// False - The error message was not logged to the Windows Event Log.
        /// </returns>
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
        public BluetoothDeviceInfo DeviceInfo { get { return deviceInfo; } set { deviceInfo = value; } }
        public string PIN { get { return pIN; } set { pIN = value; } }
        public bool ConnectionStatus { get { return client.Connected;  } }
    }
}
