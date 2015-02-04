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
        BluetoothConnectionHandler BT_Connect = new BluetoothConnectionHandler();



        public Form1()
        {
            InitializeComponent();
 
           
           
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
            BT_Connect.changeBTAddress(addr);
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
    }
}
