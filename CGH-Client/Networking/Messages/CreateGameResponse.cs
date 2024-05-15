using System;
using System.Collections.Generic;
using CGH_Client.Utility;

namespace CGH_Client.Networking.Messages
{
    internal class CreateGameResponse
    {
        public int player { get; set; } = 0;

        public List<Player> players { get; set; } = new List<Player>();

        public string gameType { get; set; } = "None";
        
        public int roomCode { get; set; } = 0;
    }
}
