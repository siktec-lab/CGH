using CGH_Server.Networking;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CGH_Server.Networking.Messages;
using Newtonsoft.Json;

namespace CGH_Server.Utility
{
    public class GameRoom
    {
        
        public string gameType { get; set; }
        public int roomCode { get; set; }
        public List<Player> players { get; set; }
        
        private List<string> symbols = new List<string>()
        {
            "Clubs",
            "Diamonds",
            "Hearts",
            "Spades"
        };

        public List<string> deck;
            
        private List<string> values = new List<string>()
        {
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "J",
            "Q",
            "K",
            "A"
        };

        public bool gameStarted = false;

        public bool gameEnded = false;
        
        public WarGameRound game;

        public GameRoom()
        {
            // Generate the deck:
            deck = new List<string>();
            foreach (string symbol in symbols)
            {
                foreach (string value in values)
                {
                    deck.Add($"card{symbol}_{value}");
                }
            }

            this.gameType = "None";
            this.GenerateGameCode();
            this.players = new List<Player>();
            this.game = new WarGameRound();
        }

        public WarRoundResponse? PlayerTurn(string name)
        {
            WarRoundResponse response = new WarRoundResponse();
            int id = this.game.GetPlayerIndex(name);
            if (id == -1)
            {
                return null;
            }
            response.roomCode = this.roomCode;
            response.yourName = this.game.nameIndex[id];
            response.yourId = id;
            response.enemyName = this.game.GetPlayerEnemy(id);
            response.enemyId = this.game.GetPlayerIndex(response.enemyName);

            // Should we play a card?
            if (this.game.ShouldPlayerPlay(id))
            {
                this.game.CardForPlayer(id, this.DrawCard());
            }
            response.yourCard = this.game.CurrenCard(id);
            response.enemyCard = this.game.CurrenCard(response.enemyId);

            // Is round over:
            if (this.game.IsRoundOver())
            {
                this.game.round++;
                response.isRoundOver = true;
                response.roundWinner = this.game.CalculateHandWinner(this.values);
            }
            
            response.yourScore = this.game.CurrenScore(id);
            response.enemyScore = this.game.CurrenScore(response.enemyId);
            response.roundNumber = this.game.round;

            // Is game over?
            response.isGameOver = this.game.IsGameOver();
            if (response.isGameOver)
            {
                this.gameEnded = true;
                response.gameWinner = this.game.CalculateFinalWinner();
            }
            
            return response;
        }

        public string DrawCard()
        {
            Random rand = new Random();
            int index = rand.Next(0, deck.Count);
            string card = deck[index];
            deck.RemoveAt(index);
            return card;
        }
        
        public void AddPlayer(Player player)
        {
            players.Add(player);
        }
        
        public void AddPlayer(string name, int imgCharNum)
        {
            // TODO: make sure the player name is unique.
            players.Add(new Player()
            {
                Name            = name,
                ImgCharNum      = imgCharNum,
                gameID          = this.gameType + "-" + this.roomCode,
                isHost          = this.players.Count == 0,
                isDisconnected  = false,
                selectedCard    = ""
            });
        }

        public bool RemovePlayer(string name)
        {
            Player? player = this.players.Find(p => p.Name == name);
            if (player != null && players.Remove(player))
            {
                Globals.phoneBook.RemoveAddress(this.roomCode, name);
                return true;
            }
            return false;
        }
        
        public void RemoveAllPlayers()
        {
            // Remove all players from phone book:
            foreach (Player p in this.players)
            {
                Globals.phoneBook.RemoveAddress(this.roomCode, p.Name);
            }
            this.players.Clear();
        }
        
        public int getPlayerIndex(string name)
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
        
        public Player? GetPlayer(string name)
        {
            foreach (Player player in this.players)
            {
                if (player.Name == name)
                {
                    return player;
                }
            }
            return null;
        }

        public Player? GetPlayer(int index)
        {
            if (this.players.Count > index && index >= 0)
            {
                return this.players[index];
            }
            return null;
        }
        
        public int GenerateGameCode()
        {
            // Genreate a random game code that is based on the current time:
            // This is a simple way to generate a unique game code.
            // and avoid collisions.
            long ticks = DateTime.Now.Ticks;

            // Truncate to the last 4 digits:
            int code = (int)(ticks % 10000);

            // Generate a random number between 1 and 1000:
            Random random = new Random();
            int randomCode = random.Next(1, 100);

            // Combine the two:
            this.roomCode = code * 1000 + randomCode;

            return Math.Abs(this.roomCode);
        }

