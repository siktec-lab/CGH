using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGH_Server.Networking.Messages
{
    internal class RemoveFromGameRequest
    {
        public string? playerName { get; set; } = null;
        
        public string gameType { get; set; } = "None";
        
        public int gameCode { get; set; } = 0;

        public RemoveFromGameRequest() { }

    }
}
