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


namespace vConnect
{    

    
    public partial class Form1 : Form
    {

        // Construct Objects to handle BT connection, server connection, and the DataCache upon startup. 
        BluetoothConnectionHandler BTConnection = new BluetoothConnectionHandler();
        ServerConnectionHandler serverConnection = new ServerConnectionHandler();
        DataCache cache = null;
        
        // Asynchronous Timer that handling polling and formatting data from the OBDII module.
        System.Threading.Timer pollData;
        
        // List that will used to hold OBDII codes before being inserted into the cache.
        List<Dictionary<string,object>> elementDictionaryList = 
                                                new List<Dictionary<string,object>>();
        
        // String that will hold the current schema.
        String schema = "";
        // Bool value specifying whether the data polling asychronous operation is currently
        // running or not. 
        bool pollingData = false;

        // Constant that determines how often the data polling Timer will run. (In miliseconds)
        const int POLLTIME = 120000;
        
        public Form1()
        {
            InitializeComponent();

            // Initialize the dataCache.
            cache = new DataCache(serverConnection);
            
// Create a Timer callback method for polling data. 
            TimerCallback tcb = RequestDataForElements;


            // Bool variable to record whether an OBDII device and server have been successfully detected and pinged/established connection. 
            bool deviceDetect = false;
            bool serverDetect = false;

            // Current server for testing. 
            serverConnection.PortNumber = 80;
            serverConnection.IPAddress = "127.7.19.130";
            
            // If there is a saved BT Address, attempt to connect with the device with that address.
            if (Properties.Settings.Default.BTAddress != "")
            {          
                BTConnection.BluetoothAddress = BluetoothAddress.Parse(Properties.Settings.Default.BTAddress);
                if(BTConnection.EstablishBTConnection())
                {
                    BT_ID.Text = Properties.Settings.Default.BTDeviceName;
                    device_Status_Label.Text = "Connected";
                    deviceDetect = true;
                }
            }
            // If no connection was established with the device with the saved BT Address, then
            // check all detectable BT devices with a Device Name corresponding with an OBDII module,
            // and attempt to connect with them. 
            else
            {   
            // Array of all detected BT devices. 
            BluetoothDeviceInfo[] peers = BTConnection.Client.DiscoverDevices();
            int peerCounter = 0;
            while (peerCounter < peers.Length)
            {
 
                if (peers[peerCounter].DeviceName == "CBT." || peers[peerCounter].DeviceName == "OBDII")
                    {
                        BTConnection.BluetoothAddress = peers[peerCounter].DeviceAddress;

                        // If connection is established, save the OBDII device's BT Address and Device Name to the 
                        // settings file, and change details on the GUI accordingly. 
                        if (BTConnection.EstablishBTConnection())
                        {
                            Properties.Settings.Default.BTAddress = BTConnection.BluetoothAddress.ToString();
                            Properties.Settings.Default.BTDeviceName = peers[peerCounter].DeviceName;
                            Properties.Settings.Default.Save();
                            BT_ID.Text = peers[peerCounter].DeviceName;
                            BTConnection.DeviceID = peers[peerCounter].DeviceName;
                            device_Status_Label.Text = "Connected";
                            peerCounter = peers.Length;
                            deviceDetect = true;
                        }

                    }
                    peerCounter++;
                }
            }

           // If no OBDII connection is established, then print to the screen stating this fact, and then
           // load the GUI. 
           if (deviceDetect == false)
                MessageBox.Show("No OBDII devices were connected to automatically.");
                // set Cursor to red. 

            // Checks if any server connection data is saved in the settings file. If so, attempts to see if connection
            // can be established.
            if (Properties.Settings.Default.ServerIP != "" && Properties.Settings.Default.ServerPort != "")
            {
                string portValue = Properties.Settings.Default.ServerPort;
                port_number.Text = portValue;
                serverConnection.PortNumber = Int32.Parse(portValue);

                string IPvalue = Properties.Settings.Default.ServerIP;                
                server_IP.Text = IPvalue;
                serverConnection.IPAddress = IPvalue;                

                if (serverConnection.CheckServerConnection())
                    serverDetect = true;
                    //Connect as well? 
                else
                {
                    MessageBox.Show("ERROR: Could not connect to server at saved IP address and port number.");
                    //server_status_label.Text = "Disconnected";
                }
            }
            else
                MessageBox.Show("No server connection data was found, please add server IP address and port number");
            

            
           // If connections have been established to the OBDII device and the server, then begin polling for 
           // vehicle data. 
            if (deviceDetect && serverDetect)
            {
                schema = SchemaUpdate();
                pollData = new System.Threading.Timer(tcb, null, 0, POLLTIME);
            }
            else
                pollData = new System.Threading.Timer(tcb, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        ///     Exits the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close_button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// This button will display a help box detailing the GUI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void help_button_Click(object sender, EventArgs e)
        {
            string helpMessage = "This GUI is used to setup and manange vConnect's Windows Aplication. \n " +
                   "Please note that this GUI cannot manage the server, or search for stored data.\n" +
                   "Listed here are the details concerning the various attributes of this GUI:\n" +
                   "BT Device ID: The ID of the OBDII Device that is currently assigned to vConnect.\n" +
                   "Device Status: Whether the OBDII Device listed above is connected or disconnect. \n" +
                   "Connect to OBDII Device: Will open up a dialog that shows all detectable BT Devices, " +
                   "selecting a device will attempt to connect with it. \n" +
                   "Disconnect BT Device: Will disconnect to the OBDII device (if one is connected).\n" +
                   "Server IP Address: The IP address that is assigned to vConnect, the edit button will " +
                   "alter this value.\n" +
                   "Server Port Number: The port number that is assigned to vConnect, the edit button will " +
                   "alter this value.\n" +
                   "Server Status: Whether the vConnect is currently connected with the server with the IP address " +
                   " and port number assigned to vConnect.\n" +
                   "Update Schema: Will query the vConnect server, and update the schema if it is out of data.\n" +
                   "Start: Will begin polling for data. \n" +
                   "Stop: Will stop polling for data.";

            MessageBox.Show(helpMessage);
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
                server_IP.Text = value;
                Properties.Settings.Default.ServerIP = value;
                Properties.Settings.Default.Save();
                // Should probably validate IP address here... 

                serverConnection.IPAddress = value;
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
                if (Int32.Parse(value) > 0 && Int32.Parse(value) < 65535)
                {
                    Properties.Settings.Default.ServerPort = value;
                    Properties.Settings.Default.Save();
                    port_number.Text = value;
                    serverConnection.PortNumber = Int32.Parse(value);
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

                // If connection is successfully esablished, save the device's name and address to ther 
                // settings file, and update the device status on the GUI to "connected.
                if (BTConnection.EstablishBTConnection())
                {
                    device_Status_Label.Text = "Connected";
                    BTConnection.DeviceID = device.DeviceName;
                    label5.Text = device.DeviceName;
                    BT_ID.Text = device.DeviceName;
                    Properties.Settings.Default.BTDeviceName = device.DeviceName;
                    Properties.Settings.Default.BTAddress = device.DeviceAddress.ToString();
                    Properties.Settings.Default.Save();
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
            if (BTConnection.CloseBTConnection())
                device_Status_Label.Text = "Disconnected";

        }


        /// <summary>
        /// Update the Schema from the supplied web site, and store it in schema.json.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void update_schema_button_Click(object sender, EventArgs e)
        {
            string address = "http://vconnect-danieladams456.rhcloud.com/schema";
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(address);
            StreamReader reader = new StreamReader(stream);
            String json = reader.ReadToEnd();
            File.WriteAllText("schema.json", json);
        }


        /// <summary>
        /// Start polling data from the vehicle, if able. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void start_button_Click(object sender, EventArgs e)
        {
            // If data is already being polled, then nothing to do. 
            if (pollingData)
                MessageBox.Show("Already Polling Data");

            // If no data is currently being polled, update the schema, then
            // begin polling data.
            else
            {
                schema = SchemaUpdate();
                pollData.Change(0, POLLTIME);
            }
        }


        /// <summary>
        /// Stop polling vehicle data. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stop_polling_button_Click(object sender, EventArgs e)
        {
            // If no data is currently being polled, do nothing. 
            if (pollingData == false)
                MessageBox.Show("Currently not polling Data.");
            
            // If data is still being polled, stop the process.  
            else
            {
                pollData.Change(Timeout.Infinite, Timeout.Infinite);
                pollData.Dispose();
            }
        }


        /// <summary>
        /// Test that data is successfully moved from vehicle data elements to
        /// the data cache.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CacheTest_Click(object sender, EventArgs e)
        {
            cache.DataCacheTest = true;
        }

        /// <summary>
        /// Test that the list of codes given to the testing team is the same
        /// as the codes being polled to the OBDII module.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OBDIITest_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Test to check if vehicle data was successfully transmitted
        /// to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerTest_Click(object sender, EventArgs e)
        {
            cache.ServerTest = true;
        }

        /// <summary>
        /// Test to see if data is successfully removed from the cache
        /// after it has been sent to the server. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveTest_Click(object sender, EventArgs e)
        {
            cache.CacheTest = true;
        }

        /// <summary>
        /// Test that the raw data polled from the OBDII module is equivalent
        /// to the data stored in the vehicle data elements.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataTest_Click(object sender, EventArgs e)
        {

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

            // Create a list of DataElements.
            List<DataElement> elemList = new List<DataElement>();
            
            // Create a dictionary of string-object pairs. This will contain the key-value pairs
            //  to be sent to the server.
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            // Create the "shell" of empty elements from the schema.
            elemList = CreateElementsFromSchema(schema);

            // Fill the contents of the elements with the data from the car
            elemList = GetElementData(elemList);

            // Create a dictionary out of the list of elements.
            dictionary = CreateDictionary(elemList);

            // Add the dictionary containing the data points to the cache.
            cache.AddElementToCache(dictionary);

            // Send the contents of the cache to the server.
            cache.SendToServer();

            // Check for OBDII error codes.
            CheckForErrorCodes();
            // cache.WriteToDisk();            

        //    MessageBox.Show(cache.JsonString, "JSON Results", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);

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
        ///     in the list. It should be passed the list of Data Elements returned from CreateElementsFromSchema
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
                    // Get data from the car for the element and format it.
                    elem.RequestDataFromCar();
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
                // If the datatype is a number, send the value as an integer
                if (elem.DataType == "number")
                    elementDictionary.Add(elem.Name, Int32.Parse(elem.ValueToSend));

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
        private void CheckForErrorCodes()
        {
            /*
            if (BTConnection.Client.Connected)
            {
                // encode message
                byte[] writeCode = System.Text.Encoding.ASCII.GetBytes("03");
                Stream peerStream = BTConnection.Client.GetStream();
                peerStream.Write(writeCode, 0, writeCode.Length);

                bool parse = true;

               

            }
            else
                MessageBox.Show("Cannot Check for Error Codes, no BT connection.");*/
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
            // Do we need this?
            return BTConnection;
        }
        
    }

}