        //sends Message over TCP
        public void SendMessageToUser(string name, string msg, string purpose)
        {
            TCP? transport = Globals.phoneBook.GetAddress(this.roomCode, name);
            transport?.SendMessage(msg, purpose);
            /*if (client == null || !transport.client.Connected)
                return;
            try
            {
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    // Serialize:
                    string json = JsonConvert.SerializeObject(new ClientMsg() { purpose = purpose, msg = msg });

                    //Encrypt:
                    if (transport.protect.HasKey())
                    {
                        json = this.protect.Encrypt(json);
                    }
                    
                    byte[] msgArr = Encoding.ASCII.GetBytes(json);
                    stream.Write(msgArr, 0, msgArr.Length);
                }
            }
            catch { }*/
        }

        public void SendMessageToAll(string msg, string purpose, List<string>? exclude = null)
        {
            foreach (Player player in this.players)
            {
                if (exclude != null && exclude.Contains(player.Name))
                    continue;
                SendMessageToUser(player.Name, msg, purpose);
            }
        }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public bool SaveGameToFile()
        {
            string file = Globals.FilterGameRoomAndBuildPath(this.gameType, this.roomCode);
            try
            {
                File.WriteAllText(file, this.ToJson());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteGameFile()
        {
            string file = Globals.FilterGameRoomAndBuildPath(this.gameType, this.roomCode);
            try
            {
                File.Delete(file);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class WarGameRound
    {
        public List<string> nameIndex { get; set; } = new List<string>();

        public List<string> player0Cards { get; set; } = new List<string>();

        public List<string> player1Cards { get; set; } = new List<string>();

        public int fullRounds { get; set; } = 6; // 26 is the max number of rounds in a game of war

        public int round = 0;
        
        public int points0 { get; set; }

        public int points1 { get; set; }

        public int winner { get; set; } = 0;

        public int GetPlayerIndex(string name)
        {
            // Get the index of the player name:
            int index = nameIndex.IndexOf(name);
            if (index == -1 && nameIndex.Count < 2 && name != "")
            {
                nameIndex.Add(name);
                index = nameIndex.Count - 1;
            }
            return index;
        }

        public string GetPlayerEnemy(string name)
        {
            int myIndex = this.GetPlayerIndex(name);
            return this.GetPlayerEnemy(myIndex);
        }
        
        public string GetPlayerEnemy(int myId)
        {
            if (myId == 0)
            {
                return this.nameIndex.Count > 1 ? this.nameIndex[1] : "";
            }
            else if (myId == 1)
            {
                return this.nameIndex.Count > 0 ? this.nameIndex[0] : "";
            }
            else
            {
                return "";
            }
        }
        
        public bool ShouldPlayerPlay(int index)
        {
            if (index == 0)
            {
                return player0Cards.Count <= player1Cards.Count;
            }
            else
            {
                return player1Cards.Count <= player0Cards.Count;
            }
        }

        public void CardForPlayer(string player, string card)
        {
            int index = this.GetPlayerIndex(player);
            this.CardForPlayer(index, card);
        }
        public void CardForPlayer(int player, string card)
        {
            if (player == 0)
            {
                player0Cards.Add(card);
            }
            else
            {
                player1Cards.Add(card);
            }
        }

        public string CurrenCard(string player)
        {
            int index = this.GetPlayerIndex(player);
            return this.CurrenCard(index);
        }
        public string CurrenCard(int player)
        {
            if (player == 0)
            {
                return this.player0Cards.Count > 0 && this.player0Cards.Count == this.round + 1 ? this.player0Cards[this.player0Cards.Count - 1] : "";
            }
            else
            {
                return this.player1Cards.Count > 0 && this.player1Cards.Count == this.round + 1 ? this.player1Cards[this.player1Cards.Count - 1] : "";
            }
        }

        public int CurrenScore(string player)
        {
            int index = this.GetPlayerIndex(player);
            return this.CurrenScore(index);
        }
        public int CurrenScore(int player)
        {
            if (player == 0)
            {
                return this.points0;
            }
            else
            {
                return this.points1;
            }
        }
        
        public bool IsRoundOver()
        {
            return player0Cards.Count == player1Cards.Count;
        }

        public int CalculateHandWinner(List<string> values)
        {
            string card0 = player0Cards[player0Cards.Count - 1];
            string card1 = player1Cards[player1Cards.Count - 1];
            int value0 = values.IndexOf(card0.Split('_')[1]);
            int value1 = values.IndexOf(card1.Split('_')[1]);
            if (value0 > value1)
            {
                this.points0++;
                return 0;
            }
            else if (value0 < value1)
            {
                this.points1++;
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public bool IsGameOver()
        {
            return IsRoundOver() && this.round == this.fullRounds;
        }
        
        public int CalculateFinalWinner()
        {
            if (this.points0 == this.points1)
            {
                this.winner = -1;
            }
            else if (this.points0 > this.points1)
            {
                this.winner = 0;
            }
            else
            {
                this.winner = 1;
            }
            return this.winner;
        }

    }
}
