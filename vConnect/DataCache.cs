

/* DataCache.cs - vConnect (Liberty University CSCI Capstone Project)
 * Written by Troy Cosner and Charlie Snyder in February-March, 2015. 
 * 
 * 
 * 
 * 
 * This class allows for the creation and maintenance of data elements and their 
 *  values to be sent to a server. It implements this using Json.Net in order to 
 *  store the data points in a list of dictionaries that are converted to a JSON
 *  string before being sent to the server. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace vConnect
{
    /// <summary>
    /// This class holds and sends the data elements after they are
    ///  cached for sending.
    /// </summary>
    class DataCache
    {
        //List<ElementCluster> cache = new List<ElementCluster>();
        List<Dictionary<string, object>> cache = new List<Dictionary<string, object>>();
        ServerConnectionHandler serverConnection;

        // NOTE: These values are used for testing purposes only.
        private bool cacheTest = false;
        private bool serverTest = false;
        private bool dataCacheTest = false;


        /// <summary>
        /// Constructor for initializing the server connection.
        /// </summary>
        /// <param name="serverConn"></param>
        public DataCache(ServerConnectionHandler serverConn)
        {
            serverConnection = serverConn;
        }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public DataCache()
        {
            // Empty
        }

        /// <summary>
        ///  Adds an element to the data Cache.
        /// </summary>
        /// <param name="dictionary"></param>
        public void AddElementToCache(Dictionary<string, object> dictionary)
        {
            cache.Add(dictionary);
        }

        /// <summary>
        /// Sends data cache element to the server.
        /// </summary>
        /// <returns>
        /// True - Data cache element was sent to the server, the server sent confirmation that it received the elements,
        /// and the element was successfully removed from the cache.
        /// False - the data was not successfully sent to the server, or the server didn't send confirmation that it recieved
        /// the element, or the element was not successfully removed from the cache.
        /// </returns>
        public bool SendToServer()
        {
            /*
             * In order to test this without needing the actual server runnning, turn on
             *  Kali in a VM. Make sure that it is connected on the virtual adapter and you
             *  can ping its IP. Set up a netcat listener with 
             *  $ echo -e "HTTP/1.1 200 OK\r\n" | nc -l -p 9999 -vvv
             *  where 9999 is whatever port number you wish. Configure that port number and
             *  your VM's ip in the settings for the app before you attempt to send the data.
             *  
             * Check and make sure the listener received the JSON and that the app received 
             *  a response of 200. The status code should pop up in a message box.
            */
            MessageBox.Show(JsonString, "JSON Results", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);

            /*
                   // Create the web address to connect to
                   string webAddress = "http://" + serverConnection.IPAddress + ":" + serverConnection.PortNumber.ToString() + "/";
                
                   // Create the web request with Json/Post attributes and given address
                   var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
                   httpWebRequest.ContentType = "text/json";
                   httpWebRequest.Method = "POST";
                   httpWebRequest.UserAgent = "vConnect";
                   httpWebRequest.KeepAlive = false;
            
                   try
                   {
                       // Write the current JSON string to the server
                       using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                       {
                           streamWriter.Write(JsonString);
                           streamWriter.Flush();
                           streamWriter.Close();
    
                           // Get web response (most importantly, status code)
                           var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                           int statusCode = (int)httpResponse.StatusCode;
    
                           MessageBox.Show(statusCode.ToString());
                           // If the web response was anything except 200, then problem. Handle it.
                           //if (statusCode!=200)
                           //{
                               // Handle Bad Error Request Here!
                           //}
                       }
                   }
                   catch (WebException e) {
                       MessageBox.Show("Could not connect to the server...", "Error!", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                   }
           */
            return true;
        }

        /// <summary>
        /// This function works, but it is very harsh. Need to do make it more robust before Alpha.
        /// </summary>
        public void WriteToDisk()
        {
            // string toWrite = "C:\\Users\\Charlie\\Desktop\\jsonCache.txt";
            // System.IO.File.WriteAllText(@toWrite, JsonString);
        }

        // C# Accessor Method
        public string JsonString { get { return JsonConvert.SerializeObject(cache); } }
        public bool CacheTest { get { return cacheTest; } set { cacheTest = value; } }
        public bool ServerTest { get { return serverTest; } set { serverTest = value; } }
        public bool DataCacheTest { get { return dataCacheTest; } set { dataCacheTest = value; } }

    }
}
