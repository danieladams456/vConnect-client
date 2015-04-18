/* Form1.cs - vConnect (Liberty University CSCI Capstone Project)
 * Written by Troy Cosner and Charlie Snyder in February-March, 2015. 
 * 
 * This class creates the Windows Form that can be accessed by clicking the
 * tray icon. It also contains the code that will be running "in the background"
 * for the duration of the program's running-time. It requires interaction with the 
 * DataCache, DataElement, and BluetoothConnectionHandler classes. It also relies heavily
 * on 32feet (open-source Bluetooth library) and Json.net (open-source .NET-JSON library)
 */
using System;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using InTheHand.Net.Ports;
using InTheHand.Net.Bluetooth;
using InTheHand.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Text.RegularExpressions;




namespace vConnect
{


    public partial class Form1 : Form
    {
        // Instantiates objects from the classes BluetoothConnectionHandler, ServerConnectionHandler,
        // and a null object of DataCache.
        BluetoothConnectionHandler BTConnection = new BluetoothConnectionHandler();
        DataCache cache = null;

        // Instantiates a timer used to poll data from the OBDII module.
        static public System.Threading.Timer pollData;

        int failCounter = 0;
        int succeedCounter = 0;
        // List that will used to hold OBDII codes before being inserted into the cache.
        List<Dictionary<string, object>> elementDictionaryList =
                                                new List<Dictionary<string, object>>();

        // String that will hold the current schema.
        String schema = "";

        // Bool value specifying whether the data polling asychronous operation is currently
        // running or not. 
        static public bool pollingData = false;
        static public Stream peerStream;
        //  BluetoothWin32Events x;


        // Constant that determines how often the data polling Timer will run. (In miliseconds)
        const int POLLTIME = 20000;



        // Timer CallBack to be used for polling data.
        TimerCallback tcb;

        public Form1()
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;

            // Initialize the dataCache.
            cache = new DataCache();
            // Create a Timer callback method for polling data. 
            tcb = RequestDataForElements;
            Microsoft.Win32.SystemEvents.PowerModeChanged += OnPowerChange;
            //  x = BluetoothWin32Events.GetInstance();
            //  x.InRange += OnInRange;
            //  x.OutOfRange += OnOutOfRange;
            // Bool variable to record whether an OBDII device and server have been successfully detected and pinged/established connection. 
            bool addrCheck = true;
            // If there is a saved BT Address, attempt to connect with the device with that address.
            if (Properties.Settings.Default.BTAddress != "" && Properties.Settings.Default.PIN != "" && Properties.Settings.Default.BTDeviceName != "")
            {
                Form1.LogMessageToFile("event", "Constructor", "Detected saved OBDII Information.");
                // Grabs the saved BT address from the settings file.
                BTConnection.BluetoothAddress = BluetoothAddress.Parse(Properties.Settings.Default.BTAddress);
                BTConnection.PIN = Properties.Settings.Default.PIN;
                BTConnection.DeviceID = Properties.Settings.Default.BTDeviceName;
                BT_ID.Text = Properties.Settings.Default.BTDeviceName;

                // If connection is established with the device with the specified BT address above,
                // save the Device's ID, indicated connection status on the GUI.
                if (BTConnection.EstablishBTConnection())
                {
                    LogMessageToFile("event", "Constructor", "Automatic BT connection successful");
                    BTConnection.DeviceID = Properties.Settings.Default.BTDeviceName;
                    BT_ID.Text = Properties.Settings.Default.BTDeviceName;
                    device_Status_Label.Text = "Connected";
                }

            }
            else
                addrCheck = false;


            // Checks if any server connection data is saved in the settings file. If so, attempts to see if connection
            // can be established.
            if (Properties.Settings.Default.ServerIP != "" && Properties.Settings.Default.ServerPort != "")
            {
                LogMessageToFile("event", "Constructor", "Server information detected");
                cache.PortNumber = Convert.ToInt32(Properties.Settings.Default.ServerPort);
                cache.IPAddress = Properties.Settings.Default.ServerIP;
                port_number.Text = Properties.Settings.Default.ServerPort;
                server_IP.Text = Properties.Settings.Default.ServerIP;
            }
            else
                MessageBox.Show("No server connection data was found, please add server IP address and port number");


            schema = SchemaUpdate();

