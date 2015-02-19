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
        System.Threading.Timer myTimer;

        List<Dictionary<string,object>> elementDictionaryList = 
                                                new List<Dictionary<string,object>>();
        String schema = "";

        public Form1()
        {
            InitializeComponent();
            cache = new DataCache(serverConnection);
            // TESTING ONLY !!!
            //serverConnection.PortNumber = 9999;
            //serverConnection.IPAddress = "192.168.56.101";
            bool deviceDetect = false;
            TimerCallback tcb = requestDataForElements;
            // This will eventauly try to connect to BT device address in a config file first. 
            
            // Adds all detectable BT devices to peers[], then attempts to connect
            // to any that are OBDII devices.
            if (Properties.Settings.Default.BTAddress != "")
            {          
                BTConnection.BluetoothAddress = BluetoothAddress.Parse(Properties.Settings.Default.BTAddress);
                if(BTConnection.EstablishBTConnection())
                {
                    BT_ID.Text = Properties.Settings.Default.BTDeviceName;
                    device_Status_Label.Text = "Connected";
                    byte[] introMessage = new byte[100];
                    deviceDetect = true;
                    //System.Threading.Thread.Sleep(5000);
                    // Read any intro text from pesky BT modules.
                    Stream peerStream = BTConnection.Client.GetStream();
                    peerStream.Read(introMessage, 0, 100);
                    peerStream.Close();

                }
            }
               
            // Automatically search for OBDII device, and connect if able. 
            else if (!deviceDetect)
            {
               
            BluetoothDeviceInfo[] peers = BTConnection.Client.DiscoverDevices();
            int x = 0;
            while (x < peers.Length)
            {
 
                if (peers[x].DeviceName == "CBT." || peers[x].DeviceName == "OBDII")
                    {
                        BTConnection.BluetoothAddress = peers[x].DeviceAddress;

                        if (BTConnection.EstablishBTConnection())
                        {
                            BT_ID.Text = peers[x].DeviceName;
                            BTConnection.DeviceID = peers[x].DeviceName;
                            device_Status_Label.Text = "Connected";
                            byte[] introMessage = new byte[100];
                            x = peers.Length;
                            deviceDetect = true;
                            System.Threading.Thread.Sleep(5000);
                            // Read any intro text from pesky BT modules.
                           // Stream peerStream = BTConnection.Client.GetStream();
                            //peerStream.Read(introMessage, 0, 100);
                            
                          //  peerStream.Close();

                        }

                    }
                    x++;
                }
            }
            // Didn't connect to OBDII module, start GUI, but not polling data. 
           
           if (deviceDetect == false)
                MessageBox.Show("No OBDII devices were detected.");
                // set Cursor to red. 

            // Start asychronous timer to poll data from OBDII module. 
            
            
               // make gui element for changing timer time 
           else 
                myTimer = new System.Threading.Timer(tcb, null, 0, 120000);
            

            }
        
        

        /// <summary>
        /// This function serves as the launching point for requesting data.
        ///     Note: These objects will not be explicitly named in the future.
        /// </summary>
        public void requestDataForElements(object sender)
        {
            
            /*

            // DataElement(elementName, mode, PID, dataType, numberBytesReturned, eqn, btconnection)
                   
            // Obviously, for the final version, we will not explicitly name these and define
            //  their obdPID and numBytesReturned values. THESE WILL COME FROM THE JSON SCHEMA.
            //  However, for current testing purposes, it will be simpler to call each object by
            //  the name of the actual element.
            // Name, obdPID, numBytes
            
            
            DataElement vin = new DataElement("vin", "02", 1, "A", getBTConnection());
            DataElement speed = new DataElement("speed", "0D", 1, "A", getBTConnection());
            DataElement rpm = new DataElement("rpm", "0C", 2, "((A*256)+B)/4", getBTConnection());
            DataElement run_time_since_start = new DataElement("run_time_since_start", "1F", 2, "(A*256)+B", getBTConnection());
            DataElement fuel = new DataElement("fuel_level_input", "2F", 1, "A*100/255", getBTConnection());
            DataElement oil_temp = new DataElement("oil_temp", "5C", 1, "A - 40", getBTConnection());
            DataElement accel = new DataElement("accel_position", "5A", 1, "A*100/255", getBTConnection());
            DataElement dist_with_MIL = new DataElement("distance_since_MIL", "21", 2, "(A*256)+B", getBTConnection());
            
             
            // Note that for the final version, we will be running all of this in some form of loop, 
            // not only one time. But for the purposes of making sure each part of this application is
            // functional, it will be simplest to call them explicitly, once.

            // Requests Data from Car. If BT connection to OBDII module is lost and not 
            // automatically regained, then the function will end and the loop stop iterating.
            if (!vin.RequestDataFromCar())
            {
                device_Status_Label.Text = "Disconnected";
                myTimer.Change(Timeout.Infinite, Timeout.Infinite);
                return;
            }
            if (!speed.RequestDataFromCar())
            {
                device_Status_Label.Text = "Disconnected";

                myTimer.Change(Timeout.Infinite, Timeout.Infinite);
                return;
            }
            if (!rpm.RequestDataFromCar())
            {
                device_Status_Label.Text = "Disconnected";

                myTimer.Change(Timeout.Infinite, Timeout.Infinite);
                return;
            }
            if (!run_time_since_start.RequestDataFromCar())
            {
                device_Status_Label.Text = "Disconnected";

                myTimer.Change(Timeout.Infinite, Timeout.Infinite);
                return;
            }
            if (!fuel.RequestDataFromCar())
            {
                device_Status_Label.Text = "Disconnected";

                myTimer.Change(Timeout.Infinite, Timeout.Infinite);
                return;
            }
            if (!oil_temp.RequestDataFromCar())
            {
                device_Status_Label.Text = "Disconnected";

                myTimer.Change(Timeout.Infinite, Timeout.Infinite);
                return;
            }
            if (!accel.RequestDataFromCar())
            {
                device_Status_Label.Text = "Disconnected";

                myTimer.Change(Timeout.Infinite, Timeout.Infinite);
                return;
            }
            if (!dist_with_MIL.RequestDataFromCar())
            {
                device_Status_Label.Text = "Disconnected";
                myTimer.Change(Timeout.Infinite, Timeout.Infinite);
                return;
            }
            

            // Format all the data elements in a manner that will be stored in the data cache. 
            //  not only one time. But for the purposes of making sure each part of this application is
            //  functional, it will be simplest to call them explicitly, once.
            
            
            vin.RequestDataFromCar();
            speed.RequestDataFromCar();
            rpm.RequestDataFromCar();
            run_time_since_start.RequestDataFromCar();
            fuel.RequestDataFromCar();
            oil_temp.RequestDataFromCar();
            accel.RequestDataFromCar();
            dist_with_MIL.RequestDataFromCar();
            


             
            vin.FormatData();
            speed.FormatData();
            rpm.FormatData();
            run_time_since_start.FormatData();
            fuel.FormatData();
            oil_temp.FormatData();
            accel.FormatData();
            dist_with_MIL.FormatData();
             

            
            // Send to cache!
            // Create new cluster containing each of the values read.
            ElementCluster cluster = new ElementCluster(vin.ValueToSend, speed.ValueToSend, rpm.ValueToSend,
                                            run_time_since_start.ValueToSend, fuel.ValueToSend, oil_temp.ValueToSend,
                                            accel.ValueToSend, dist_with_MIL.ValueToSend);

            
            // Add cluster to the list of clusters in the cache.
            cache.AddElementToCache(cluster);
            cache.AddElementToCache(cluster);

            // Temporarily, SendToServer doesn't actually send it to the server, but it creates a JSON file 
            //   (stored in a string) from the data currently in the cache.
            cache.SendToServer();

            // Since we aren't sending to server yet, the following message box allows you to view the created JSON.
            //  With the box in focus, ctrl+c allows for copy'n'paste, even though it doesn't look like it.
            MessageBox.Show(cache.JsonString, "JSON Results", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);

            // The following function will write the file to disk. It needs to be made MUCH more robust before Alpha.
            // cache.WriteToDisk();
          */
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
            MessageBox.Show("");
           
        }

        /// <summary>
        ///  Allows the user to enter a new port number using the GUI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void edit_port_Click(object sender, EventArgs e)
        {
            string value = "Port Number";
            if (InputBox("New Port Number", "New Port Number (1-65535):", ref value) == DialogResult.OK)
            {
                // Bounds checking for a valid port number
                if (Int32.Parse(value) > 0 && Int32.Parse(value) < 65535)
                {
                    port_number.Text = value;
                    serverConnection.PortNumber = Int32.Parse(value);
                }
            }
        }
        
        /// <summary>
        ///  Allows the user to enter a new IP address using the GUI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void edit_IP_Click(object sender, EventArgs e)
        {
            string value = "IP Address";
            if (InputBox("New IP Address", "New IP Address:", ref value) == DialogResult.OK)
            {
                server_IP.Text = value;

                // Should probably validate IP address here... 

    //            serverConnection.IPAddress = value;
            }
         }
        





        private void apply_button_Click(object sender, EventArgs e)
        {
            
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void server_test_Click(object sender, EventArgs e)
        {

        }

       /// <summary>
       /// Button opens up a Dialog box to select a BT device to attempt to connect to.
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void browse_button_Click(object sender, EventArgs e)
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
                myTimer.Change(0, 120000);
                BT_ID.Text = device.DeviceName;
            }
        }

        public BluetoothConnectionHandler getBTConnection()
        {
            return BTConnection;
        }

        private void button3_Click(object sender, EventArgs e)
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

        private void Disconnect_BT_Click(object sender, EventArgs e)
        {
            BTConnection.CloseBTConnection();
        }

        private void DataTest_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Update the Schema from the supplied web site, and store it in schema.json
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getSchema_Click(object sender, EventArgs e)
        {
            string address = "http://vconnect-danieladams456.rhcloud.com/schema";
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(address);
            StreamReader reader = new StreamReader(stream);
            String json = reader.ReadToEnd();
            File.WriteAllText("schema.json", json);
        }

        private void start_Click(object sender, EventArgs e)
        {
            string schema = schemaUpdate();          
            List<DataElement> elemList = new List<DataElement>();
            Dictionary<string,object> dictionary =  new Dictionary<string,object>();
            
            // This will loop through the make, read, add to cache process 10x.
            for (int a = 0; a < 10; a++)
            {
                elemList = createElementsFromSchema(schema);
                elemList = getElementData(elemList);
                dictionary = createDictionary(elemList);
                cache.AddElementToCache(dictionary);
            }

            cache.SendToServer();
        }

        private string schemaUpdate()
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

        private List<DataElement> createElementsFromSchema(string schema)
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

        private List<DataElement> getElementData(List<DataElement> elemList)
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

        private Dictionary<string,object> createDictionary(List<DataElement> elemList)
        {    
            var elementDictionary = new Dictionary<string, object>();
            
            foreach (DataElement elem in elemList)
            {
                if (elem.DataType == "number")
                    elementDictionary.Add(elem.Name, Int32.Parse(elem.ValueToSend));
                else if (elem.DataType == "date") // Do we need to format 
                                                  //  the date in a special way?
                    elementDictionary.Add(elem.Name, elem.ValueToSend);
                else if (elem.DataType == "string")
                    elementDictionary.Add(elem.Name, elem.ValueToSend);
            }

            return elementDictionary;
        }      

    }

}