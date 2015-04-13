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
 *  
 * This class also supports general Json strings to be sent to a server, which 
 * in the case of vConnect, also includes Alert/Error codes. 
 *  
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
    /// cached for sending.
    /// </summary>
    class DataCache
    {
        // Cache list to hold data elements (will be in Json form.)
        List<Dictionary<string, object>> cache = new List<Dictionary<string, object>>();
        ServerConnectionHandler serverConnection;

        // File to write the cache to if necessary.
        const string CACHEFILE = "jsonCache.txt";
        
        // Value used by Form1 to determine if the server has been connected with as
        // of its last request. Used to keep UI up to date. 
        private bool connect_check = true;

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
        public bool SendToServer(string jsonString, string type)
        {
            // Check if the cache-file (used for storing cached data that failed to send)
            //  contains any data. If it does, then read that data.
            try
            {
                if (new FileInfo(CACHEFILE).Length != 0 && type != "alert")
                    ReadFromDisk();
            }
            catch (Exception e)
            {
                Form1.LogMessageToFile("error","Cache File Info Error", e.ToString());
            }
            

            string webAddress = null;
            
            // Create the web address to connect to
            webAddress = "http://" + serverConnection.IPAddress + ":" + serverConnection.PortNumber + "/" + type;
            
            // Create the web request with Json/Post attributes and given address
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = "vConnect";
            try
            {
                // Create streamWriter object to handle the request.
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    // Send json string and close the stream. 
                    streamWriter.Write(jsonString + "\n");
                    streamWriter.Flush();
                    streamWriter.Close();

                    // Get web response (most importantly, status code)
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    int statusCode = (int)httpResponse.StatusCode;
                   // streamWriter.Close();
                   // streamWriter.Dispose();
                    if (statusCode == 204)
                    {
                        // Set to true, to be used in Form1 to keep UI up to date in terms of 
                        // server connection status. 
                        connect_check = true;

                        // Empty the cache since all stored data elements have been successfully
                        // sent to the server.
                        if (type == "data")
                            cache.Clear();
                           
                        return true;
                    }
                    else
                    {
                        //  If the web server returned an unexpceted response code or failure,
                        //  write the current cache to disk, then clear it, if this was called to set
                        // data elements, and not alerts. 
                        connect_check = false;
                        Form1.LogMessageToFile("error","Server Response Error", "The server returned a " + statusCode.ToString() + " code instead of a 204");
                        if (type == "data")
                        {
                            WriteToDisk();
                            cache.Clear();
                        }
                        return false;

                    }
                    

                }
            }
            catch (WebException e)
            {
                // If the web server raised an exception, write the cached data to disk, then clear it.
                Form1.LogMessageToFile("error","Server Connect Error", e.ToString());
                if (type == "data")
                {
                    connect_check = false;
                    WriteToDisk();
                    cache.Clear();
                }
            }

            return false;
        }

        /// <summary>
        /// This function works, but it is very harsh. Need to do make it more robust before Alpha.
        /// </summary>
        public void WriteToDisk()
        {
            try
            {
                // This function creates a new file and writes to it.
                //  If the file already exists, it is overwritten.
                File.WriteAllText(CACHEFILE, JsonString);
            }
            catch (IOException e)
            {
                // If the write to file fails, log the error.
                Form1.LogMessageToFile("error","Cache File Write Error", e.ToString());
            }
        }

        /// <summary>
        /// Read the contents of the cache file to the List(Dict)
        /// 
        /// Note: After this method finishes, the List(Dict) object "cache" will contain
        ///     its original contents with the contents of the cache-file prepended to it. 
        /// </summary>
        public void ReadFromDisk()
        {
            // Temporary 'cache's to be used for creating a new cache containing the contents
            //  from the existing cache as well as the data read from the file.
            List<Dictionary<string, object>> readCache = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> tempCache = new List<Dictionary<string, object>>();
            string jsonFromFile = "";
            try
            {
                // This function creates a new file and writes to it.
                //  If the file already exists, it is overwritten.
                jsonFromFile = File.ReadAllText(CACHEFILE);
            }
            catch (IOException e)
            {
                // If the write to file fails, log the error.
                Form1.LogMessageToFile("error","Cache File Read Error", e.ToString());
            }

            try
            {
                // Create the new cache List<Dict> from the file contents and place it in
                //  readCache, a holding place.
                readCache = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonFromFile);
            }
            catch (JsonSerializationException e)
            {
                // If the Json -> List<Dictionary<string,object>> fails, catch it.
                Form1.LogMessageToFile("error","Cache File Read Error", "Malformed JSON in file:" + e.ToString());
            }

            // Create a temporary copy of the existing cache.
            tempCache = cache;

            // Re-create a List<dict> of the length of the existing cache + the cache read from file
            cache = new List<Dictionary<string, object>>(readCache.Count + tempCache.Count);

            // Re-populate the cache with the contents read from file and the original cache appended to it.
            cache.AddRange(readCache);
            cache.AddRange(tempCache);

            // Empty the cache-file contents.
            try
            {
                File.WriteAllText(CACHEFILE, string.Empty);
            }
            catch (IOException e)
            {
                // If the write to file fails, log the error.
                Form1.LogMessageToFile("error","Cache File Empty Error", e.ToString());
            }
        }

        // C# Accessor Method
        public string JsonString { get { return JsonConvert.SerializeObject(cache); } set { JsonString = value; } }
        public bool Connect_check { get { return connect_check; } set { connect_check = value; } }

    }
}
