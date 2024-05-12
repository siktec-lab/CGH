using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGH_Server.Utility
{
    public class Player
    {

        public string Name { get; set; }
        public int ImgCharNum { get; set; }
        public string gameID { get; set; }
        public bool isHost { get; set; }
        public bool isDisconnected { get; set; }
        public string selectedCard { get; set; }

        public Player()
        {

        }

    }
}