            if (addrCheck)
            {
                if (schema != "NOT FOUND")
                {
                    poll_status.Text = "Polling";

                    pollingData = true;
                    LogMessageToFile("event", "Constructor", "Polling Loop initiated and started");

                    pollData = new System.Threading.Timer(tcb, null, 0, POLLTIME);
                }
                else
                {
                    MessageBox.Show("No Schema Found. Update Schema.");
                    LogMessageToFile("event", "Constructor", "Schema file is empty");
                    LogMessageToFile("event", "Constructor", "Polling Loop initiated, but not started");
                    pollData = new System.Threading.Timer(tcb, null, Timeout.Infinite, Timeout.Infinite);
                }
            }
            else
            {
                pollData = new System.Threading.Timer(tcb, null, Timeout.Infinite, Timeout.Infinite);
                LogMessageToFile("event", "Constructor", "No OBDII data was detected");
                LogMessageToFile("event", "Constructor", "Polling Loop initiated, but not started");
                MessageBox.Show("No OBDII Connection info detected. Please set up OBDII Connection.");
            }
        }


        /*   private void OnInRange(object sender, BluetoothWin32RadioInRangeEventArgs e)
           {


           }

           private void OnOutOfRange(object sender, BluetoothWin32RadioOutOfRangeEventArgs e)
           {


           }
           */

        /// <summary>
        /// Closes the Application GUI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// This button will display a help box detailing the GUI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void help_button_Click(object sender, EventArgs e)
        {
            string helpMessage = "This GUI is used to setup and manange vConnect's Windows Aplication. \n\n " +
                   "Please note that this GUI cannot manage the server, or search for stored data.\n\n" +
                   "Listed here are the details concerning the various attributes of this GUI:\n\n" +
                   "BT Device ID: The ID of the OBDII Device that is currently assigned to vConnect.\n\n" +
                   "Device Status: Whether the OBDII Device listed above is connected or disconnect. \n\n" +
                   "Connect to OBDII Device: Will open up a dialog that shows all detectable BT Devices, " +
                   "selecting a device will attempt to connect with it. \n\n" +
                   "Disconnect BT Device: Will disconnect to the OBDII device (if one is connected).\n\n" +
                   "Server IP Address: The IP address that is assigned to vConnect, the edit button will " +
                   "alter this value.\n\n" +
                   "Server Port Number: The port number that is assigned to vConnect, the edit button will " +
                   "alter this value.\n\n" +
                   "Server Status: Whether the vConnect is currently connected with the server with the IP address " +
                   " and port number assigned to vConnect.\n\n" +
                   "Update Schema: Will query the vConnect server, and update the schema if it is out of data.\n\n" +
                   "Start: Will begin polling for data. \n\n" +
                   "Stop: Will stop polling for data.";


            MessageBox.Show(helpMessage);


        }

        /// <summary>
        /// This function displays the most recent NUM_LINES lines from the error log, for ease of use.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void view_error_log_button_Click(object sender, EventArgs e)
        {
            int NUM_LINES = 5;
            try
            {
                MessageBox.Show("The 5 most recent error messages recorded are: \n\n" + string.Join("\r\n", File.ReadLines("error.log").Reverse().Take(NUM_LINES).Reverse()), "Error Log");
            }
            catch (FileNotFoundException)
            {
                LogMessageToFile("error", "Error Log", "The error log could not be found/opened.");
            }
        }

        /// <summary>
        ///  Allows the user to enter a new IP address using the GUI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void edit_IP_button_Click(object sender, EventArgs e)
        {

            string value = "IP Address";

            // Saves IP address to the settings file, as well as the server connection handler. 
            if (InputBox("New IP Address", "New IP Address:", ref value) == DialogResult.OK)
            {
                cache.IPAddress = value;

                server_IP.Text = value;
                Properties.Settings.Default.ServerIP = value;
                Properties.Settings.Default.Save();
                LogMessageToFile("event", "IP Address Button", "IP address changed.");
            }

        }

        /// <summary>
        ///  Allows the user to enter a new port number using the GUI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void edit_port_button_Click(object sender, EventArgs e)
        {

            string value = "Port Number";
            if (InputBox("New Port Number", "New Port Number (1-65535):", ref value) == DialogResult.OK)
            {

                // Bounds checking for a valid port number
                // Saves Port Number to the settings file, as well as the server connection handler.
                int valueInt = 0;
                try
                {
                    valueInt = Int32.Parse(value);
                    if (valueInt > 0 && valueInt < 65535)
                    {
                        cache.PortNumber = Int32.Parse(value);

                        Properties.Settings.Default.ServerPort = value;
                        Properties.Settings.Default.Save();
                        port_number.Text = value;
                        LogMessageToFile("event", "Port Number Button", "Port number changed.");

                    }
                    else
                    {
                        MessageBox.Show("Invalid Port Number: Port number must be between 1 and 65534.");
                        LogMessageToFile("error", "Port Number", "Invalid Port number.");

                    }

                }

                catch
                {
                    MessageBox.Show("Invalid Port Number: Port number must be between 1 and 65534.");
                    LogMessageToFile("error", "Port Number", "Invalid Port number.");

                }
            }

        }


