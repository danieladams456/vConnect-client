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


namespace vConnect
{


    public partial class Form1 : Form
    {
        // Instantiates objects from the classes BluetoothConnectionHandler, ServerConnectionHandler,
        // and a null object of DataCache.
        BluetoothConnectionHandler BTConnection = new BluetoothConnectionHandler();
        ServerConnectionHandler serverConnection = new ServerConnectionHandler();
        DataCache cache = null;

        // Instantiates a timer used to poll data from the OBDII module.
        static public System.Threading.Timer pollData;


        // List that will used to hold OBDII codes before being inserted into the cache.
        List<Dictionary<string, object>> elementDictionaryList =
                                                new List<Dictionary<string, object>>();

        // String that will hold the current schema.
        String schema = "";

        // Bool value specifying whether the data polling asychronous operation is currently
        // running or not. 
        static public bool pollingData = false;
        static public Stream peerStream;


        // Constant that determines how often the data polling Timer will run. (In miliseconds)
        const int POLLTIME = 60000;



        // Timer CallBack to be used for polling data.
        TimerCallback tcb;

        public Form1()
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;

            // Initialize the dataCache.
            cache = new DataCache(serverConnection);

            // Create a Timer callback method for polling data. 
            tcb = RequestDataForElements;

            // Bool variable to record whether an OBDII device and server have been successfully detected and pinged/established connection. 
            bool deviceDetect = false;
            bool serverDetect = false;


            BTConnection.PIN = Properties.Settings.Default.PIN;
            
            // If there is a saved BT Address, attempt to connect with the device with that address.
            if (Properties.Settings.Default.BTAddress != "" && Properties.Settings.Default.PIN != "")
            {
                // Grabs the saved BT address from the settings file.
                BTConnection.BluetoothAddress = BluetoothAddress.Parse(Properties.Settings.Default.BTAddress);

                // If connection is established with the device with the specified BT address above,
                // save the Device's ID, indicated connection status on the GUI.
                if (BTConnection.EstablishBTConnection())
                {
                    BTConnection.DeviceID = Properties.Settings.Default.BTDeviceName;
                    BT_ID.Text = Properties.Settings.Default.BTDeviceName;
                    device_Status_Label.Text = "Connected";
                    MessageBox.Show("Auto BT Connect!");
                    deviceDetect = true;
                }
                else
                {
                    Properties.Settings.Default.BTAddress = null;
                    Properties.Settings.Default.BTDeviceName = null;
                    Properties.Settings.Default.Save();
                }
            }

            // If no connection was established with the device with the saved BT Address, then
            // check all detectable BT devices with a Device Name corresponding with an OBDII module,
            // and attempt to connect with them. 
         /*   else
            {
                // Array of all detected BT devices.
                BluetoothDeviceInfo[] peers = BTConnection.Client.DiscoverDevices();


                int peerCounter = 0;

                // Loops through all of the BT Devices detected, and attempt to connect with any one who's device ID
                // indicates that it is an OBDII module.

                while (peerCounter < peers.Length)
                {

                    if (peers[peerCounter].DeviceName == "CBT." || peers[peerCounter].DeviceName == "OBDII")
                    {
                        // Retrieve the BT adress from the BT device whose device name indicates that it is 
                        // as OBDII module.
                        BTConnection.BluetoothAddress = peers[peerCounter].DeviceAddress;
                        BTConnection.DeviceID = peers[peerCounter].DeviceName;

                        // If connection is established, save the OBDII device's BT Address and Device Name to the 
                        // settings file, and change details on the GUI accordingly. 
                        if (BTConnection.EstablishBTConnection())
                        {
                            BTConnection.DeviceInfo = peers[peerCounter];
                            BT_ID.Text = peers[peerCounter].DeviceName;
                            BTConnection.DeviceID = peers[peerCounter].DeviceName;
                            Properties.Settings.Default.BTAddress = peers[peerCounter].DeviceAddress.ToString();
                            Properties.Settings.Default.BTDeviceName = peers[peerCounter].DeviceName;
                            Properties.Settings.Default.Save();
                            device_Status_Label.Text = "Connected";
                            peerCounter = peers.Length;
                            deviceDetect = true;
                        }
                    }
                    peerCounter++;
                }

            }
            */
            // If no OBDII connection is established, then print to the screen stating this fact, and then
            // load the GUI. 
            if (deviceDetect == false)
            {
                Properties.Settings.Default.BTAddress = null;
                Properties.Settings.Default.BTDeviceName = null;
                Properties.Settings.Default.Save();
                BT_ID.Text = "N/A";
                BTConnection.DeviceID = null;
                BTConnection.DeviceInfo = null;
                device_Status_Label.Text = "Disconnected";
            }

