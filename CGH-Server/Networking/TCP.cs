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

        public TcpClient? client;
        private Thread? listenerThread;
        private int port;
        string clientIp = "Unknown";
        bool isCreated = false;
        TcpListener? listener;
        TcpClient? tcpClient;
        Protect protect;

        /*Constructor*/
        public TCP(int port, Protect protect)
        {
            this.port = port;
            this.protect = protect;
            Start();
        }
        
        public TCP(int port, bool encrypted = true)
        {
            this.port = port;
            this.protect = new Protect(encrypted);
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
                client = listener?.AcceptTcpClient();
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        // Handles communication with a client
        private void HandleClientComm(object clientObj)
        {
            // An encryption ecoder and decoder
            Protect Encryptor = new Protect();
            
            while (true)
            {
                tcpClient = (TcpClient)clientObj;
                using (NetworkStream clientStream = tcpClient.GetStream())
                {
                    // Client IP:
                    clientIp = Convert.ToString(((IPEndPoint)client.Client.RemoteEndPoint).Address) ?? "Unknown";

                    // Debug:
                    Globals.ServerDebug("Server", $"Player {clientIp} Is Connected to Port {port}");

                    // Initialize a buffer:
                    byte[] buffer = new byte[BUFFER_SIZE];
                    int bytesRead;

                    try
                    {
                        // Wait for the client to send data:
                        while ((bytesRead = clientStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            string msgReceived = "";
                            // Decrypt the message:
                            if (this.protect.HasKey())
                            {
                                try
                                {
                                    msgReceived = protect.Decrypt(Encoding.ASCII.GetString(buffer, 0, bytesRead));
                                }
                                catch (Exception)
                                {
                                    msgReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                                }
                            }
                            else
                            {
                                msgReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            }

                            ClientMsg clientMsg = JsonConvert.DeserializeObject<ClientMsg>(msgReceived) ?? new ClientMsg();

                            // Debug:
                            Globals.ServerDebug("Server", $"Player {clientIp}:{port} Sent ->  purpose: {clientMsg.purpose}, msg: {clientMsg.msg}");
                            
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
                                            
                                            // Debug:
                                            Globals.ServerDebug("Server", $"Player {clientIp}:{port} -> Created Game Room { gameRoom.gameType + "-" + gameRoom.roomCode }");
                                            
                                            // Add to address book:
                                            Globals.phoneBook.AddAddress(
                                                gameCode: gameRoom.roomCode,
                                                user: createGameRequest.playerName,
                                                transport: this
                                            );

                                            CreateGameResponse createGameResponse = new CreateGameResponse();
                                            createGameResponse.From(gameRoom, createGameRequest.playerName);
                                            SendMessage(JsonConvert.SerializeObject(createGameResponse), "created-game-room");
                                        }
                                        else
                                        {
                                            // Debug:
                                            Globals.ServerDebug("Server", $"Player {clientIp}:{port} -> Game Room '{gameRoom.gameType + "-" + gameRoom.roomCode}' already created try again");

                                            // Send Error:
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
                                        if (gameRoomToJoin.gameStarted || gameRoomToJoin.gameEnded)
                                        {
                                            SendMessage("Game Room is in play mode or done", "Error");
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
                                            transport: this
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
                                        GameRoom? gameRoom = Globals.GetGameRoom(removeFromGameRequest.gameType, removeFromGameRequest.gameCode);
                                        if (gameRoom == null)
                                        {
                                            SendMessage("", "removed-from-game-none");
                                            continue;
                                        }

                                        // The player: 
                                        Player? playerToRemove = gameRoom.GetPlayer(removeFromGameRequest.playerName);
                                        if (playerToRemove == null)
                                        {
                                            SendMessage("", "removed-from-game-none");
                                            continue;
                                        }

                                        // If the player is the host:
                                        if (playerToRemove.isHost)
                                        {
                                            // Remove the game room:
                                            gameRoom.DeleteGameFile();

                                            // Let the players know:
                                            gameRoom.SendMessageToAll(
                                                msg: removeFromGameRequest.playerName,
                                                purpose: "broadcast-removed-from-game-host",
                                                exclude: new List<string> { removeFromGameRequest.playerName }
                                            );

                                            // Remove the players and from the address book:
                                            gameRoom.RemoveAllPlayers();
                                        }
                                        else
                                        {
                                            // Remove the player:
                                            gameRoom.RemovePlayer(removeFromGameRequest.playerName);

                                            // Save Game Room:
                                            gameRoom.SaveGameToFile();

                                            gameRoom.SendMessageToAll(
                                                msg: removeFromGameRequest.playerName,
                                                purpose: "broadcast-removed-from-game-player",
                                                exclude: new List<string> { removeFromGameRequest.playerName } // Probably not required but just in case
                                            );
                                        }
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

                                        if (gameRoomToSart.gameStarted || gameRoomToSart.gameEnded)
                                        {
                                            SendMessage("Game Room already started or ended", "Error");
                                            continue;
                                        }

                                        // Start the game:
                                        gameRoomToSart.gameStarted = true;
                                        gameRoomToSart.gameEnded = false;

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
                                            continue;
                                        }

                                        // Is the game ended?
                                        if (gameRoom.gameEnded)
                                        {
                                            SendMessage("Game already ended", "Error");
                                            continue;
                                        }

                                        // Perform the game turn:
                                        WarRoundResponse? warRoundResponse = gameRoom.PlayerTurn(gameTurnRequest.playerName);
                                        if (warRoundResponse == null) // Will mostly mean that the player is not in the game!
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

                                        // Finaly if the game is over delete it:
                                        if (warRoundResponse.isGameOver)
                                        {
                                            gameRoom.RemoveAllPlayers(); // Will also remove from phone book!
                                            gameRoom.DeleteGameFile();
                                        }
                                    }
                                    break;
                                    
                                case "disconnect":
                                    {
                                        Disconnect();
                                    }
                                    break;
                                case "Test":
                                    {
                                        // Debug:
                                        Globals.ServerDebug("Server", $"Test {clientIp}:{port} -> Message {clientMsg.msg}");
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
            // Debug:
            Globals.ServerDebug("Server", $"Player {clientIp}:{port} -> Disconnected"); 

            //Update Port list to set this port as available
            for (int i = 0; i < Globals.RouterServer?.ports.Count; i++)
            {
                if (Globals.RouterServer.ports[i].port == port)
                {
                    Globals.RouterServer.ports[i].isAvailable = true;
                }
            }
            tcpClient?.Close();
            client?.Close();
            Thread.Sleep(500);
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
                    
                    // Serialize the message:
                    string json = JsonConvert.SerializeObject(new ClientMsg() { purpose = purpose, msg = msg });

                    //Encrypt:
                    if (this.protect.HasKey())
                    {
                        json = this.protect.Encrypt(json);
                    }

                    // Send Bytes:
                    byte[] msgArr = Encoding.ASCII.GetBytes(json);
                    stream.Write(msgArr, 0, msgArr.Length);
                }

            }
            catch { }
        }
    }
}
