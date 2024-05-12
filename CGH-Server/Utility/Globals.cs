using CRS_Server.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGH_Server.Utility
{
    public static class Globals
    {

        public static string baseDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        public static List<TCP> ClientTCPS;
        public static Router RouterServer;

        public static List<string> gameCards = new List<string>()
        {
            "cardClubs_A", "cardClubs_2", "cardClubs_3", "cardClubs_4", "cardClubs_5", "cardClubs_6", "cardClubs_7", "cardClubs_8", "cardClubs_9", "cardClubs_10", "cardClubs_J", "cardClubs_Q", "cardClubs_K", "cardDiamonds_A", "cardDiamonds_2", "cardDiamonds_3", "cardDiamonds_4", "cardDiamonds_5", "cardDiamonds_6", "cardDiamonds_7", "cardDiamonds_8", "cardDiamonds_9", "cardDiamonds_10", "cardDiamonds_J", "cardDiamonds_Q", "cardDiamonds_K", "cardHearts_A", "cardHearts_2", "cardHearts_3", "cardHearts_4", "cardHearts_5", "cardHearts_6", "cardHearts_7", "cardHearts_8", "cardHearts_9", "cardHearts_10", "cardHearts_J", "cardHearts_Q", "cardHearts_K", "cardSpades_A", "cardSpades_2", "cardSpades_3", "cardSpades_4", "cardSpades_5", "cardSpades_6", "cardSpades_7", "cardSpades_8", "cardSpades_9", "cardSpades_10", "cardSpades_J", "cardSpades_Q", "cardSpades_K"
        };

    }
}
