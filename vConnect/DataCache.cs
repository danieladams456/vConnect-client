using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace vConnect
{
    /// <summary>
    /// This class holds and sends the data elements after they are
    ///  cached for sending.
    /// </summary>
    class DataCache
    {
        List<ElementCluster> cache = new List<ElementCluster>();
        string jsonString = "";
        
        public void AddElementToCache(ElementCluster cluster)
        {
            cache.Add(cluster);
        }

        public void SendToServer()
        {
            JsonString = JsonConvert.SerializeObject(cache);
            
            // JSONstring now contains the correct JSON file to send to the server.
            
            
            // Send it. Server stuff here.
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
