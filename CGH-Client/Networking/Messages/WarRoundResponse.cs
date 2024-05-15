using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGH_Client.Networking.Messages
{
    public class WarRoundResponse
    {
        public int roomCode { get; set; } = 0;
        public string yourName { get; set; } = "";
        public int yourId { get; set; } = -1;

        public string enemyName { get; set; } = "";
        public int enemyId { get; set; } = -1;

        public int yourScore { get; set; } = 0;

        public int enemyScore { get; set; } = 0;

        public bool isRoundOver { get; set; } = false;

        public int roundWinner { get; set; } = -1;
        
        public int roundNumber { get; set; } = 0;

        public string yourCard { get; set; } = "";

        public string enemyCard { get; set; } = "";

        public WarRoundResponse() { }

    }
}
