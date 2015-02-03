using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vConnect
{
    class DataElement
    {
        /// <summary>
        /// This class describes a data element received from the car via BT.
        /// </summary>
        private string obdPID = "";
        private int returnDataSize = 0;
        private double returnData = 0;

        public bool RequestDataFromCar()
        {
            return true;
        }

        public void FormatData()
        {
            return;
        }

        public bool SendEmergencyCode()
        {
            return true;
        }
    }
}
