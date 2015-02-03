using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace vConnect
{
    /// <summary>
    /// This class holds and sends the data elements after they are
    ///  cached for sending.
    /// </summary>
    class DataCache
    {
        private XmlDocument cache = new XmlDocument();

        bool AddElementToCache()
        {
            return true;
        }

        bool SendToServer()
        {
            return true;
        }

        bool WriteToDisk()
        {
            return true;
        }
    }
}
