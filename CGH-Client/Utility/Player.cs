using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGH_Client.Utility
{
    public class Player
    {

        public string Name { get; set; } = "";
        public int ImgCharNum { get; set; } = 0;
        public string gameID { get; set; } = "";
        public bool isHost { get; set; } = false;
        public bool isDisconnected { get; set; } = false;
        public string selectedCard { get; set; } = "";

        public Player()
        {

        }

        

    }
}