        /// <summary>
        /// Button opens up a Dialog box to select a BT device to attempt to connect to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browse_button_Click(object sender, EventArgs e)
        {
            if (pollingData)
                MessageBox.Show("Must stop polling data before changing OBDII device.");
            else
            {
                var msg = "Please disconnect current OBDII connection " +
                    "before connecting to a new OBDII device";

                // If there is already an established connection with an OBDII device, then
                // prompt the user to disconnect with it before attempting to browse for a new device.
                if (BTConnection.Client.Connected)
                    MessageBox.Show(msg);

                // Open a dialog box that will show all detectable BT Devices. Selecting a device 
                // will save its name and address. 
                else
                {
                    var dlg = new SelectBluetoothDeviceDialog();
                    DialogResult result = dlg.ShowDialog(this);
                    if (result != DialogResult.OK)
                    {
                        return;
                    }
                    BluetoothDeviceInfo device = dlg.SelectedDevice;
                    BluetoothAddress BTaddr = device.DeviceAddress;
                    BTConnection.BluetoothAddress = BTaddr;
                    BTConnection.DeviceID = device.DeviceName;

                    // If connection is successfully esablished, save the device's name and address to the 
                    // settings file, and update the device status on the GUI to "connected.
                    if (BTConnection.EstablishBTConnection())
                    {
                        device_Status_Label.Text = "Connected";
                        LogMessageToFile("event", "Browse BT Device Button", "OBDII device connected via browse button");
                        BT_ID.Text = device.DeviceName;

                    }
                }
            }
        }


        /// <summary>
        /// Button that will attempt to close the BT connection with an OBDII device, if one exists. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void disconnect_BT_button_Click(object sender, EventArgs e)
        {
            if (pollingData)
            {
                MessageBox.Show("Stop Polling Data before disconnecting OBDII device.");
            }
            else
            {
                if (!BTConnection.Client.Connected)
                    MessageBox.Show("There are no OBDII devices connected.");
                else if (BTConnection.CloseBTConnection())
                {
                    device_Status_Label.Text = "Disconnected";
                }
            }

        }




        /// <summary>
        /// Start polling data from the vehicle, if able. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void start_button_Click(object sender, EventArgs e)
        {
            start_polling();
        }


        private void start_polling()
        {
            if (pollingData)
            {
                MessageBox.Show("Already Polling Data");
                return;
            }
            // If no data is currently being polled, update the schema, then
            // begin polling data.

            schema = SchemaUpdate();
            if (schema != "NOT FOUND")
            {
                peerStream.Flush();
                this.Invoke((MethodInvoker)delegate
                {
                    poll_status.Text = "Polling";
                });
                pollingData = true;
                this.Invoke((MethodInvoker)delegate
                {
                    pollData.Change(0, POLLTIME);
                });
                Form1.LogMessageToFile("event", "start_polling()", "Polling Loop began");

            }
            else
            {
                MessageBox.Show("Error: No Schema detected, need to update schema.");
                LogMessageToFile("error", "Start Click", "Schema file was empty.");
            }
        }

        /// <summary>
        /// Stop polling vehicle data. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stop_polling_button_Click(object sender, EventArgs e)
        {
            if (pollingData == false)
                MessageBox.Show("Currently not polling Data.");

            else
                stop_polling();
        }

        private void stop_polling()
        {


            // If data is still being polled, stop the process.  
            if (pollingData)
            {
                succeedCounter = 0;
                failCounter = 0;

                this.Invoke((MethodInvoker)delegate
                {
                    data_failed.Text = "0";
                });
                this.Invoke((MethodInvoker)delegate
                {
                    data_sent.Text = "0";
                });
                this.Invoke((MethodInvoker)delegate
                {
                    poll_status.Text = "Not Polling";
                });
                pollingData = false;
                pollData.Change(Timeout.Infinite, Timeout.Infinite);
                LogMessageToFile("event", "stop_polling()", "Polling was stopped");

                System.Threading.Thread.Sleep(2050);

            }
            else
                LogMessageToFile("error", "stop_polling()", "Attempted to stop polling when it was already stopped.");

        }

        /// <summary>
        /// Read the schema from the json file to the schema string for use. 
        /// </summary>
        /// <returns></returns>
        private string SchemaUpdate()
        {
            // Read the schema from the file.
            try
            {
                StreamReader reader = new StreamReader("schema.json");
                schema = reader.ReadToEnd();
                LogMessageToFile("event", "SchemaUpdate()", "Schema file has been updated");
            }
            catch (FileNotFoundException exception)
            {
                schema = "NOT FOUND";
                LogMessageToFile("error", "Schema Error", "Could not retrieve schema:" + exception);
            }
            return schema;
        }



