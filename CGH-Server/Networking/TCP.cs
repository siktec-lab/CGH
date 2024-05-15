using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.Http;
using CGH_Server.Utility;
using CGH_Server.Networking.Messages;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace CGH_Server.Networking
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
                                    {
                                        // Validtae Message:
                                        if (clientMsg.msg == null)
                                        {
                                            SendMessage("Bad Request", "Error");
                                            continue;
                                        }

                                        // Parse:
                                        CreateGameRequest? createGameRequest = JsonConvert.DeserializeObject<CreateGameRequest>(clientMsg.msg);

                                        // Validate Game Request
                                        if (createGameRequest == null || createGameRequest.playerName == null || createGameRequest.gameType == null)
                                        {
                                            SendMessage("Bad Request", "Error");
                                            continue;
                                        }

                                        GameRoom gameRoom = new GameRoom();
                                        gameRoom.gameType = createGameRequest.gameType;
                                        gameRoom.AddPlayer(createGameRequest.playerName, createGameRequest.playerAvatar);

                                        //string fileNameCreate = gameRoom.gameType + "-" + gameRoom.roomCode + ".json";
                                        string pathTo = Globals.ServerPathToFile("GameLobbies", gameRoom.gameType + "-" + gameRoom.roomCode + ".json");
                                        if (!File.Exists(pathTo))
                                        {
                                            // Save Game Room:
                                            gameRoom.SaveGameToFile();

                                            Console.ResetColor();
                                            Console.Write("[");
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Console.Write("Server");
                                            Console.ResetColor();
                                            Console.WriteLine($"] {port}: msg: Game Room created successfully. purpose: created-game-room");

                                            // Add to address book:
                                            Globals.phoneBook.AddAddress(
                                                gameCode: gameRoom.roomCode,
                                                user: createGameRequest.playerName,
                                                client: tcpClient
                                            );

                                            CreateGameResponse createGameResponse = new CreateGameResponse();
                                            createGameResponse.From(gameRoom, createGameRequest.playerName);
                                            SendMessage(JsonConvert.SerializeObject(createGameResponse), "created-game-room");
                                        }
                                        else
                                        {
                                            Console.ResetColor();
                                            Console.Write("[");
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Console.Write("Server");
                                            Console.ResetColor();
                                            Console.WriteLine($"] {port}: msg: Game Room already created try again. purpose: Error");
                                            SendMessage("Game Room already created try again.", "Error");
                                        }
                                    }
                                    break;

                                case "joinPlayerToGame":
                                    {
                                        // Validtae Message:
                                        if (clientMsg.msg == null)
                                        {
                                            SendMessage("Bad Request", "Error");
                                            continue;
                                        }

                                        // Parse:
                                        JoinGameRequest? joinGameRequest = JsonConvert.DeserializeObject<JoinGameRequest>(clientMsg.msg);
                                        
                                        // Validate Game Request
                                        if (joinGameRequest == null || joinGameRequest.playerName == null || joinGameRequest.gameType == null || joinGameRequest.gameCode == 0)
                                        {
                                            SendMessage("Bad Request", "Error");
                                            continue;
                                        }

                                        // The game room:
                                        GameRoom? gameRoomToJoin = Globals.GetGameRoom(joinGameRequest.gameType, joinGameRequest.gameCode);
                                        if (gameRoomToJoin == null)
                                        {
                                            SendMessage("Game Room not found", "Error");
                                            continue;
                                        }

                                        // Make sure the game is not in play mode:
                                        if (gameRoomToJoin.gameStarted)
                                        {
                                            SendMessage("Game already started", "Error");
                                            continue;
                                        }

                                        // Add the player to the game room:
                                        gameRoomToJoin.AddPlayer(joinGameRequest.playerName, joinGameRequest.playerAvatar);

                                        // Save Game Room:
                                        gameRoomToJoin.SaveGameToFile();

                                        // Add to address book:
                                        Globals.phoneBook.AddAddress(
                                            gameCode: gameRoomToJoin.roomCode,
                                            user: joinGameRequest.playerName,
                                            client: tcpClient
                                        );
                                        
                                        //Response:
                                        JoinGameResponse joinGameResponse = new JoinGameResponse();
                                        joinGameResponse.From(gameRoomToJoin, joinGameRequest.playerName);

                                        //Report directly to the player:
                                        SendMessage(JsonConvert.SerializeObject(joinGameResponse), "joined-game-room");

                                        //Report to all:
                                        gameRoomToJoin.SendMessageToAll(
                                            msg     : JsonConvert.SerializeObject(joinGameResponse), 
                                            purpose : "broadcast-joined-game",
                                            exclude : new List<string> { joinGameRequest.playerName }
                                        );
                                    }
                                    break;
                                
                                case "isRoomAvailable":
                                    {
                                        // Validtae Message:
                                        if (clientMsg.msg == null)
                                        {
                                            SendMessage("Bad Request", "Error");
                                            continue;
                                        }

                                        // Normalize:
                                        string file = Globals.FilterGameRoomAndBuildPath(clientMsg.msg);
                                        
                                        // Validate Game Exists:
                                        if (File.Exists(Globals.ServerPathToFile("GameLobbies", file)))
                                        {
                                            SendMessage("True", "room-availability");
                                        }
                                        else
                                        {
                                            SendMessage("False", "room-availability");
                                        }
                                    }
                                    break;

                                case "removeFromGame":
                                    {
                                        // Validtae Message:
                                        if (clientMsg.msg == null)
                                        {
                                            SendMessage("Bad Request", "Error");
                                            continue;
                                        }

                                        // Parse:
                                        RemoveFromGameRequest? removeFromGameRequest = JsonConvert.DeserializeObject<RemoveFromGameRequest>(clientMsg.msg);

                                        // Validate Game Request
                                        if (
                                            removeFromGameRequest == null ||
                                            removeFromGameRequest.playerName == null ||
                                            removeFromGameRequest.playerName == "" ||
                                            removeFromGameRequest.gameType == null ||
                                            removeFromGameRequest.gameType == "None" ||
                                            removeFromGameRequest.gameCode == 0)
                                        {
                                            SendMessage("Bad Request", "Error");
                                            continue;
                                        }

                                        // The game room:
                                        GameRoom? gameRoomToJoin = Globals.GetGameRoom(removeFromGameRequest.gameType, removeFromGameRequest.gameCode);
                                        if (gameRoomToJoin == null)
                                        {
                                            SendMessage("", "removed-from-game-none");
                                            continue;
                                        }

                                        // The player: 
                                        Player? playerToRemove = gameRoomToJoin.GetPlayer(removeFromGameRequest.playerName);
                                        if (playerToRemove == null)
                                        {
                                            SendMessage("", "removed-from-game-none");
                                            continue;
                                        }

                                        // If the player is the host:
                                        if (playerToRemove.isHost)
                                        {
                                            // Remove the game room:
                                            File.Delete(Globals.ServerPathToFile("GameLobbies", Globals.FilterGameRoomAndBuildPath(removeFromGameRequest.gameType, removeFromGameRequest.gameCode)));
                                            
                                            // Let the players know:
                                            gameRoomToJoin.SendMessageToAll(
                                                msg: removeFromGameRequest.playerName,
                                                purpose: "broadcast-removed-from-game-host"
                                            );
                                    
                                            // Remove the players and from the address book:
                                            gameRoomToJoin.RemoveAllPlayers();
                                        }
                                        else
                                        {
                                            // Remove the player:
                                            gameRoomToJoin.RemovePlayer(removeFromGameRequest.playerName);

                                            // Save Game Room:
                                            gameRoomToJoin.SaveGameToFile();

                                            gameRoomToJoin.SendMessageToAll(
                                                msg: removeFromGameRequest.playerName,
                                                purpose: "broadcast-removed-from-game-player",
                                                exclude: new List<string> { removeFromGameRequest.playerName } // Probably not required but just in case
                                            );
                                        }
                                    }
                                    break;

                                case "deleteGame":
                                    {
                                        /*string fileNameDelete = clientMsg.msg + ".json";

                                        string fileLinesDelete = File.ReadAllText(Globals.baseDirectory + @"\GameLobbies\" + fileNameDelete);

                                        GameRoom tempGameRoomDelete = JsonConvert.DeserializeObject<GameRoom>(fileLinesDelete);
                                        for (int i = 0; i < tempGameRoomDelete.players.Count; i++)
                                        {
                                            Globals.ClientTCPS[i].SendMessage(tempGameRoomDelete.gameType + "-" + tempGameRoomDelete.roomCode, "gameDeleted");
                                        }

                                        File.Delete(Globals.baseDirectory + @"\GameLobbies\" + fileNameDelete);*/
                                    }
                                    break;
                                case "startGameRoom":
                                    {
                                        // Validtae Message:
                                        if (clientMsg.msg == null)
                                        {
                                            SendMessage("Bad Request", "Error");
                                            continue;
                                        }

                                        // Normalize:
                                        string file = Globals.FilterGameRoomAndBuildPath(clientMsg.msg);


                                        // The game room:
                                        GameRoom? gameRoomToSart = Globals.GetGameRoom(file);
                                        if (gameRoomToSart == null)
                                        {
                                            SendMessage("Game Room not found", "Error");
                                            continue;
                                        }

                                        // Start the game:
                                        gameRoomToSart.gameStarted = true;

                                        // Save Game Room:
                                        gameRoomToSart.SaveGameToFile();
                                        
                                        // Notify all players:
                                        gameRoomToSart.SendMessageToAll(
                                            msg: gameRoomToSart.roomCode.ToString(),
                                            purpose: "broadcast-start-game"
                                        );
                                    }
                                    break;

                                case "gameTurn":
                                    {
                                        // Validtae Message:
                                        if (clientMsg.msg == null || clientMsg.msg == "")
                                        {
                                            SendMessage("Bad Request", "Error");
                                            continue;
                                        }

                                        // Parse:
                                        GameRoundRequest? gameTurnRequest = JsonConvert.DeserializeObject<GameRoundRequest>(clientMsg.msg);

                                        // Validate Game Round Request
                                        if (gameTurnRequest == null || gameTurnRequest.playerName == "" || gameTurnRequest.gameType == "" || gameTurnRequest.gameCode == 0)
                                        {
                                            SendMessage("Bad Request", "Error");
                                            continue;
                                        }

                                        // The game room:
                                        GameRoom? gameRoom = Globals.GetGameRoom(gameTurnRequest.gameType, gameTurnRequest.gameCode);
                                        if (gameRoom == null)
                                        {
                                            SendMessage("Game Room not available", "Error");
                                            continue;
                                        }
                                        
                                        // Is the game started?
                                        if (!gameRoom.gameStarted)
                                        {
                                            SendMessage("Game not started", "Error");
                                        }

                                        WarRoundResponse? warRoundResponse = gameRoom.PlayerTurn(gameTurnRequest.playerName);

                                        if (warRoundResponse == null)
                                        {
                                            SendMessage("UnAuthorized Game Turn", "Error");
                                            continue;
                                        }

                                        // Save Game Room:
                                        gameRoom.SaveGameToFile();

                                        // Notify current player:
                                        this.SendMessage(JsonConvert.SerializeObject(warRoundResponse), "broadcast-game-turn");

                                        // Fliping Sides:
                                        warRoundResponse.FlipSides();
                                        
                                        // Notify all players:
                                        gameRoom.SendMessageToAll(
                                            msg: JsonConvert.SerializeObject(warRoundResponse),
                                            purpose: "broadcast-game-turn",
                                            exclude: new List<string> { gameTurnRequest.playerName }
                                        );
                                    }
                                    break;
                                    
                                case "disconnect":
                                    {
                                        Disconnect();
                                    }
                                    break;
                                case "Test":
                                    {
                                        Console.ResetColor();
                                        Console.Write("[");
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.Write("Server");
                                        Console.ResetColor();
                                        Console.WriteLine($"] {port}: {clientMsg.msg}");
                                        SendMessage("Hello back from your server!", "Test");
                                    }
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
