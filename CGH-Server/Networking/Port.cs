using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS_Server.Networking
{
    public class Port
    {
        public int port { get; set; }
        public bool isAvailable { get; set; }

        public Port(int port)
        {
            this.port = port;
            isAvailable = true;
        }
    }
}
