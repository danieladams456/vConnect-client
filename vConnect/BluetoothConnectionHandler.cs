using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vConnect
{
    /// <summary>
    /// This class manages the BT connection.
    /// </summary>
    class BluetoothConnectionHandler
    {
        private string deviceID = "";
        private bool BTConnectionStatus = false;
        private string erorrMessageToUI = "";

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
