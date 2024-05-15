using CGH_Client.Forms;
using CRS_Client.Networking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGH_Client.Utility
{
    public static class Globals
    {

        public static string baseDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        public static string gameChoosed;
        public static int gameCode;
        public static string hostOrJoin;
        public static string gameID;

        public static Image charImgSelected;
        public static int charTagSelected;
        public static string charName;

        public static Client ServerConnector;
        public static string serverIP = "127.0.0.1";

        public static GameRoom globalGameRoom;

        public static string isRoomAvailable = "waiting";

        public static GameLobbyForm gameLobbyForm;
        public static bool isGameCreated = false;

    }
}
