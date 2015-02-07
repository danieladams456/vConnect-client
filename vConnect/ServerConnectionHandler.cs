using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vConnect
{
    /// <summary>
    /// This class manages the application's connection to the server.
    /// </summary>
    public class ServerConnectionHandler
    {
        private bool serverConnectionStatus = false;
        private string errorMessageToServer = "";
        private string errorMessageToClient = "";
        private string ipAddress = "";
        private int portNumber = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool EstablishServerConnection()
        {
            return true;
        }

        bool CloseServerConnection()
        {
            return true;
        }

        bool CheckServerConnection()
        {
            return serverConnectionStatus;
        }

        bool SendServerErrorMessage()
        {
            return true;
        }

        bool SendClientErrorMessage()
        {
            return true;
        }

        public bool setIPAddress(string newIP)
        {
            ipAddress = newIP;

            if (newIP.Length > 0)
                return true;
            else
                return false;
        }

        public bool setPortNumber(int newPortNum)
        {
            if (newPortNum > 0 && newPortNum < 65535)
            {
                portNumber = newPortNum;
                return true;
            }
            else
                return false;
        }


    }
}