        private void OnPowerChange(object s, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.Resume:

                    System.Threading.Thread.Sleep(10000);
                    LogMessageToFile("event", "Power Mode: Resume", "Resumed Power, closeing vConnect...");
                    Environment.Exit(2);

                    break;
                case Microsoft.Win32.PowerModes.Suspend:
                    if (pollingData)
                    {
                        try
                        {
                            if (BTConnection.CloseBTConnection())
                                LogMessageToFile("event", "Power Mode: Suspend", "Closed BT Connection and Stopped Polling");

                            stop_polling();
                        }
                        catch
                        {
                            LogMessageToFile("error", "Power Mode: Suspend", "ERROR:" + e);
                        }
                        finally
                        {
                            pollingData = true;
                        }
                    }
                    LogMessageToFile("event", "PowerMode", "Suspending");
                    break;
            }
        }
        /// <summary>
        /// This function serves as the launching point for requesting data. Note that 
        /// this function is being used as a time callback method for a Threading Timer object,
        /// so it is being called repeatedly, as long as vConnect is polling for Data.
        /// 
        /// If there is a BT Connection, do the following:
        /// The format method of retrieving and sending data is as follows:
        ///     
        ///     1. Create a list of DataElement objects.
        ///     
        ///     2. For each element in the list, request the car for the associated data,
        ///         to be stored in the DataElement.valueToSend variable.
        ///         
        ///     3. Create a dictionary of string-object pairs containing the DataElement.name 
        ///         (as the string) and its associated DataElement.valueToSend value.
        ///         The object value will be a string, integer, or other depending on the
        ///         DataElement.type value.
        ///     
        ///     4. Add the dictionary to the List of Dictionaries in the cache. Eventually, the
        ///         cache will convert the List of Dictionaries (each containing name,value of
        ///         data elements) to an array of JSON objects the JsonConvert.SerializeObject()
        ///         method from Json.net library. 
        ///         
        ///     5. Request OBDII Error Codes from the OBDII module, and send them to the server
        ///        if any are detected.
        ///     
        ///     6. Send all vehicle information stored in the cache into the server.
        ///     
        /// The details regarding how this function handles various failures are detailed in 
        /// comments in the function itself.
        ///     
        /// If there is no BT Connection:s
        ///     
        ///     1. Increment the fail counter 
        ///     
        ///     2. Mark the device label as diconnected if it is not so already.
        ///     
        ///     3. Check if vConnect has connection with the server, and edit the server connection
        ///        label as needed.
        ///        
        ///     4. Attempt to reconnect with the registered OBDII module.
        /// 
        ///         
        /// </summary>
        public void RequestDataForElements(object sender)
        {

            // Try block in case of any unexpected exceptions.
            try
            {
                // Check if the OBDII module is connected.
                if (BTConnection.Client.Connected)
                {
                    // If so, perform an integrity check to make sure that 
                    // the OBDII module is not sending corrupted data.
                    if (!BTConnection.IntegrityCheck())
                    {
                        // Close the connection is corrupted data is detected,
                        // and update the UI accordingly.
                        BTConnection.CloseBTConnection();
                        this.Invoke((MethodInvoker)delegate
                        {
                            device_Status_Label.Text = "Disconnected";
                        });
                    }
                }

                // If OBDII module still connected, continue with requesting for data.
                if (BTConnection.Client.Connected)
                {
                    // Mark that vConnect is currently polling data.
                    // NOTE that polling data can be changed to false via the UI,
                    // hence all the checks to see if pollingData is false.
                    pollingData = true;
                    // Create a list of DataElements.
                    List<DataElement> elemList = new List<DataElement>();

                    // Create a dictionary of string-object pairs. This will contain the key-value pairs
                    //  to be sent to the server.
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();

                    // Create the "shell" of empty elements from the schema.
                    elemList = CreateElementsFromSchema(schema);
        
                    // return if no longer polling data.
                    if (!pollingData)
                        return;


                    // Fill the contents of the elements with the data from the car
                    elemList = GetElementData(elemList);

                    // If element list returned null, and polling data is false, return.
                    if (elemList == null && !pollingData)
                        return;

                    // If just elemList is equal to null, then something failed in getting
                    // element data. Increment the fail counter, update the UI, and return.
                    else if (elemList == null)
                    {
                        failCounter = failCounter + 1;
                        this.Invoke((MethodInvoker)delegate
                        {
                            data_failed.Text = failCounter.ToString();
                        });
                        return;
                    }

                    // Create a dictionary out of the list of elements.
                    dictionary = CreateDictionary(elemList);

                    // Add the dictionary containing the data points to the cache.
                    cache.AddElementToCache(dictionary);

                    // If no longer polling data, return.
                    if (!pollingData)
                        return;

                    // Attempt to check for error codes. If it returns false,
                    // some error occurred with checking for error codes, in 
                    // which case increment the fail counter.
                    if (CheckForErrorCodes(elemList[1].ValueToSend) == false)
                    {
                        failCounter = failCounter + 1;
                        this.Invoke((MethodInvoker)delegate
                        {
                            data_failed.Text = failCounter.ToString();
                        });
                        return;
                    }

                    // If error codes were sent correctly, or if there were no 
                    // error codes, increment the succeedCounter.
                    else
                    {
                        succeedCounter = succeedCounter + 1;
                        this.Invoke((MethodInvoker)delegate
                        {
                            data_sent.Text = succeedCounter.ToString();
                        });
                    }

                    // Attempt to send the data points stored in the 
                    // cache to the server. If it succeeds, increment
                    // the succeed Counter, and update the UI in regards
                    // to the server status.
                    if (cache.SendToServer(cache.JsonString, "data"))
                    {
                        succeedCounter = succeedCounter + 1;
                        this.Invoke((MethodInvoker)delegate
                        {
                            data_sent.Text = succeedCounter.ToString();
                        });
                        this.Invoke((MethodInvoker)delegate
                        {
                            server_status.Text = "Connected";
                        });

                    }

                    // If sending codes to the server fails, increment
                    // the fail counter, update the UI in regards to the
                    // server connection status, and return.
                    else
                    {
                        failCounter = failCounter + 1;
                        this.Invoke((MethodInvoker)delegate
                        {
                            data_failed.Text = failCounter.ToString();
                        });

                        this.Invoke((MethodInvoker)delegate
                        {
                            server_status.Text = "Not Connected";
                        });
                        return;

                    }
                }
                
                
                // Else, if the OBDII module is disconnected, do the following.
                else
                {
                    // Update to device status UI label.
                    this.Invoke((MethodInvoker)delegate
                    {
                        device_Status_Label.Text = "Disconnected";
                    });

                    // Increment the fail counter.
                    failCounter = failCounter + 1;
                    this.Invoke((MethodInvoker)delegate
                    {
                        data_failed.Text = failCounter.ToString();
                    });
                   
                    // Check if vConnect can still connect with the server,
                    // whatever the result, update the server status UI label
                    // accordingly.
                    if (cache.CheckServerConnection())
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            server_status.Text = "Connected";
                        });

                    }
                    else
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            server_status.Text = "Disconnected";
                        });

                    }

                    
                    // Try to close the stream if it is open.
                    try { peerStream.Close(); }
                    catch { ;}

                    // If the OBDII module regained partial (faulty) connection
                    // with vConnect, dispose of the connection.
                    if (BTConnection.Client.Connected)
                        BTConnection.Client.Dispose();

                    // Attempt to re-establish BT connection with the OBDII module.
                    // If successfully, update the UI and log.
                    if (BTConnection.EstablishBTConnection())
                    {
                        Form1.LogMessageToFile("event", "Polling Loop", "BT Connection Reestablished in Polling Loop");
                        this.Invoke((MethodInvoker)delegate
                        {
                            device_Status_Label.Text = "Connected";
                        });
                        this.Invoke((MethodInvoker)delegate
                        {
                            BTID_label.Text = BTConnection.DeviceID;
                        });
                    }
                    return;
                }
            }
            
            // Catch the error and return. vConnect should be able to resolve the issue itself. 
            catch
            {
                return;
            }
        }



        /// <summary>
        /// This function creates a list of DataElement objects from the schema.
        /// </summary>
        /// <param name="schema">string containing the JSON schema</param>
        /// <returns>List of DataElement objects</returns>
        private List<DataElement> CreateElementsFromSchema(string schema)
        {
            // Create a list of the objects contained in the JSON file, and parse it
            //  using JSON.NET
            JObject jsonObjectList = JObject.Parse(schema);

            // Create empty list of DataElement objects
            List<DataElement> elementList = new List<DataElement>();

            // Temporary variables to hold data before object creation.
            string name = "", mode = "", code = "", size = "", type = "", equation = "";

            // For each item in the schema (e.g. VIN, timestamp, speed, etc.),
            //  get all of the values associated with it.
            foreach (var pair in jsonObjectList)
            {
                // The name of the element is the key (by definition)
                // Use it as the key for the JsonObject list.
                name = pair.Key;

                // Get the "mode" from the schema.
                // This refers to the MODE on which the OBDII module will ask
                //  the car for data. Usually 01 or 09.
                if (jsonObjectList[name]["mode"] == null)
                    mode = "n/a";
                else
                    mode = (string)jsonObjectList[name]["mode"];

                // Get the "code" from the schema.
                // This refers to the OBD PID with which the element can retrieve
                //  its information from the car. 
                if (jsonObjectList[name]["code"] == null)
                    code = "n/a";
                else
                    code = (string)jsonObjectList[name]["code"];

                // Get the "size" of the associated data element.
                // This refers to the number of bytes the car will return when 
                //  polled for data. (Usually 1-4)
                if (jsonObjectList[name]["size"] == null)
                    size = "0";
                else
                    size = (string)jsonObjectList[name]["size"];

                // Get the "datatype" for the data element.
                // This is used for knowing which format to send the data to 
                //  the server (string, int, etc.)
                if (jsonObjectList[name]["type"] == null)
                    type = "n/a";
                else
                    type = (string)jsonObjectList[name]["type"];

                // Get the "equation" for the data element.
                // This is used to parse the return value from the car into 
                //  human-readable data. (For example, equation="2A+B" would
                //  signal the FormatData function to take the first byte (A),
                //  multiply it by two, and add the second byte (B) to it. This
                //  will transform the car-returned HEX values into its real value.
                if (jsonObjectList[name]["equation"] == null)
                    equation = "n/a";
                else
                    equation = (string)jsonObjectList[name]["equation"];

                // Add a DataElement to the list containing the values of the parsed object.
                elementList.Add(new DataElement(name, mode, code, type, Int32.Parse(size), equation, getBTConnection()));

            }

            // Return the list of DataElements
            return elementList;
        }

        /// <summary>
        /// This function gets the associated datapoints for each of the DataElements
        /// in the list. It should be passed the list of Data Elements returned from CreateElementsFromSchema
        /// </summary>
        /// <param name="elemList">List of DataElement objects that have their size, equation, type, etc. filled in.</param>
        /// <returns>Returns the same list of DataElement objects but with the valueToReturn variable filled.</returns>
        private List<DataElement> GetElementData(List<DataElement> elemList)
        {
            // For each element in the list, if the element is not for TIME, 
            //  get data from the car and format it.
            foreach (DataElement elem in elemList)
            {
                if (elem.DataType == "date")
                {
                    elem.ValueToSend = DateTime.Now.ToString();
                }
                else
                {
                    if (!pollingData)
                        return elemList;
                    // Get data from the car for the element and format it.
                    if (!elem.RequestDataFromCar())
                    {
                        //  if (!BTConnection.ConnectionStatus)
                        //     stop_polling();

                        return null;
                    }
                    elem.FormatData();
                }
            }

            // Return the same list of DataElement objects.
            return elemList;
        }


        /// <summary>
        /// This function creates a dictionary out of the list of DataElement objects passsed to it.
        ///     It uses objects for the value since the datatype will vary based on the type of the
        ///     element (string, integer, etc.)
        /// </summary>
        /// <param name="elemList">List of DataElement objects returned from GetElementData()</param>
        /// <returns>Dictionary containing the data points to send to the server.</returns>
        private Dictionary<string, object> CreateDictionary(List<DataElement> elemList)
        {
            // Create an empty dictionary with a string-object key-value pair.
            var elementDictionary = new Dictionary<string, object>();

            // Loop through each element in the list, and 
            foreach (DataElement elem in elemList)
            {
                if (elem.ValueToSend != "Not supported")
                {

                    // If the datatype is a number, send the value as an integer
                    if (elem.DataType == "number")
                        elementDictionary.Add(elem.Name, elem.ValueToSend);

                   // If the datatype is a date, send it as a String?
                    else if (elem.DataType == "date")
                        elementDictionary.Add(elem.Name, elem.ValueToSend);

                    // IF the datatype is a date, just send it as a string.
                    else if (elem.DataType == "string")
                        elementDictionary.Add(elem.Name, elem.ValueToSend);
                }
            }

            // Return the dictionary containing the name of each element and its
            //  associated value (be it a string, integer, etc.)
            return elementDictionary;
        }

        /// <summary>
        /// Requests the vehicles error codes (check engine PIDs) from the OBDII module.
        /// If error codes are returned, parse out the actual error codes from the return byte
        /// array using the ParseErrorCode function. Once all error codes have been parsed, send
        /// the error codes to the server. If no error codes are detected, do not send any message
        /// to ther server.
        /// </summary>
        /// <param name="VIN"> 
        /// VIN of the car that is being checked for error codes. VIN
        /// is needed to send error codes to the server.</param>
        /// <returns>
        /// True if no error codes are detected, or if error codes are detected, correctly
        /// parsed, and then successfully sent to the server.
        /// False if vConnect has stopped polling data, BT Connection is lost with the OBDII module,
        /// the parser function fails,
        /// </returns>
        private bool CheckForErrorCodes(string VIN)
        {


            byte[] errorCode = new byte[200];   // Byte array to store error code. 

            string errorString = "";            // String to hold error code after being parsed 
            // from byte array.

            // Return false if vConnect polling has ceased.
            if (!pollingData)
                return false;


            // Try to request and receive error codes.
            try
            {
                // Create the code request
                byte[] writeCode = System.Text.Encoding.ASCII.GetBytes("03 \r");

                // Write code to OBDII module
                peerStream.Flush();
                peerStream.Write(writeCode, 0, writeCode.Length);

                // Pause to let OBDII module process request.
                System.Threading.Thread.Sleep(2000);

                // Read OBDII error codes. 
                peerStream.Read(errorCode, 0, errorCode.Length);
            }


            // Catch exception for losing BT connection with OBDII module, log the error
            // and return false.
            catch (Exception ex)
            {
                LogMessageToFile("error", "CheckForErrorCodes()", ex.ToString());
                return false;
            }

            // Return false if vConnect has ceased polling data.
            if (!pollingData)
                return false;

            byte[] subErrorCode = new byte[4];  // Array to hold each error code.
            int counter = 2;                    // Counter used to keep track of location in errorCode array.
            bool loopExit = false;              // Bool for loop value.
            string toSend = "";                 // String that will store the message with error codes to 
                                                // be sent to the server

            // String that has the actual string value of the bytes in errorCode.
            string hexLiteral = System.Text.Encoding.ASCII.GetString(errorCode);
            
            
            // If the errorCode byte array contains 4300\r4300, then it has no error codes.
            if (!hexLiteral.Contains("4300\r4300"))
            {
                bool firstError = true;
                while (!loopExit)
                {
                    // Get the error code from errorCode.
                    subErrorCode = errorCode.Skip(counter).Take(4).ToArray();

                    // Parse the error code byte array.
                    errorString = parseErrorCode(subErrorCode);


                    // If the error code was all 0's (signifying that there are no more
                    // error codes), exit the loop.
                    if (errorString == "P0000")
                        loopExit = true;

                    // If errorString = -1, then ParseErrorCode encountered an error due to 
                    // corrupted data from the OBDII module.
                    else if (errorString == "-1")
                        return false;

                    // Format the errorCode in a Json Message.
                    else
                    {
                        // If its the first error Code read, format the first part of the error Code message to ther server.
                        if (firstError)
                        {
                            toSend = toSend + "{\"VIN\":\"" + VIN + "\",\"timestamp\":\"" + DateTime.Now.ToString()
                                + "\",\"trouble_code\":\"" + errorString + "\"}";
                            firstError = false;
                        }
                        // If it it not the first error code read, then add this error code to the message to be sent
                        // to the server.
                        else
                            toSend = toSend + ",{\"VIN\":\"" + VIN + "\",\"timestamp\":\"" + DateTime.Now.ToString()
                                + "\",\"trouble_code\":\"" + errorString + "\"}";
                    }
                    // Move the counter forward in the Byte array.
                    counter += 4;

                    // If the next two characters in the array are newline characters, there are no more error codes, so exit the loop.
                    if (System.Text.Encoding.ASCII.GetString(errorCode, counter, 1) == "\r" && System.Text.Encoding.ASCII.GetString(errorCode, 1 + counter, 1) == "\r")
                        loopExit = true;
                    // If not, there are more error codes, so increment the counter by 3 to pass the newline character and two placeholder bytes.
                    else if (System.Text.Encoding.ASCII.GetString(errorCode, counter, 1) == "\r")
                        counter += 3;

                }

                // Enclose the message with brackets to be sent to the server.
                toSend = "[" + toSend + "]";

                // Log the error codes to the log.
                LogMessageToFile("event", "CheckForErrorCodes()", "Error codes polled: " + toSend);


                // Attempt to send the error codes to the server. If Sending is successful, update the
                // UI to reflect this and return true, if sending failed, update the UI to signify vConnect has lost 
                // server connectivity, and return false.
                if (!cache.SendToServer(toSend, "alert"))
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        server_status.Text = "Disconnected";
                    });
                    return false;
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        server_status.Text = "Connected";
                    });
                    return true;
                }
            }

            // If no error codes, log to the event log.
            else
            {
                LogMessageToFile("event", "CheckForErrorCodes()", "No error codes");
                return true;
            }

        }


        /// <summary>
        /// Receives a binary encoded errorCode as an input, and parses the bits out
        /// in order to get the actual error code to send to the server.
        /// For more information concerning the pattern in which the bits are parsed,
        /// visit http://en.wikipedia.org/wiki/OBD-II_PIDs , and go view the Mode 3 
        /// section.
        /// </summary>
        /// <param name="errorCode">
        /// The binary encoded error code to be parsed.
        /// </param>
        /// <returns>
        /// If the error code is parsed correctly, then return that error code string.
        /// If some error occured while parsing the error code, return -1.
        /// </returns>
        private string parseErrorCode(byte[] errorCode)
        {

            string errorString = "";    // String that will hold the actual error code.

            // Strings that will hold parts of the error code.
            string DTC1 = "";
            string DTC2 = "";
            string DTC3 = "";
            string DTC4 = "";
            string DTC5 = "";

            try
            {
                // Retrieves the 3rd and 4th least significant bits of the first byte,
                // and selects a value based on the bits integer values.
                int DTC1Check = (errorCode[0] >> 2) & 0x3;

                if (DTC1Check == 0)
                    DTC1 = "P";
                else if (DTC1Check == 1)
                    DTC1 = "C";
                else if (DTC1Check == 2)
                    DTC1 = "B";
                else if (DTC1Check == 3)
                    DTC1 = "U";

                // Retrieves the 1st and 2nd least significant bits of the first byte,
                // and get their integer value.
                int DTC2Check = errorCode[0] & 0x3;
                DTC2 = DTC2Check.ToString();

                // Retrieves the 1-4 least significant bits of the second byte,
                // and get their integer value.
                int DTC3Check = errorCode[1] & 0xF;
                DTC3 = DTC3Check.ToString();

                // Retrieves the 1-4 least significant bits of the third byte,
                // and get their integer value.
                int DTC4Check = errorCode[2] & 0xF;
                DTC4 = DTC4Check.ToString();
                // Retrieves the 1-4 least significant bits of the fourth byte,
                // and get their integer value.
                int DTC5Check = errorCode[3] & 0xF;
                DTC5 = DTC5Check.ToString();

                // Connects all the strings into the correct error code to be
                // sent to the server.
                errorString = DTC1 + DTC2 + DTC3 + DTC4 + DTC5;
            }

            // Received unexpected values and could not parse correctly,
            // return -1 to signify failure.
            catch (Exception e)
            {
                LogMessageToFile("error", "ParseErrorCode()", "Bad Parse" + e);
                return "-1";
            }

            // Return the error code.
            return errorString;
        }


        /// <summary>
        ///  This handles a simple input dialog box. Taken from 
        ///     http://www.csharp-examples.net/inputbox/
        /// </summary>
        /// <param name="title"></param>
        /// <param name="promptText"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;

        }

        /// <summary>
        /// Returns the BT Connection.
        /// </summary>
        /// <returns></returns>
        public BluetoothConnectionHandler getBTConnection()
        {
            return BTConnection;
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }



        /// <summary>
        /// This static function allows for writing errors to a standard log file
        ///     from any of the classes. It writes the error in the format:
        ///     
        ///     3/1/2015 6:27:22 PM: [caller] message.
        ///     
        ///     where   type    ->  whether logs to event or error log
        ///             caller  ->  where the error occurred and 
        ///             message ->  the error message to write.
        ///     
        /// </summary>
        /// <param name="caller">This term gives anyone reading the log file
        ///                         a way to know from where the error came. </param>
        /// <param name="message">The error message to write to the file</param>
        public static void LogMessageToFile(string type, string caller, string message)
        {
            string FILE_TYPE = "";

            // File name of the error log
            if (type == "error")
                FILE_TYPE = "error.log";
            else if (type == "event")
                FILE_TYPE = "event.log";
            else
            {
                LogMessageToFile("error", "LogMessageToFile", "ERROR: Invalid log file type used.");
                return;
            }

            // Append to the feild.
            StreamWriter writer = File.AppendText(FILE_TYPE);

            try
            {
                // Write according to "3/1/2015 6:27:22 PM: [caller] message." format
                string logLine = String.Format(
                    "{0:G}: [{1}] {2}.\n\n", System.DateTime.Now, caller, message);
                writer.WriteLine(logLine);
            }
            finally
            {
                // Close the stream
                writer.Close();
            }
        }

        private void Set_PIN_Button_Click(object sender, EventArgs e)
        {
            if (pollingData)
                MessageBox.Show("Must stop polling before changing PIN");
            else
            {
                string value = null;
                if (InputBox("New PIN", "New PIN:", ref value) == DialogResult.OK)
                {
                    try
                    {
                        int test = Convert.ToInt32(value);
                        BTConnection.PIN = value;
                        Properties.Settings.Default.PIN = value;
                        Properties.Settings.Default.Save();
                    }
                    catch
                    {
                        MessageBox.Show("Must insert numerical value for PIN.");

                    }
                }
            }
        }

        private void update_schema_button_Click_1(object sender, EventArgs e)
        {
            if (pollingData)
                MessageBox.Show("Must stop polling before updating schema");
            else
            {
                try
                {
                    // Address to request schema from.
                    string address = "http://vconnect-danieladams456.rhcloud.com/data/schema";
                    // Initialize the connection the address above.
                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead(address);
                    StreamReader reader = new StreamReader(stream);
                    String json = reader.ReadToEnd();
                    File.WriteAllText("schema.json", json);
                }
                catch
                {
                    MessageBox.Show("ERROR: Could not retrieve schema.");
                    LogMessageToFile("error", "Schema Retrieval", "Could not retrieve Schema from server.");
                }
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}