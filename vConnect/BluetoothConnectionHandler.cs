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
        private bool bTConnectionStatus = false;
        private string errorMessageToUI = "";
        private BluetoothAddress bluetoothAddress;

        bool EstablishBTConnection()
        {
            return true;
        }

        bool CloseBTConnection()
        {
            return true;
        }

        bool SendWindowsErrorMessage()
        {
            return true;
        }

        // C# Style Accessors. Refer to DataElement.cs for example usage.

        public string DeviceID { get { return deviceID; } set { deviceID = value; } }
        public bool BTConnectionStatus { get { return bTConnectionStatus; } set { bTConnectionStatus = value; } }
        public string ErorrMessageToUI { get { return errorMessageToUI; } set { errorMessageToUI = value; } }
        public BluetoothAddress BluetoothAddress { get { return bluetoothAddress; } set { bluetoothAddress = value; } }


    }
}
