using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGH_Client.Utility
{
    internal class WarGameRoom : BaseGameRoom
    {
        public WarGameRoom() {
            this.gameType = "War";
        }
        
        public override bool IsGameReady()
        {
            return this.players.Count >= 2;
        }

        public override void StartGame()
        {
            this.isGameStarted = true;
            this.isGameEnded = false;
        }
    }

    internal class WarGameRound
    {
        string myCard { get; set; }
        
        string enemyCard { get; set; }

        int points { get; set; }

    }
}
