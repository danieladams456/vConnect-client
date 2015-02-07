using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public BluetoothConnectionHandler BTConnection;
  
        public Form1()
        {
            BluetoothConnectionHandler BTConnection = new BluetoothConnectionHandler();
            InitializeComponent();
            
            // This function begins requesting the car for data.
            requestDataForElements();
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void help_button_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void edit_port_Click(object sender, EventArgs e)
        {

        }

        private void edit_IP_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }


        // Refresh button for BT Devices
        private void refresh_button_Click(object sender, EventArgs e)
        {
            var cli = new BluetoothClient();
            BluetoothDeviceInfo[] peers = cli.DiscoverDevices();
            this.BT_Devices.DisplayMember = "DeviceName";
            this.BT_Devices.ValueMember = null;
            this.BT_Devices.DataSource = peers;
        }

        private void BT_Devices_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void apply_button_Click(object sender, EventArgs e)
        {

        }

        private void ok_button_Click(object sender, EventArgs e)
        {

        }

        private void server_test_Click(object sender, EventArgs e)
        {

        }

        private void browse_button_Click(object sender, EventArgs e)
        {
            var dlg = new SelectBluetoothDeviceDialog();
            DialogResult result = dlg.ShowDialog(this);
            if (result != DialogResult.OK)
            {
                return;
            }
            BluetoothDeviceInfo device = dlg.SelectedDevice;
            BluetoothAddress addr = device.DeviceAddress;
            label5.Text = device.DeviceName;
            BTConnection.changeBTAddress(addr);
            /*    // bool isPaired = false;
           var serviceClass = new Guid();
           serviceClass = BluetoothService.SerialPort;
           var cli = new BluetoothClient();
           var ep = new BluetoothEndPoint(addr, serviceClass);
         // isPaired = BluetoothSecurity.PairRequest(addr, "0000");
           cli.Connect(ep);
       if (cli.Connected)
       {
            
           BluetoothDeviceInfo[] peers = cli.DiscoverDevices();
           this.BT_Devices.DisplayMember = "DeviceName";
           this.BT_Devices.ValueMember = null;
           this.BT_Devices.DataSource = peers;

       }*/


        }

        public BluetoothConnectionHandler getBTConnection()
        {
            return BTConnection;
        }

        /// <summary>
        /// This function serves as the launching point for requesting data.
        ///     Note: These objects will not be explicitly named in the future.
        /// </summary>
        public void requestDataForElements()
        {
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
        }

    }



}
