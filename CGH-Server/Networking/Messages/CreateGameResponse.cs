using CGH_Server.Utility;
using System.Collections.Generic;

namespace CGH_Server.Networking.Messages
{
    internal class CreateGameResponse
    {
        public int player { get; set; } = 0;

        public List<Player> players { get; set; } = new List<Player>();

        public string gameType { get; set; } = "None";
        
        public int roomCode { get; set; } = 0;
        
        public void From(GameRoom room, string playerName)
        {
            this.player     = room.getPlayerIndex(playerName);
            this.players    = room.players;
            this.gameType   = room.gameType;
            this.roomCode   = room.roomCode;
        }
    }
}
