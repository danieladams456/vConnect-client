/* DataCache.cs - vConnect (Liberty University CSCI Capstone Project)
 * Written by Troy Cosner and Charlie Snyder in February-March, 2015. 
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
using System.Windows.Forms;
using Newtonsoft.Json;

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

        // File to write the cache to if necessary.
        const string CACHEFILE = "jsonCache.txt";

        // Value used by Form1 to determine if the server has been connected with as
        // of its last request. Used to keep UI up to date. 
        private bool connect_check = true;

        // Variables to hold the ip and port number. They are eventually set by Form1
        private string ipAddress = "";
        private int portNumber = 0;

        /// <summary>
        /// Default Constructor that essentially does nothing.
        /// </summary>
        public DataCache()
        {
            // Empty
        }

        /// <summary>
        ///  Adds a dictionary to the list of dictionaries that makes up the cache.
        /// </summary>
        /// <param name="dictionary">dictionary containing string,object pairs 
        ///                             containing name,val for data points</param>
        public void AddElementToCache(Dictionary<string, object> dictionary)
        {
            cache.Add(dictionary);
        }

        /// <summary>
        /// Sends data cache to the server in a JSON Format
        /// </summary>
        /// <returns>
        /// True - Data cache element was sent to the server, the server sent 
        ///     confirmation that it received the elements, and the element was 
        ///     successfully removed from the cache.
        /// False - the data was not successfully sent to the server, or the server 
        ///     didn't send confirmation that it recieved the element, or the element 
        ///     was not successfully removed from the cache.
        /// </returns>
        public bool SendToServer(string jsonString, string type)
        {

            // Check if the cache-file (used for storing cached data that failed to send)
            //      contains any data. If it does, then read that data into the existing cache string.
            try
            {
                // If the file is not empty and we are not sending alerts, read the file.
                if (new FileInfo(CACHEFILE).Length != 0 && type != "alert")
                {
                    // Read from the 
                    ReadFromDisk();
                    jsonString = JsonString;

                    // Record read-cache-from-file to the event log
                    Form1.LogMessageToFile("event", "SendToServer", "Data to send: " + jsonString);
                }
            }
            catch (Exception e)
            {
                // Log any exceptions
                Form1.LogMessageToFile("error", "Cache File Info Error", e.ToString());
            }

            // Construct the address of the server
            string webAddress = "http://" + ipAddress + ":" + portNumber + "/" + type;

            // Create the HTTP request with Json/Post attributes and the given address
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = "vConnect";
            httpWebRequest.Timeout = 5000;

            // Begin writing to the server
            try
            {
                // Create streamWriter object to handle the request.
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    // Send json string and close the stream. 
                    streamWriter.Write(jsonString + "\n");

                    // Get web response (most importantly, the status code from the server)
                    // Note: 204 expected.
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    // Get the status code form the response
                    int statusCode = (int)httpResponse.StatusCode;

                    // Correct response from the server = HTTP 204
                    if (statusCode == 204)
                    {
                        // Set to true, to be used in Form1 to keep UI up to date in terms of 
                        // server connection status. 
                        connect_check = true;

                        // Empty the cache since all stored data elements have been successfully
                        // sent to the server. Note: Don't do this if sending alerts only.
                        if (type == "data")
                            cache.Clear();

                        // Return true since a successful send.
                        return true;
                    }
                    else
                    {
                        // For the UI, the check is considered a failure
                        connect_check = false;

                        // Log the status code received in place of 204.
                        Form1.LogMessageToFile("error", "Server Response Error", "The server returned a "
                                                        + statusCode.ToString() + " code instead of a 204");

                        // Write the cache to file due to failed send and clear the cache object
                        //      if sending data and not alerts
                        if (type == "data")
                        {
                            WriteToDisk();
                            cache.Clear();
                        }

                        // Return false due to unexpected server reply.
                        return false;
                    }
                }
            }

            // If the web request encounters a web-related exception, log it here.
            catch (WebException e)
            {
                // Log the error
                // If the web server raised an exception, write the cached data to disk, then clear it.
                Form1.LogMessageToFile("error", "Server Connect Error", e.ToString());

                // If dealing with data and not alerts, write to disk and clear.
                if (type == "data")
                {
                    connect_check = false;
                    WriteToDisk();
                    cache.Clear();
                }
            }

            // If this point is reached, there was a failure, so return false.
            return false;
        }

        /// <summary>
        /// This function writes the contents of the cache to the specified file CACHEFILE
        /// to be sent to the server at a later time when connection is regained.
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
                Form1.LogMessageToFile("error", "Cache File Write Error", e.ToString());
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

            // READ
            try
            {
                // This function creates a new file and writes to it.
                // If the file already exists, it is overwritten.
                jsonFromFile = File.ReadAllText(CACHEFILE);
            }
            catch (IOException e)
            {
                // If the write to file fails, log the error.
                Form1.LogMessageToFile("error", "Cache File Read Error", e.ToString());
            }

            // String->JSON
            try
            {
                // Create the new cache List<Dict> from the file contents and place it in
                //  readCache, a holding place.
                readCache = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonFromFile);
            }
            catch (JsonSerializationException e)
            {
                // If the Json -> List<Dictionary<string,object>> fails, catch it.
                Form1.LogMessageToFile("error", "Cache File Read Error", "Malformed JSON in file:" + e.ToString());
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
                Form1.LogMessageToFile("error", "Cache File Empty Error", e.ToString());
            }
        }


        public bool CheckServerConnection()
        {
            // Web address to send the request to. 
            string webAddress = "http://" + ipAddress + ":" + portNumber + "/status";

            // Create the web request with Post attributes and given address
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
            httpWebRequest.ContentType = "text/plain";
            httpWebRequest.Method = "HEAD";
            httpWebRequest.UserAgent = "vConnect";
            httpWebRequest.Timeout = 5000;



            try
            {
                // Get web response (most importantly, status code)
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                int statusCode = (int)httpResponse.StatusCode;
                httpResponse.Close();
                if (statusCode.ToString() == "204")
                    return true;
            }
            catch (Exception e)
            {
                Form1.LogMessageToFile("error", "Server Connection Handler", e.Message);
                return false;
            }


            catch
            {
                return false;
            }
            return false;
        }

        // C# Accessor Method
        public string JsonString { get { return JsonConvert.SerializeObject(cache); } set { JsonString = value; } }
        public bool Connect_check { get { return connect_check; } set { connect_check = value; } }
        public string IPAddress { get { return ipAddress; } set { ipAddress = value; } }
        public int PortNumber { get { return portNumber; } set { portNumber = value; } }

    }
}
