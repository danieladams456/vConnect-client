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
        /// Function that sends a specific http request to the specified IP and port number to determine
        /// if the server is currently accepting vConnect/OBDII requests. 
        /// </summary>
        /// <returns>
        /// True => A http request to the server is verified.
        /// False => A http request to the server fails. 
        /// </returns>
        public bool CheckServerConnection()
        {
            try
            {
                // Web address to send the request to. 
                string webAddress = "http://" + ipAddress + ":" + portNumber + "/status";
                
                // Create the web request with /Post attributes and given address
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
                httpWebRequest.ContentType = "text/plain";
                httpWebRequest.Method = "HEAD";
                httpWebRequest.UserAgent = "vConnect";

                // Get web response (most importantly, status code)
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                int statusCode = (int)httpResponse.StatusCode;

                if (statusCode.ToString() == "204")
                    return true;
            }
            catch (Exception e)
            {
                Form1.LogMessageToFile("Server Connection Handler", e.Message);
                return false;
            }

            return false;
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