            // Checks if any server connection data is saved in the settings file. If so, attempts to see if connection
            // can be established.
            if (Properties.Settings.Default.ServerIP != "" && Properties.Settings.Default.ServerPort != "")
            {
                serverConnection.PortNumber = Convert.ToInt32(Properties.Settings.Default.ServerPort);

                serverConnection.IPAddress = Properties.Settings.Default.ServerIP;

                // If the server is available, switch the bool value to save that info. 
                if (serverConnection.CheckServerConnection())
                {
                    serverDetect = true;
                    MessageBox.Show("Auto server connect!");
                    port_number.Text = serverConnection.PortNumber.ToString();
                    server_IP.Text = serverConnection.IPAddress;
                    serverConnection.ServerConnectionStatus = true;
                }
                else
                {
                    MessageBox.Show("ERROR: Could not connect to server at saved IP address and port number.");
                    Properties.Settings.Default.ServerIP = null;
                    server_IP.Text = "N/A";
                    serverConnection.IPAddress = null;

                    Properties.Settings.Default.ServerPort = null;
                    port_number.Text = "0";
                    serverConnection.PortNumber = 0;
                    Properties.Settings.Default.Save();
                }
            }
            else
                MessageBox.Show("No server connection data was found, please add server IP address and port number");

            // If connections have been established to the OBDII device and the server, then begin polling for 
            // vehicle data. 
            if (deviceDetect && serverDetect)
            {
                MessageBox.Show("Beginning auto poll now.");
                schema = SchemaUpdate();
                if (schema != "NOT FOUND")
                    pollData = new System.Threading.Timer(tcb, null, 0, POLLTIME);
                else
                {
                    MessageBox.Show("Error: No Schema detected, need to update schema.");
                    LogMessageToFile("Start Click", "Schema file was empty.");

                }
            }
            // If connections have not been established to the OBDII device and server, then initialize the loop to 
            // poll data, but do not start it. 
            else
                pollData = new System.Threading.Timer(tcb, null, Timeout.Infinite, Timeout.Infinite);
        }

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
                LogMessageToFile("Error Log", "The error log could not be found/opened.");
            }
        }

        /// <summary>
        ///  Allows the user to enter a new IP address using the GUI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void edit_IP_button_Click(object sender, EventArgs e)
        {
            if (pollingData)
            {
                MessageBox.Show("Stop Polling before Changing IP Address.");
            }
            else
            {
                string value = "IP Address";

                // Saves IP address to the settings file, as well as the server connection handler. 
                if (InputBox("New IP Address", "New IP Address:", ref value) == DialogResult.OK)
                {
                    serverConnection.IPAddress = value;

                    server_IP.Text = value;
                    Properties.Settings.Default.ServerIP = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        /// <summary>
        ///  Allows the user to enter a new port number using the GUI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void edit_port_button_Click(object sender, EventArgs e)
        {
            if (pollingData)
            {
                MessageBox.Show("Stop Polling data before changing the port number.");
            }
            else
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
                            serverConnection.PortNumber = Int32.Parse(value);

                            Properties.Settings.Default.ServerPort = value;
                            Properties.Settings.Default.Save();
                            port_number.Text = value;
                        }
                        else
                        {
                            MessageBox.Show("Invalid Port Number: Port number must be between 1 and 65534.");
                            LogMessageToFile("Port Number", "Invalid Port number.");

                        }

                    }

                    catch
                    {
                        MessageBox.Show("Invalid Port Number: Port number must be between 1 and 65534.");
                        LogMessageToFile("Port Number", "Invalid Port number.");

                    }
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
                if (BTConnection.ConnectionStatus)
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
                        BTConnection.DeviceID = device.DeviceName;
                        BTConnection.DeviceInfo = device;
                        BT_ID.Text = device.DeviceName;
                        Properties.Settings.Default.BTDeviceName = device.DeviceName;
                        Properties.Settings.Default.BTAddress = device.DeviceAddress.ToString();
                        Properties.Settings.Default.Save();
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
                if (!BTConnection.BTConnectionStatus)
                    MessageBox.Show("There are no OBDII devices connected.");
                else if (BTConnection.CloseBTConnection())
                    device_Status_Label.Text = "Disconnected";
            }

        }


        /// <summary>
        /// Update the Schema from the supplied web site, and store it in schema.json.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void update_schema_button_Click(object sender, EventArgs e)
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
                MessageBox.Show(json);
                File.WriteAllText("schema.json", json);
            }
            catch
            {
                MessageBox.Show("ERROR: Could not retrieve schema.");
                LogMessageToFile("Schema Retrieval", "Could not retrieve Schema from server.");
            }
        }


        /// <summary>
        /// Start polling data from the vehicle, if able. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void start_button_Click(object sender, EventArgs e)
        {
            if (serverConnection.CheckServerConnection())
            {
                Properties.Settings.Default.ServerIP = serverConnection.IPAddress;
                Properties.Settings.Default.ServerPort = serverConnection.PortNumber.ToString();
                Properties.Settings.Default.Save();
            }
            // If data is already being polled, then nothing to do. 
            if (pollingData)
                MessageBox.Show("Already Polling Data");

            // If no data is currently being polled, update the schema, then
            // begin polling data.
            else if (BTConnection.BTConnectionStatus && serverConnection.CheckServerConnection())
            {
                schema = SchemaUpdate();
                if (schema != "NOT FOUND")
                {
                    peerStream.Flush();
                    pollData = new System.Threading.Timer(tcb, null, 0, POLLTIME);
                }
                else
                {
                    MessageBox.Show("Error: No Schema detected, need to update schema.");
                    LogMessageToFile("Start Click", "Schema file was empty.");

                }
            }
            else
            {
                if (!BTConnection.BTConnectionStatus && !serverConnection.ServerConnectionStatus)
                    MessageBox.Show("No connection to OBDII device or server, cannot start.");
                else if (!BTConnection.BTConnectionStatus)
                    MessageBox.Show("No connection to OBDII device, cannot start.");
                else if (!serverConnection.ServerConnectionStatus)
                    MessageBox.Show("No connection to server, cannot start.");
            }
        }


        /// <summary>
        /// Stop polling vehicle data. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stop_polling_button_Click(object sender, EventArgs e)
        {
            stop_polling();
        }
       
        static private void stop_polling()
        {
            // If no data is currently being polled, do nothing. 

            if (pollingData == false)
                MessageBox.Show("Currently not polling Data.");

           // If data is still being polled, stop the process.  
            else
            {
                pollingData = false;
                pollData.Change(Timeout.Infinite, Timeout.Infinite);
                System.Threading.Thread.Sleep(7000);
                pollData = null;
            }

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
            }
            catch (FileNotFoundException exception)
            {
                schema = "NOT FOUND";
                MessageBox.Show("Did not find the schema");
                LogMessageToFile("Schema Error", "Could not retrieve schema:" + exception);
            }
            return schema;
        }


        /// <summary>
        /// This function serves as the launching point for requesting data.
        /// 
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
        /// </summary>
        public void RequestDataForElements(object sender)
        {
            pollingData = true;
            MessageBox.Show("Starting.");
            // Create a list of DataElements.
            List<DataElement> elemList = new List<DataElement>();

            // Create a dictionary of string-object pairs. This will contain the key-value pairs
            //  to be sent to the server.
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            // Create the "shell" of empty elements from the schema.
            elemList = CreateElementsFromSchema(schema);

            // Fill the contents of the elements with the data from the car
            if (!pollingData)
                return;
            elemList = GetElementData(elemList);
            if (elemList == null)
                return;
            MessageBox.Show("Done Polling ELements");
            // Create a dictionary out of the list of elements.
            dictionary = CreateDictionary(elemList);

            // Add the dictionary containing the data points to the cache.
            cache.AddElementToCache(dictionary);
            if (!pollingData)
                return;
            CheckForErrorCodes(elemList);
            cache.SendToServer(cache.JsonString, "data");
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
                        if(!BTConnection.ConnectionStatus)
                            stop_polling();

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
                if (elem.ValueToSend == "Not supported")
                    ;

                // If the datatype is a number, send the value as an integer
                else if (elem.DataType == "number")
                    elementDictionary.Add(elem.Name, elem.ValueToSend);

                // If the datatype is a date, send it as a String?
                else if (elem.DataType == "date")
                    elementDictionary.Add(elem.Name, elem.ValueToSend);

                // IF the datatype is a date, just send it as a string.
                else if (elem.DataType == "string")
                    elementDictionary.Add(elem.Name, elem.ValueToSend);
            }

            // Return the dictionary containing the name of each element and its
            //  associated value (be it a string, integer, etc.)
            return elementDictionary;
        }

        /// <summary>
        /// Check for any error codes, and immediately send them to the
        /// server if detected. 
        /// </summary>
        private bool CheckForErrorCodes(List<DataElement> elemList)
        {
            byte[] errorCode = new byte[60];
            string errorString = "";
            bool exit = false;

            try
            {
                if (!pollingData)
                    return false;
                // encode message
                try
                {
                    byte[] writeCode = System.Text.Encoding.ASCII.GetBytes("03 \r");
                    peerStream.Flush();
                    peerStream.Write(writeCode, 0, writeCode.Length);
                    System.Threading.Thread.Sleep(5000);
                    peerStream.Read(errorCode, 0, errorCode.Length);
                    //peerStream.Close();
                }

                catch (Exception ex)
                {
                    if (BTConnection.EstablishBTConnection())
                        return CheckForErrorCodes(elemList);
                    else
                    {
                        var msg = "failed to re- connect to BT Device.  ";
                        MessageBox.Show(msg);
                        LogMessageToFile("Checking for error codes error", msg + ex);
                        stop_polling();
                        return false; 
                      //  exit = true;
                    }

                }
                if (!pollingData || exit)
                    return false;
                byte[] subErrorCode = new byte[5];
                int counter = 7;
                bool loopExit = false;
                string toSend = null;

                if (!System.Text.Encoding.ASCII.GetString(errorCode).Contains("NO DATA"))
                {



                    while (!loopExit)
                    {
                        subErrorCode = errorCode.Skip(counter).Take(5).ToArray();

                        errorString = parseErrorCode(subErrorCode);


                        if (errorString == "P0000")
                            loopExit = true;
                        else
                        {
                            toSend = "[{\"VIN\":\"" + elemList[1].ValueToSend + "\",\"timestamp\":\"" + DateTime.Now.ToString()
                                + "\",\"trouble_code\":\"" + errorString + "\"}]";

                            // cache.SendToServer(toSend, "alert");
                        }
                        counter += 6;
                        if ((counter - 3) % 22 == 0)
                        {
                            if (System.Text.Encoding.ASCII.GetString(errorCode, counter, 1) == "\r" && System.Text.Encoding.ASCII.GetString(errorCode, 1 + counter, 1) == "\r")
                                loopExit = true;
                            else
                                counter += 4;
                        }
                    }
                    cache.SendToServer(toSend, "alert");


                }
                else
                {
         /////           MessageBox.Show("No error codes.");
                    return false;
                }


            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot Check for Error Codes, no BT connection.");
                LogMessageToFile("Error code check", "Lost BT connection: " + e.Message);
                stop_polling();
                return false;
            }

            return true;
        }


        /// <summary>
        /// Parses the bits from the a two byte error Code received from the OBDII module
        /// into its actual error code representation. 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        private string parseErrorCode(byte[] errorCode)
        {
            string errorString = "";
            string DTC1 = "";
            string DTC2 = "";
            string DTC3 = "";
            string DTC4 = "";
            string DTC5 = "";


            // This is just using bytes based... 
            int DTC1Check = (errorCode[0] >> 2) & 0x3;

            if (DTC1Check == 0)
                DTC1 = "P";
            else if (DTC1Check == 1)
                DTC1 = "C";
            else if (DTC1Check == 2)
                DTC1 = "B";
            else if (DTC1Check == 3)
                DTC1 = "U";

            int DTC2Check = errorCode[0] & 0x3;
            DTC2 = DTC2Check.ToString();

            int DTC3Check = errorCode[1] & 0xF;
            DTC3 = DTC3Check.ToString();

            int DTC4Check = errorCode[3] & 0xF;
            DTC4 = DTC4Check.ToString();

            int DTC5Check = errorCode[4] & 0xF;
            DTC5 = DTC5Check.ToString();

            errorString = DTC1 + DTC2 + DTC3 + DTC4 + DTC5;

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
        ///     where   caller  ->  where the error occurred and 
        ///             message ->  the error message to write.
        ///     
        /// </summary>
        /// <param name="caller">This term gives anyone reading the log file
        ///                         a way to know from where the error came. </param>
        /// <param name="message">The error message to write to the file</param>
        public static void LogMessageToFile(string caller, string message)
        {
            // File name of the error log
            string ERROR_FILE = "error.log";

            // Append to the feild.
            StreamWriter writer = File.AppendText(ERROR_FILE);

            try
            {
                // Write according to "3/1/2015 6:27:22 PM: [caller] message." format
                string logLine = String.Format(
                    "{0:G}: [{1}] {2}.", System.DateTime.Now, caller, message);
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
                    LogMessageToFile("Schema Retrieval", "Could not retrieve Schema from server.");
                }
            }
        }
    }
}