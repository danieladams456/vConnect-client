using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Net;

namespace vConnect
{
    /// <summary>
    /// This class manages the BT connection.
    /// </summary>
    public class BluetoothConnectionHandler
    {
        private string deviceID = "";
        private bool BTConnectionStatus = false;
        private string erorrMessageToUI = "";
        // new addition!!
        private BluetoothAddress bluetooth_Address;

        public void changeBTAddress(BluetoothAddress addr)
        {
            bluetooth_Address = addr;
        }

        bool EstablishBTConnection()
        {
            return true;
        }

        bool CloseBTConnection()
        {
            return true;
        }

        bool CheckBTConnection()
        {
            return BTConnectionStatus;
        }

        bool SendWindowsErrorMessage()
        {
            return true;
        }


    }
}
