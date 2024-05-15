using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGH_Client.Networking.Messages
{
    internal class GameRoundMessage
    {
        public string playerName { get; set; }
        
        public string gameType { get; set; }

        public int gameCode { get; set; }
        
        public string action { get; set; } = "turn";
    }
}
