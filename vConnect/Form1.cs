using System;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using InTheHand.Net.Ports;
using InTheHand.Net.Bluetooth;
using InTheHand.Windows.Forms;


namespace vConnect
{    

    public partial class Form1 : Form
    {
        BluetoothConnectionHandler BTConnection = new BluetoothConnectionHandler();
        ServerConnectionHandler serverConnection = new ServerConnectionHandler();

        public Form1()
        {
            InitializeComponent();

            // This will eventauly try to connect to BT device address in a config file, but will do this for now.
            var dlg = new SelectBluetoothDeviceDialog();
            DialogResult result = dlg.ShowDialog(this);
            if (result != DialogResult.OK)
            {
                return;
            }
            BluetoothDeviceInfo device = dlg.SelectedDevice;
            BluetoothAddress BTaddr = device.DeviceAddress;
            label5.Text = device.DeviceName;
            BTConnection.BluetoothAddress = BTaddr;

            BTConnection.EstablishBTConnection();



         //   System.Timers.Timer myTimer = new System.Timers.Timer(120000);
          //  myTimer.Elapsed += new ElapsedEventHandler(requestDataForElements);
           // myTimer.Enabled = true;
            TimerCallback tcb = requestDataForElements;
            System.Threading.Timer myTimer = new System.Threading.Timer(tcb, null, 0, 120000); 
         
        }

        /// <summary>
        /// This function serves as the launching point for requesting data.
        ///     Note: These objects will not be explicitly named in the future.
        /// </summary>
        public void requestDataForElements(object sender)
        {

            MessageBox.Show("Beginning of Thread", "My Application",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
            // Obviously, for the final version, we will not explicitly name these and define
            //  their obdPID and numBytesReturned values. THESE WILL COME FROM THE JSON SCHEMA.
            //  However, for current testing purposes, it will be simpler to call each object by
            //  the name of the actual element.
            // Name, obdPID, numBytes
            DataElement vin = new DataElement("vin", "02", 1, getBTConnection());
            DataElement speed = new DataElement("speed", "0D", 1, getBTConnection());
            DataElement rpm = new DataElement("rpm", "0C", 2, getBTConnection());
            DataElement run_time_since_start = new DataElement("run_time_since_start", "1F", 2, getBTConnection());
            DataElement fuel = new DataElement("fuel_level_input", "2F", 1, getBTConnection());
            DataElement oil_temp = new DataElement("oil_temp", "5C", 1, getBTConnection());
            DataElement accel = new DataElement("accel_position", "5A", 1, getBTConnection());
            DataElement dist_with_MIL = new DataElement("distance_since_MIL", "21", 2, getBTConnection());

            // Note that for the final version, we will be running all of this in some form of loop, 
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


            // Format ze data. 
            vin.FormatData();
            speed.FormatData();
            rpm.FormatData();
            run_time_since_start.FormatData();
            fuel.FormatData();
            oil_temp.FormatData();
            accel.FormatData();
            dist_with_MIL.FormatData();

            ///
            // Some type of impelementation to put data elements into data cache.
            ///


            // Cleaning up memory
            vin = null;
            speed = null;
            rpm = null;
            run_time_since_start = null;
            fuel = null;
            oil_temp = null;
            accel = null;
            dist_with_MIL = null;
            
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
            MessageBox.Show("Sorry, this button isn't quite helpful yet...", "My Application",
                  MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
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

                /* Should probably validate IP address here... */

                serverConnection.IPAddress = value;
            }
        }

        

       
        private void apply_button_Click(object sender, EventArgs e)
        {
            /*(MessageBox.Show("The calculations are complete", "My Application",
/MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);*/
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
            label5.Text = device.DeviceName;
            BTConnection.BluetoothAddress = BTaddr;
            
            // Can call this elsewhere, just have it here for now. 
            BTConnection.EstablishBTConnection();


            
         }

        /// <summary>
        /// Sample read/write code, nothing crazy thus far. 
        /// </summary>
        /// <returns></returns>
        /// 
        /*        Stream peerStream = cli.GetStream();
                    string stuff = "010D\r";
                    byte[] test = System.Text.Encoding.ASCII.GetBytes(stuff);
                    peerStream.Write(test, 0, test.Length);

         *             System.Threading.Thread.Sleep(10000);

                    byte[] readtest = new byte[200];
                    peerStream.Read(readtest, 0, 199);
                    string da = "not a code";
                    da = System.Text.Encoding.ASCII.GetString(readtest); 
                    MessageBox.Show(da, "My Application",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);*/



        public BluetoothConnectionHandler getBTConnection()
        {
            return BTConnection;
        }

       

      

    }



}
