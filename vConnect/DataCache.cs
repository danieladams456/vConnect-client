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
        string jsonString = "";
        ServerConnectionHandler serverConnection;

        // NOTE: These values are used for testing purposes only.
        private bool cacheTest = false;
        private bool serverTest = false;
        private bool dataCacheTest = false;
        
        public DataCache(ServerConnectionHandler serverConn)
        {
            serverConnection = serverConn;
        }

        public DataCache()
        {
            // Empty
        }

        public void AddElementToCache(Dictionary<string,object> dictionary)
        {
            cache.Add(dictionary);
        }

        public void SendToServer()
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
            JsonString = JsonConvert.SerializeObject(cache);
            MessageBox.Show(JsonString, "JSON Results", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
        /*    // Create the web address to connect to
            string webAddress = "http://" + serverConnection.IPAddress + ":" + serverConnection.PortNumber.ToString() + "/";
                
            // Create the web request with Json/Post attributes and given address
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = "vConnect";
            httpWebRequest.KeepAlive = false;
    
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
            }*/
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
        public string JsonString { get { return jsonString; } set { jsonString = value; } }
        public bool CacheTest { get { return cacheTest; } set { cacheTest = value; } }
        public bool ServerTest { get { return serverTest; } set { serverTest = value; } }
        public bool DataCacheTest { get { return dataCacheTest; } set { dataCacheTest = value; } }
    
    }

    /// <summary>
    /// This struct is used in order to create a correctly-formatted JSON file. That's it.
    /// </summary>
    public struct ElementCluster
    {
        public string VIN;
        public string vehicle_speed;
        public string engine_rpm;
        public string run_time_since_start;
        public string fuel_level;
        public string oil_temp;
        public string accel_pos;
        public string dist_with_MIL;

        /// <summary>
        /// Constructor that specifies each value when called.
        /// </summary>
        public ElementCluster(string VIN, string vehicle_speed, string engine_rpm, string run_time_since_start,
                                    string fuel_level, string oil_temp, string accel_pos, string dist_with_MIL)
        {
            this.VIN = VIN;
            this.vehicle_speed = vehicle_speed;
            this.engine_rpm = engine_rpm;
            this.run_time_since_start = run_time_since_start;
            this.fuel_level = fuel_level;
            this.oil_temp = oil_temp;
            this.accel_pos = accel_pos;
            this.dist_with_MIL = dist_with_MIL;
        }
    }

}
