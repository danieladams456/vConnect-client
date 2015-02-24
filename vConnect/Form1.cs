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
        BluetoothConnectionHandler BTConnection = new BluetoothConnectionHandler();
        ServerConnectionHandler serverConnection = new ServerConnectionHandler();
        DataCache cache = null;
        
        // For asychronous call to read OBDII codes.
        System.Threading.Timer myTimer;

        // List that will used to hold OBDII codes before being inserted into the cache.
        List<Dictionary<string,object>> elementDictionaryList = 
                                                new List<Dictionary<string,object>>();
        String schema = "";
        bool pollingData = false;
        int POLLTIME = 120000;
        
        public Form1()
        {
            InitializeComponent();
            cache = new DataCache(serverConnection);
            TimerCallback tcb = RequestDataForElements;

            bool deviceDetect = false;
            bool serverDetect = true;

            // TESTING ONLY !!!
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
                    server_status_label.Text = "Disconnected";
                }
            }
            else
                MessageBox.Show("No server connection data was found, please add server IP address and port number");
            

            
           // If connections have been established to the OBDII device and the server, then begin polling for 
           // vehicle data. 
            if (deviceDetect && serverDetect)
            {
                schema = SchemaUpdate();
                myTimer = new System.Threading.Timer(tcb, null, 0, POLLTIME);
            }
            else
                myTimer = new System.Threading.Timer(tcb, null, Timeout.Infinite, Timeout.Infinite);
        }


        private void ok_button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void apply_button_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// This button will close the setting GUI without applying any changes made.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void help_button_Click(object sender, EventArgs e)
        {
            string helpMessage = "This GUI is used to setup and manange vConnect's Windows Aplication. \n " +
                   "Please note that this GUI cannot manage the server, or search for stored data.\n" +
                   "Listed here are the details concerning the various attributes of this GUI:\n" +
                   "BT Device ID: The ID of the OBDII Device that is currently assigned to vConnect.\n" +
                   "Device Status: Whether the OBDII Device listed above is connected or disconnect. \n" +
                   "Connect to ODBII Device: Will open up a dialog that shows all detectable BT Devices, " +
                   "selecting a device will attempt to connect with it. \n" +
                   "Disconnect BT Device: Will disconnect to the ODBII device (if one is connected).\n" +
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
                if (Int32.Parse(value) > 0 && Int32.Parse(value) < 65535)
                {
                    Properties.Settings.Default.ServerPort = value;
                    Properties.Settings.Default.Save();
                    port_number.Text = value;
                    serverConnection.PortNumber = Int32.Parse(value);
                }
            }
        }


        private void server_test_button_Click(object sender, EventArgs e)
        {

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
            if (BTConnection.Client.Connected)
                MessageBox.Show(msg);
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

                // Can call this elsewhere, just have it here for now. 
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


        private void disconnect_BT_button_Click(object sender, EventArgs e)
        {
            BTConnection.CloseBTConnection();
        }


        /// <summary>
        /// Update the Schema from the supplied web site, and store it in schema.json
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


        private void start_button_Click(object sender, EventArgs e)
        {
            if (pollingData)
                MessageBox.Show("Already Polling Data");
            else
            {
                schema = SchemaUpdate();
                myTimer.Change(0, POLLTIME);
            }

        }


        private void stop_polling_button_Click(object sender, EventArgs e)
        {
            if (pollingData == false)
                MessageBox.Show("Currently not polling Data.");
            else
            {
                myTimer.Change(Timeout.Infinite, Timeout.Infinite);
                myTimer.Dispose();
            }
        }


        private void CacheTest_Click(object sender, EventArgs e)
        {
            cache.DataCacheTest = true;
        }


        private void OBDIITest_Click(object sender, EventArgs e)
        {

        }


        private void ServerTest_Click(object sender, EventArgs e)
        {
            cache.ServerTest = true;
        }


        private void RemoveTest_Click(object sender, EventArgs e)
        {
            cache.CacheTest = true;
        }


        private void DataTest_Click(object sender, EventArgs e)
        {

        }


        private string SchemaUpdate()
        {
            // GET SCHEMA!
            try
            {
                StreamReader reader = new StreamReader("schema.json");
                schema = reader.ReadToEnd();
            }
            catch (FileNotFoundException exception)
            {
                schema = "NOT FOUND";
                // ERROR GOES HERE WITH EXCEPTION
            }
            return schema;
        }


        /// <summary>
        /// This function serves as the launching point for requesting data.
        /// </summary>
        public void RequestDataForElements(object sender)
        {
            pollingData = true;

            List<DataElement> elemList = new List<DataElement>();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

           
            elemList = CreateElementsFromSchema(schema);
            elemList = GetElementData(elemList);
            dictionary = CreateDictionary(elemList);
            cache.AddElementToCache(dictionary);
            cache.SendToServer();
            CheckForErrorCodes();
            // cache.WriteToDisk();            

        //    MessageBox.Show(cache.JsonString, "JSON Results", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);

       }


        private List<DataElement> CreateElementsFromSchema(string schema)
        {
            JObject jsonObjectList = JObject.Parse(schema);
            List<DataElement> elementList = new List<DataElement>();

            // Temporary variables to hold data before object creation.
            string name = "", mode = "", code = "", size = "", type = "", equation = "";

            foreach (var pair in jsonObjectList)
            {
                name = pair.Key;

                if (jsonObjectList[name]["mode"] == null)
                    mode = "n/a";
                else
                    mode = (string)jsonObjectList[name]["mode"];

                if (jsonObjectList[name]["code"] == null)
                    code = "n/a";
                else
                    code = (string)jsonObjectList[name]["code"];

                if (jsonObjectList[name]["size"] == null)
                    size = "0";
                else
                    size = (string)jsonObjectList[name]["size"];

                if (jsonObjectList[name]["type"] == null)
                    type = "n/a";
                else
                    type = (string)jsonObjectList[name]["type"];

                if (jsonObjectList[name]["equation"] == null)
                    equation = "n/a";
                else
                    equation = (string)jsonObjectList[name]["equation"];

                // Add a DataElement to the list containing the values of the parsed object.
                elementList.Add(new DataElement(name, mode, code, type, Int32.Parse(size), equation, getBTConnection()));

            }

            return elementList;

        }


        private List<DataElement> GetElementData(List<DataElement> elemList)
        {
            // We now have a List of DataElements that matches the schema.

            // For each element in the list, if the element is not for TIME, 
            //  get data from the car and format it.
            foreach (DataElement elem in elemList)
            {
                if (elem.DataType == "date")
                {
                    // Do something here to place the date into this element.
                }
                else
                {
                    // Get data from the car for the element and format it.
                    elem.RequestDataFromCar();
                    elem.FormatData();
                }
            }

            return elemList;
        }


        private Dictionary<string, object> CreateDictionary(List<DataElement> elemList)
        {
            var elementDictionary = new Dictionary<string, object>();

            foreach (DataElement elem in elemList)
            {
                if (elem.DataType == "number")
                    elementDictionary.Add(elem.Name, elem.ValueToSend);
                else if (elem.DataType == "date") // Do we need to format 
                    //  the date in a special way?
                    elementDictionary.Add(elem.Name, elem.ValueToSend);
                else if (elem.DataType == "string")
                    elementDictionary.Add(elem.Name, elem.ValueToSend);
            }

            return elementDictionary;
        }


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

                // Parser
                
                while (!parse)
                {
                    System.Threading.Thread.Sleep(10000);



                }

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

        
        public BluetoothConnectionHandler getBTConnection()
        {
            return BTConnection;
        }
        
    }

}