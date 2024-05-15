using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGH_Client.Networking.Messages
{
    internal class RemoveFromGameMessage
    {
        public string playerName { get; set; } = "";
        
        public string gameType { get; set; } = "None";
        
        public int gameCode { get; set; } = 0;

        public RemoveFromGameMessage() { }

    }
}
