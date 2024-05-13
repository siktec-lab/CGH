using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.Http;
using CGH_Server.Utility;
using Newtonsoft.Json;

namespace CRS_Server.Networking
{
    public class TCP
    {
        private const int BUFFER_SIZE = 10000000;//10MB

        TcpClient client;
        private Thread listenerThread;
        private int port;
        string clientIp;
        bool isCreated = false;
        TcpListener listener;
        TcpClient tcpClient;

        /*Constructor*/
        public TCP(int port)
        {
            this.port = port;
            Start();
        }

        // Establishes TCP connection
        private void Start()
        {
            if (!isCreated)
            {
                listenerThread = new Thread(new ThreadStart(ListenForClients));
                listenerThread.IsBackground = true;
                listenerThread.Start();
            }
        }

        // Listens for incoming clients and starts a new thread for each client
        private void ListenForClients()
        {
            if (!isCreated)
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                isCreated = true;
            }
            while (true)
            {
                client = listener.AcceptTcpClient();
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        // Handles communication with a client
        private void HandleClientComm(object clientObj)
        {
            while (true)
            {
                tcpClient = (TcpClient)clientObj;
                using (NetworkStream clientStream = tcpClient.GetStream())
                {
                    clientIp = Convert.ToString(((IPEndPoint)client.Client.RemoteEndPoint).Address);
                    Console.ResetColor();
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Server");
                    Console.ResetColor();
                    Console.WriteLine($"] {port}: Connected");
                    byte[] buffer = new byte[BUFFER_SIZE];
                    int bytesRead;

                    try
                    {
                        while ((bytesRead = clientStream.Read(buffer, 0, buffer.Length)) != 0)
                        {

                            Thread.Sleep(10);
                            string msgReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            Thread.Sleep(10);

                            List<string> gameCards = new List<string>();

                            if (msgReceived.Contains("}{"))
                            {
                                continue;
                            }

                            ClientMsg clientMsg = JsonConvert.DeserializeObject<ClientMsg>(msgReceived);

                            Console.ResetColor();
                            Console.Write("[");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("Server");
                            Console.ResetColor();
                            Console.WriteLine($"] {port}: purpose: {clientMsg.purpose}, msg: {clientMsg.msg}");

                            // Handle the message based on its purpose
                            switch (clientMsg.purpose)
                            {
                                case "createGameLobby":

                                    string[] msgParts = clientMsg.msg.Split("{0}");

                                    string[] gameID = msgParts[0].Split("-");
                                    string gameType = gameID[0];
                                    string gameCode = gameID[0];
                                    string playerStr = msgParts[1];

                                    Player player = JsonConvert.DeserializeObject<Player>(playerStr);

                                    GameRoom gameRoom = new GameRoom()
                                    {
                                        gameType = gameType,
                                        roomCode = int.Parse(gameCode),
                                        cardPlayed = "",
                                        currentPlayerTurn = ""
                                    };
                                    gameRoom.players.Add(player);

                                    string fileNameCreate = gameRoom.gameType + "-" + gameRoom.roomCode + ".json";

                                    if (!File.Exists(Globals.baseDirectory + @"\GameLobbies\" + fileNameCreate))
                                    {
                                        File.WriteAllText(Globals.baseDirectory + @"\GameLobbies\" + fileNameCreate, clientMsg.msg);

                                        Console.ResetColor();
                                        Console.Write("[");
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.Write("Server");
                                        Console.ResetColor();
                                        Console.WriteLine($"] {port}: msg: Game Room created successfully. purpose: Success");
                                        SendMessage("", "gameCreated");
                                    }

                                    else
                                    {
                                        Console.ResetColor();
                                        Console.Write("[");
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.Write("Server");
                                        Console.ResetColor();
                                        Console.WriteLine($"] {port}: msg: Game Room already created try again. purpose: Error");
                                        SendMessage("", "gameNotCreated");
                                    }

                                    break;

                                case "joinPlayerToGame":

                                    Player player = JsonConvert.DeserializeObject<Player>(clientMsg.msg);
                                    bool playerFound = false;

                                    string fileNameJoin = player.gameID + ".json";

                                    string fileLines = File.ReadAllText(Globals.baseDirectory + @"\GameLobbies\" + fileNameJoin);
                                    GameRoom tempGameRoomJoin = JsonConvert.DeserializeObject<GameRoom>(fileLines);

                                    for (int i = 0; i < tempGameRoomJoin.players.Count; i++)
                                    {
                                        if (tempGameRoomJoin.players[i].Name == player.Name)
                                        {
                                            playerFound = true;
                                            tempGameRoomJoin.players[i].isDisconnected = false;
                                        }
                                    }

                                    if (!playerFound)
                                    {
                                        tempGameRoomJoin.players.Add(player);
                                    }

                                    string newFileLines = JsonConvert.SerializeObject(tempGameRoomJoin);
                                    File.WriteAllText(Globals.baseDirectory + @"\GameLobbies\" + fileNameJoin, newFileLines);

                                    break;

                                case "isRoomAvailable":

                                    string fileNameCheckRoom = clientMsg.msg + ".json";

                                    if (File.Exists(Globals.baseDirectory + @"\GameLobbies\" + fileNameCheckRoom))
                                    {
                                        SendMessage("True", "isRoomAvailable");
                                    }

                                    else
                                    {
                                        SendMessage("False", "isRoomAvailable");
                                    }

                                    break;

                                case "getGameLobby":

                                    string fileLinesGGL = File.ReadAllText(Globals.baseDirectory + @"\GameLobbies\" + clientMsg.msg + ".json");

                                    SendMessage(fileLinesGGL, "returnedGameLobby");

                                    break;

                                case "removeFromGame":

                                    Player tempPlayerRemove = JsonConvert.DeserializeObject<Player>(clientMsg.msg);

                                    string fileNameRemove = tempPlayerRemove.gameID + ".json";

                                    string fileLinesRemove = File.ReadAllText(Globals.baseDirectory + @"\GameLobbies\" + fileNameRemove);
                                    GameRoom tempGameRoomRemove = JsonConvert.DeserializeObject<GameRoom>(fileLinesRemove);

                                    if (tempPlayerRemove.isHost)
                                    {
                                        File.Delete(Globals.baseDirectory + @"\GameLobbies\" + fileNameRemove);
                                        for (int i = 0; i < tempGameRoomRemove.players.Count; i++)
                                        {
                                            Globals.ClientTCPS[i].SendMessage(tempPlayerRemove.gameID, "hostDisconnected");
                                        }
                                    }

                                    else
                                    {
                                        for (int i = 0; i < tempGameRoomRemove.players.Count; i++)
                                        {
                                            if (tempGameRoomRemove.players[i].Name == tempPlayerRemove.Name)
                                            {
                                                tempGameRoomRemove.players[i].isDisconnected = true;
                                            }
                                        }

                                        string newFileLinesRemove = JsonConvert.SerializeObject(tempGameRoomRemove);
                                        File.WriteAllText(Globals.baseDirectory + @"\GameLobbies\" + fileNameRemove, newFileLinesRemove);

                                    }

                                    break;

                                case "deleteGame":

                                    string fileNameDelete = clientMsg.msg + ".json";

                                    string fileLinesDelete = File.ReadAllText(Globals.baseDirectory + @"\GameLobbies\" + fileNameDelete);

                                    GameRoom tempGameRoomDelete = JsonConvert.DeserializeObject<GameRoom>(fileLinesDelete);
                                    for (int i = 0; i < tempGameRoomDelete.players.Count; i++)
                                    {
                                        Globals.ClientTCPS[i].SendMessage(tempGameRoomDelete.gameType + "-" + tempGameRoomDelete.roomCode, "gameDeleted");
                                    }

                                    File.Delete(Globals.baseDirectory + @"\GameLobbies\" + fileNameDelete);

                                    break;

                                case "gameStarted":

                                    string fileLinesGameStart = File.ReadAllText(Globals.baseDirectory + @"\GameLobbies\" + clientMsg.msg + ".json");

                                    GameRoom tempGameStart = JsonConvert.DeserializeObject<GameRoom>(fileLinesGameStart);

                                    for (int i = 0; i < tempGameStart.players.Count; i++)
                                    {
                                        Globals.ClientTCPS[i].SendMessage(tempGameStart.gameType + "-" + tempGameStart.roomCode, "gameStarted");
                                        Thread.Sleep(100);
                                    }

                                    break;

                                case "changeSelectedCard":

                                    Random r = new Random();
                                    int randomCard = r.Next(0, Globals.gameCards.Count);

                                    string[] msgParts = clientMsg.msg.Split("{0}");

                                    string playerName = msgParts[0];
                                    string gameID = msgParts[1];
                                    string selectedCard = Globals.gameCards[randomCard];

                                    Globals.gameCards.Remove(selectedCard);

                                    string fileLinesChangeSelectedCard = File.ReadAllText(Globals.baseDirectory + @"\GameLobbies\" + gameID + ".json");

                                    GameRoom tempGameChangeSelectedCard = JsonConvert.DeserializeObject<GameRoom>(fileLinesChangeSelectedCard);

                                    for (int i = 0; i < tempGameChangeSelectedCard.players.Count; i++)
                                    {
                                        if (tempGameChangeSelectedCard.players[i].Name == playerName)
                                        {
                                            tempGameChangeSelectedCard.players[i].selectedCard = selectedCard;
                                            if (i+1 >= tempGameChangeSelectedCard.players.Count)
                                            {
                                                tempGameChangeSelectedCard.currentPlayerTurn = tempGameChangeSelectedCard.players[0].Name;
                                            }

                                            else
                                            {
                                                tempGameChangeSelectedCard.currentPlayerTurn = tempGameChangeSelectedCard.players[i+1].Name;
                                            }
                                        }
                                    }

                                    gameCards.Add(selectedCard);

                                    string newFileLinesChanged = JsonConvert.SerializeObject(tempGameChangeSelectedCard);
                                    File.WriteAllText(Globals.baseDirectory + @"\GameLobbies\" + gameID + ".json", newFileLinesChanged);

                                    break;

                                case "checkWinner":



                                    break;

                                case "Test":
                                    Console.ResetColor();
                                    Console.Write("[");
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write("Server");
                                    Console.ResetColor();
                                    Console.WriteLine($"] {port}: {clientMsg.msg}");
                                    SendMessage("Hello back from your server!", "Test");
                                    break;
                            }

                        }
                        Thread.Sleep(10);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        Disconnect();
                        break;
                    }
                }
            }
        }

        public void Disconnect()
        {

            //Update Port list to set this port as available
            Console.ResetColor();
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Server");
            Console.ResetColor();
            Console.WriteLine($"] {port}: {clientIp} Disconnected");
            for (int i = 0; i < Globals.RouterServer.ports.Count; i++)
            {
                if (Globals.RouterServer.ports[i].port == port)
                {
                    Globals.RouterServer.ports[i].isAvailable = true;
                }
            }
            tcpClient.Close();
            client.Close();
            Thread.Sleep(1000);
        }
        //sends Message over TCP
        public void SendMessage(string msg, string purpose)
        {
            if (client == null)
                return;
            try
            {
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    byte[] msgArr = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new ClientMsg() { purpose = purpose, msg = msg }));
                    stream.Write(msgArr, 0, msgArr.Length);
                }

            }
            catch { }
        }
    }
}
