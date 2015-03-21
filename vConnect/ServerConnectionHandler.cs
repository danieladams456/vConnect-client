using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows.Forms;

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
        public bool EstablishServerConnection()
        {
            return true;
        }

        public bool CloseServerConnection()
        {
            return true;
        }

        public bool CheckServerConnection()
        {
            string webAddress = "http://" + ipAddress + ":" + portNumber + "/status";

            // Create the web request with Json/Post attributes and given address
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
            httpWebRequest.ContentType = "text/plain";
            httpWebRequest.Method = "HEAD";
            httpWebRequest.UserAgent = "vConnect";

            bool check = false;
            

                // Get web response (most importantly, status code)
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                int statusCode = (int)httpResponse.StatusCode;

                MessageBox.Show("Status Code: " + statusCode.ToString());
                if (statusCode.ToString() == "204")
                    check = true;
            

            return check;
        }

        public bool SendServerErrorMessage()
        {
            return true;
        }

        public bool SendClientErrorMessage()
        {
            return true;
        }


        // C# Style Accessors (get and set)
        // Refer to DataElement.cs accessors for method of using them.

        public bool ServerConnectionStatus { get { return serverConnectionStatus; } set { serverConnectionStatus = value; } }

        public string IPAddress { get { return ipAddress; } set { ipAddress = value; } }

        public int PortNumber { get { return portNumber; } set { portNumber = value; } }

        public string ErrorMessageToServer { get { return errorMessageToServer; } set { errorMessageToServer = value; } }

        public string ErrorMessageToClient { get { return errorMessageToClient; } set { errorMessageToClient = value; } }
    }
}
