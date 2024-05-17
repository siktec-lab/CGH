using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGH_Client.Networking.Messages;
using Newtonsoft.Json;

namespace CGH_Client.Utility
{
    abstract public class BaseGameRoom
    {
        public string gameType { get; set; } = "None";

        public List<Player> players { get; set; } = new List<Player>();

        public int myPlayerIndex { get; set; } = -1;
        
        public string myPlayerName { get; set; } = "";
        
        public int currentPlayerTurn { get; set; } = -1;

        public string myCard { get; set; } = "";

        public int roomCode { get; set; } = 0;

        public bool isGameStarted { get; set; } = false;

        public bool isGameEnded { get; set; } = false;

        public BaseGameRoom()
        {

        }

        abstract public bool IsGameReady();

        abstract public void StartGame();
        
        public void AddPlayer(Player player)
        {
            players.Add(player);
        }
        
        public void AddPlayers(List<Player> player)
        {
            players.AddRange(player);
        }

        public void MergePlayers(List<Player> newPlayers)
        {
            // Add only new players:
            foreach (Player p in newPlayers)
            {
                Player found = this.players.Find(x => x.Name == p.Name);
                if (found == null || found.Name == "")
                {
                    this.AddPlayer(p);
                }
            }
        }

        public void AddPlayer(string name, int imgCharNum)
        {
            players.Add(new Player()
            {
                Name = name,
                ImgCharNum = imgCharNum,
                gameID = this.gameType + "-" + this.roomCode,
                isHost = this.players.Count == 0,
                isDisconnected = false,
                selectedCard = ""
            });
        }

        public bool RemovePlayer(string name)
        {
            Player player = players.Find(p => p.Name == name);
            if (player.Name != "")
            {
                return players.Remove(player);
            }
            return false;
        }
        
        public int GetPlayerIndex(string name)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public Player GetPlayer(string name)
        {
            foreach (Player player in this.players)
            {
                if (player.Name == name)
                {
                    return player;
                }
            }
            return new Player() { };
        }

        public Player GetPlayer(int index)
        {
            if (this.players.Count > index && index >= 0)
            {
                return this.players[index];
            }
            return new Player() { };
        }

        public Player GetMyPlayer()
        {
            return this.GetPlayer(this.myPlayerName);
        }

        public List<Player> GetOpponents(int howmMany = 0)
        {
            List<Player> opponents = new List<Player>();
            foreach (Player player in this.players)
            {
                if (player.Name != this.myPlayerName)
                {
                    opponents.Add(player);
                }
                if (howmMany > 0 && opponents.Count >= howmMany)
                {
                    break;
                }
            }
            return opponents;
        }

        public void RemoveFromGame()
        {
            RemoveFromGameMessage message = new RemoveFromGameMessage(){
                playerName = this.myPlayerName,
                gameType = this.gameType,
                gameCode = this.roomCode
            };
            // Serialize the message
            Globals.ServerConnector.SendMessage(
                JsonConvert.SerializeObject(message), 
                "removeFromGame"
            );
        }
    }
}
