﻿/* BluetoothConnectionHandler.cs - vConnect (Liberty University CSCI Capstone Project)
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
        private const string PIN = "1234";

        // Number of times vConnect with attempt to connect to the application. 
        private int connectionAttempts = 1;

        // Testing values
        private bool bT_Test = false;
 
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
                BluetoothClient client = new BluetoothClient();
            }
            catch (PlatformNotSupportedException)
            {
                // Write the error to the log.
                Form1.LogMessageToFile("Bluetooth Failure", "The application does not support the client's Bluetooth Stack.");
                
                // Inform the user why the application will not run
                MessageBox.Show("vConnect does not support the client's Bluetooth Stack.\n\nConsult the documentation for supported ones.","Quitting");

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
        { /// CHECK THAT ITS A OBDII MODULE BREH
            // Close connection if already established. 
            // Not sure if we want to try and auto-connect to previous. 
            bool catchBool = false;
            if (client.Connected)
                client.Close();

            // Initialize serviceClass and endpoint.
            serviceClass = BluetoothService.SerialPort;
            endpoint = new BluetoothEndPoint(bluetoothAddress, serviceClass);

            // Set the PIN to be used in the connection attempt.
            client.SetPin(PIN);

            // Tries to connect, catches exception is connection fails,
            // and then will try to connect seven more times before giving it up.
            try { client.Connect(endpoint); }

            catch (Exception ex)
            {
                if (connectLoop < connectionAttempts)
                {
                    connectLoop++;
                    EstablishBTConnection();
                }
                // If connection cannot be established after seven attempts, send Windows Error Message,
                // and print message to the screen.
                else
                {
                    var msg = "failed to connect to BT Device. ERROR:\n\n " + ex;
                    MessageBox.Show(msg);
                    Form1.LogMessageToFile("BT Connection ERROR", msg);
                    catchBool = true;
                }
            }
           

          /*  if (deviceInfo.DeviceName != "CBT." || deviceInfo.DeviceName != "OBDII")
            {
                MessageBox.Show("ERROR: attempted to connect to a non-OBDII device");
                return false;


            }*/


            if (bT_Test)
            {
                MessageBox.Show("BT Connection Reestablished, Test successful.");
                connectionAttempts = 0;
                bTConnectionStatus = true;
            }

            // If connection is established, set the check  bool value to true, and save the OBDII device's
            // Name and BT address. 
            if (client.Connected)
            {
                bTConnectionStatus = true;
                // Properties.Settings.Default.BTDeviceName = deviceID;
                //Properties.Settings.Default.BTAddress = bluetoothAddress.ToString();
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
                Properties.Settings.Default.BTAddress = "";
                Properties.Settings.Default.BTDeviceName = "";
                client.Close();
                return true;
            }
            // If there is no connection to close, print to the screen, and print to screen.
            else
            {
                MessageBox.Show("No connection to close.", "My Application",
                  MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
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
        public bool BT_Test { get { return bT_Test; } set { bT_Test = value; } }
        public BluetoothClient Client { get { return client; } set { client = value; } }
        public BluetoothDeviceInfo DeviceInfo { get { return deviceInfo; } set { deviceInfo = value; } }


    }
}
