/* BluetoothConnectionHandler.cs - vConnect (Liberty University CSCI Capstone Project)
 * Written by Troy Cosner and Charlie Snyder in February-May, 2015. 
 * 
 * Contains methods that manage BT connections with OBDII devices, including connecting,
 * disconnecting, and automatic reconnection attempts. Also performs integrity checks
 * of the BT connection with the OBDII module, as well as changes the OBDII module's 
 * settings upon initial connection. 
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
        private BluetoothAddress bluetoothAddress;      // Class object that contains formatted BT address.
        private BluetoothEndPoint endpoint;             // Class object that contains data for BT endpoint.
        private int connectLoop = 0;                    // Integer used to keep track of automatic reconnect attempts. 
        private BluetoothDeviceInfo deviceInfo = null;  // Object stores device info regarding connected OBDII module.
        private BluetoothClient client = null;          // Class object that contains client info.
        private string pIN = "0";                       // PIN to be used in connection with the BT module.

        const int CONNECTIONATTEMPTS = 4;         // Number of times vConnect will attempt to connect to the application. 


        /// <summary>
        /// This constructor exists to ensure that the application is running on a computer with a Bluetooth stack
        /// supported by 32Feet, the Bluetooth Library used by this application. If a supported stack is not found,
        /// this constructor presents the error, logs it, then quietly exits.
        ///     
        /// Note: If the client's Bluetooth Stack is not supported, the application will never proceed.
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
                Form1.LogMessageToFile("error", "BluetoothConnectHandler()", "vConnect does not support the client's Bluetooth Stack.");
                // Inform the user why the application will not run
                MessageBox.Show("vConnect does not support the client's Bluetooth Stack.\n\nConsult the documentation for supported ones.", "Quitting");

                // Exit the application with a (1) for error.
                Environment.Exit(1);
            }
        }


        /// <summary>
        /// Function that attempts to connect to the BT device with address bluetoothAddress. 
        /// will close any current BT connection.
        /// Will also initialize the connection with the OBDII module via some AT commands 
        /// to increase data polling speed.
        /// </summary>
        /// <returns>
        /// True - If connection is successfully established.
        /// False - If connection is not established. 
        /// </returns>
        public bool EstablishBTConnection()
        {
            // Close connection if already established. 
            if (client.Connected)
            {
                Form1.peerStream.Close();
                client.Dispose();
            }

            // If connection is attempted with an BT device that isn't the OBDLink LX module,
            // inform the user that this OBDII module is not usable with this software.
            if (deviceID != "OBDLink LX")
            {
                MessageBox.Show("ERROR: attempted to connect to a non-OBDII device");
                deviceID = "";
                return false;
            }

            // Save the OBDII module's device information, since it is an OBDLink LX, so
            // vConnect will need this information later for reconnect attempts.
            else
            {
                Properties.Settings.Default.BTDeviceName = deviceID;
                Properties.Settings.Default.BTAddress = bluetoothAddress.ToString();
                Properties.Settings.Default.Save();
            }
            //pIN = "631434"; // THIS IS PIN FOR MY MODULE, DONT REMOVE, ITS HERE CAUSE ITS ALWAYS ON HAND.

            client = new BluetoothClient();         // Reinitialize client object for new connection.
            Guid serviceClass;                      // GUID object.

            serviceClass = BluetoothService.SerialPort;     // Process ID used for BT connection.       
            endpoint = new BluetoothEndPoint(bluetoothAddress, serviceClass); // Endpoint for the BT connection.

            // Set the PIN to be used in the connection attempt.
            client.SetPin(pIN);

            // Tries to connect to the specified endpoint.
            try { client.Connect(endpoint); }

            // Catch the exception that is the result of a failed connection attempt.
            catch (Exception ex)
            {
                // Will connect a maximum of 4 connection attempts to the specified endpoint by
                // recursively calling EstablishBTConnection(), as it is possible to fail a completely
                // legitimate connection attempt due to timing out the connection attempt. 
                if (connectLoop < CONNECTIONATTEMPTS)
                {
                    connectLoop++;
                    return EstablishBTConnection();
                }

                // If connection cannot be established after CONNECTIONATTEMPTS attempts, do the following...
                else
                {
                    // If the exception thrown contains the following, then it is possibly due to an attempted 
                    // connection to an OBDII module that disconnected improperly during this executable session.
                    // So, write a log recording this to event.log, and kill vConnect. The monitor program will
                    // restart it, which will then re-connect properly to the OBDII module, if able. 
                    if (ex.ToString().Contains("An invalid argument was supplied"))
                    {
                        Form1.LogMessageToFile("event", "EstablishBTConnection()", "Exiting vConnect");

                        Form1.LogMessageToFile("error", "EstablishBTConnection()", "Exiting vConnect due expected exception");
                        System.Threading.Thread.Sleep(500);
                        Application.Exit();
                        Environment.Exit(2);
                    }

                    // If some other connection exception, simply log the exception and return false.
                    else
                    {
                        var msg = "failed to connect to BT Device. ERROR:\n\n " + ex;
                        Form1.LogMessageToFile("error", "EstablishBTConnection()", msg);
                        connectLoop = 0;
                        return false;
                    }

                }
            }

            // If connection is established, perform an integrity check, and then finalize setting up the connection. 
            if (client.Connected)
            {

                Form1.LogMessageToFile("event", "EstablishBTConnection()", "Initial Connection Made with OBDII module");

                // Initilize the read/write stream with the OBDII module.
                Form1.peerStream = client.GetStream();

                // Perform an integrity check of the connection with the OBDII module.
                // If the connection passes the check, continue to finalize the connection.
                // If it does not pass, close the connection and return false.
                if (!IntegrityCheck())
                {
                    CloseBTConnection();
                    return false;
                }

                // These are AT commands, which will change the settings of the OBDII module
                // in order to increase data polling speed. 
                byte[] firstCode = System.Text.Encoding.ASCII.GetBytes("AT D\r");       // Set all to defaults 
                byte[] secondCode = System.Text.Encoding.ASCII.GetBytes("AT Z\r");      // reset all 
                byte[] thirdCode = System.Text.Encoding.ASCII.GetBytes("AT E0\r");      // Turn request echo off
                byte[] fourthCode = System.Text.Encoding.ASCII.GetBytes("AT L0\r");     // Turn linefeeds off
                byte[] fifthCode = System.Text.Encoding.ASCII.GetBytes("AT S0\r");      // Turn printing of spaces off
                byte[] sixthCode = System.Text.Encoding.ASCII.GetBytes("AT H0\r");      // Turn Headers off
                byte[] seventhCode = System.Text.Encoding.ASCII.GetBytes("AT SP 00\r");  // Set Protocol to auto.

                // Write the AT commands listed above to the OBDII module to finalize the connection.
                System.Threading.Thread.Sleep(500);
                Form1.peerStream.Write(firstCode, 0, firstCode.Length);
                System.Threading.Thread.Sleep(500);
                //Form1.peerStream.Write(second, 0, second.Length);
                System.Threading.Thread.Sleep(500);
                Form1.peerStream.Write(thirdCode, 0, thirdCode.Length);
                System.Threading.Thread.Sleep(500);
                Form1.peerStream.Write(fourthCode, 0, fourthCode.Length);
                System.Threading.Thread.Sleep(500);
                Form1.peerStream.Write(fifthCode, 0, fifthCode.Length);
                System.Threading.Thread.Sleep(500);
                Form1.peerStream.Write(sixthCode, 0, sixthCode.Length);
                System.Threading.Thread.Sleep(500);
                //Form1.peerStream.Write(seventh, 0, seventh.Length);
                System.Threading.Thread.Sleep(5000);

                // Read the responses from the AT commands and save them in event.log
                byte[] readBytes = new byte[100];
                Form1.peerStream.Read(readBytes, 0, readBytes.Length);
                Form1.LogMessageToFile("event", "EstablishBTConnection()", "Return values from AT commands: " + System.Text.Encoding.ASCII.GetString(readBytes));

                Properties.Settings.Default.BTAddress = bluetoothAddress.ToString();
                Properties.Settings.Default.Save();
                connectLoop = 0;
                Form1.LogMessageToFile("event", "EstablishBTConnection()", "Successfully finalized BT connection with OBDII module.");
                return true;
            }
            return false;
        }


        /// <summary>
        /// Sends a generic OBDII request to the OBDII module, and inspects its response to ensure 
        /// that the connection is legitimate. 
        /// </summary>
        /// <returns>
        /// True if the inspection of the response from a generic request is legitimate.
        /// False if the inspection of the response from a generic request is no legitimate. 
        /// </returns>
        public bool IntegrityCheck()
        {
            // Check if connection is currently established.
            if (client.Connected)
            {
                // Generic OBDII request. 
                byte[] testMessage = System.Text.Encoding.ASCII.GetBytes("010D\r");

                // Write the request to the OBDII module, and read the response.
                Form1.peerStream.Write(testMessage, 0, testMessage.Length);
                System.Threading.Thread.Sleep(1000);
                byte[] testRead = new byte[20];
                Form1.peerStream.Read(testRead, 0, testRead.Length);

                string check = System.Text.Encoding.ASCII.GetString(testRead);
                // Log the response in event.log
                Form1.LogMessageToFile("event", "IntegrityCheck()", "Integrity check value: " + check);
                
                // Check if the message returns contains any of these strings. Which would indicate that a legitimate BT connection was not established. 
                // IF so, return false, and log to error.log.
                if (check.Contains("SEARCHING") || check.Contains("BUS INIT") || check.Contains("UNABLE TO CONNECT") || check.Contains("ERROR") || check.Contains("NO DATA"))
                {
                    Form1.LogMessageToFile("error", "IntegrityCheck()", "Couldn't initialize OBDII connection. Faulty value: " + check);
                    return false;
                }

                return true;
            }
            
            // Return false if no connection is present.
            else
            {
                Form1.LogMessageToFile("error", "IntegrityCheck()", "Cannot perform integrity check when there is no connection.");
                return false;
            }
        }


        /// <summary>
        /// Closes the current BT Connection, if one exists. 
        /// </summary>
        /// <returns>
        /// True - The BT Connection was successfully closed.
        /// False - No BT Connection was closed available to be closed.
        /// </returns>
        public bool CloseBTConnection()
        {
            // If connected to a BT device, close the connection.
            if (client.Connected)
            {
                Form1.peerStream.Close();
                client.Dispose();
                Form1.LogMessageToFile("event", "CloseBTConnection()", "Closed BT connection with OBDII module");
                return true;
            }
            // If there is no connection to close, print to the screen, and print to screen.
            else
                return false;
        }


        // C# Style Accessors. Refer to DataElement.cs for example usage.
        public string DeviceID { get { return deviceID; } set { deviceID = value; } }
        public BluetoothAddress BluetoothAddress { get { return bluetoothAddress; } set { bluetoothAddress = value; } }
        public BluetoothClient Client { get { return client; } set { client = value; } }
        public BluetoothDeviceInfo DeviceInfo { get { return deviceInfo; } set { deviceInfo = value; } }
        public string PIN { get { return pIN; } set { pIN = value; } }
    }
}
