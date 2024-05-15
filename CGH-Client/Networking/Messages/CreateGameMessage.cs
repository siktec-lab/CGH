using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGH_Client.Networking.Messages
{
    internal class CreateGameMessage
    {
        public string playerName { get; set; }
        public int playerAvatar { get; set; }
        public string gameType { get; set; }
    }
}
