using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGH_Server.Networking.Messages
{
    internal class JoinGameRequest 
    {
        public string? playerName { get; set; } = null;

        public int playerAvatar { get; set; } = 0;

        public string? gameType { get; set; } = null;

        public int gameCode { get; set; } = 0;

        public JoinGameRequest() { }
    }
}
