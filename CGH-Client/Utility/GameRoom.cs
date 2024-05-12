using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGH_Client.Utility
{
    public class GameRoom
    {

        public string gameType { get; set; }
        public int roomCode { get; set; }
        public List<Player> players { get; set; }
        public string cardPlayed { get; set; }
        public string currentPlayerTurn { get; set; }

        public GameRoom()
        {

        }

    }
}
