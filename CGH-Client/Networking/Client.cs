using CGH_Client.Forms;
using CGH_Client.Utility;
using CGH_Client.Networking.Messages;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace CGH_Client.Networking
{
    public class Client
    {
        private const int BUFFER_SIZE = 10000000; //10MB

        public TcpClient client { get; set; }
        private Thread tcpThread;

        private string ip;
        private int port;
        public bool isListenToServer { get; set; }
        public bool isConnectedToServer { get; set; }
        public Client(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            isListenToServer = true;
        }

        //establish TCP connection
        public void Start()
        {
            tcpThread = new Thread(new ThreadStart(ListenToServer));
            tcpThread.IsBackground = true;
            tcpThread.Start();
        }
        public void Stop()
        {
            this.SendMessage("", "disconnect");
            Thread.Sleep(100);
            tcpThread.Abort();
        }

        //Listen to messages come from Server
        public void ListenToServer()
        {
            while (isListenToServer)
            {
                try
                {
                    using (client = new TcpClient())
                    {
                        client.Connect(ip, port);
                        if (client.Connected)
                            isConnectedToServer = true;
                        using (NetworkStream stream = client.GetStream())
                        {
                            while (isListenToServer)
                            {
                                try
                                {
                                    byte[] buffer = new byte[BUFFER_SIZE];
                                    int bytesRead;
                                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                                    {
                                        string msg = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                                        ServerMsg serverMsg = JsonConvert.DeserializeObject<ServerMsg>(msg);
                                        switch (serverMsg.purpose)
                                        {
                                            case "Connect Here":
                                                {
                                                    SendMessage("", "Connected");
                                                    isListenToServer = false;
                                                    Globals.ServerConnector = new Client(Globals.serverIP, int.Parse(serverMsg.msg));
                                                    Globals.ServerConnector.Start();
                                                }
                                                break;
                                            case "Connected To Router":
                                                {
                                                    SendMessage("", "Connect Me");
                                                }
                                                break;
                                            case "Error":
                                                {
                                                    MessageBox.Show(serverMsg.msg, serverMsg.purpose);
                                                }
                                                break;
                                            case "room-availability":
                                                {
                                                    if (Globals.currentScreen is EnterRoomCodeForm screen)
                                                    {
                                                        if (serverMsg.msg == "True" && screen.selectedRoomCode > 0)
                                                        {
                                                            screen.Invoke((MethodInvoker)delegate
                                                            {
                                                                // Open Select Player:
                                                                ChooseNameForm next = new ChooseNameForm(null);
                                                                next.withCode = screen.selectedRoomCode;
                                                                Globals.hostOrJoin = "JOIN";
                                                                screen.SwitchToForm(next);
                                                            });
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("הקוד שהזנת אינו תואם למשחקים קיימים", "Error");
                                                        }
                                                    }
                                                }
                                                break;
                                            case "created-game-room":
                                                {
                                                    CreateGameResponse createGameResponse = JsonConvert.DeserializeObject<CreateGameResponse>(serverMsg.msg);
                                                    if (createGameResponse == null)
                                                    {
                                                        MessageBox.Show("Error in creating game room", "Error");
                                                    }

                                                    // Only activate if we are in the ChooseNameForm
                                                    if (
                                                        Globals.hostOrJoin == "HOST" &&
                                                        Globals.currentScreen is ChooseNameForm screen)
                                                    {
                                                        screen.Invoke((MethodInvoker)delegate
                                                        {
                                                            if (createGameResponse.gameType == "War")
                                                            {
                                                                // Create the game room:
                                                                WarGameRoom gameRoom = new WarGameRoom();
                                                                gameRoom.AddPlayers(createGameResponse.players);
                                                                gameRoom.myPlayerIndex = createGameResponse.player;
                                                                Player me = gameRoom.GetPlayer(gameRoom.myPlayerIndex);
                                                                gameRoom.myPlayerName = me.Name;
                                                                gameRoom.gameType = createGameResponse.gameType;
                                                                gameRoom.roomCode = createGameResponse.roomCode;
                                                                Globals.gameRoom = gameRoom;

                                                            }
                                                            else if (createGameResponse.gameType == "Uno")
                                                            {
                                                                MessageBox.Show("Uno not implemented yet", "ToDo");
                                                            }
                                                            else if (createGameResponse.gameType == "Cheat")
                                                            {
                                                                MessageBox.Show("Cheat not implemented yet", "ToDo");
                                                            }
                                                            else
                                                            {
                                                                MessageBox.Show("Error unknown game type", "Error");
                                                            }

                                                            // Open Lobby:
                                                            GameLobbyForm lobby = new GameLobbyForm(null);
                                                            screen.SwitchToForm(lobby);
                                                        });
                                                    }
                                                }
                                                break;
                                            case "joined-game-room":
                                                {
                                                    JoinGameResponse joinGameResponse = JsonConvert.DeserializeObject<JoinGameResponse>(serverMsg.msg);
                                                    if (joinGameResponse == null)
                                                    {
                                                        MessageBox.Show("Error while joining game room", "Error");
                                                    }

                                                    // Only activate if we are in the ChooseNameForm
                                                    if (
                                                        Globals.hostOrJoin == "JOIN" &&
                                                        Globals.currentScreen is ChooseNameForm screen)
                                                    {
                                                        screen.Invoke((MethodInvoker)delegate
                                                        {
                                                            if (joinGameResponse.gameType == "War")
                                                            {
                                                                // Create the game room:
                                                                WarGameRoom gameRoom = new WarGameRoom();
                                                                gameRoom.AddPlayers(joinGameResponse.players);
                                                                gameRoom.myPlayerIndex = joinGameResponse.player;
                                                                Player me = gameRoom.GetPlayer(gameRoom.myPlayerIndex);
                                                                gameRoom.myPlayerName = me.Name;
                                                                gameRoom.roomCode = joinGameResponse.roomCode;
                                                                Globals.gameRoom = gameRoom;

                                                            }
                                                            else if (joinGameResponse.gameType == "Uno")
                                                            {
                                                                MessageBox.Show("Uno not implemented yet", "ToDo");
                                                            }
                                                            else if (joinGameResponse.gameType == "Cheat")
                                                            {
                                                                MessageBox.Show("Cheat not implemented yet", "ToDo");
                                                            }
                                                            else
                                                            {
                                                                MessageBox.Show("Error unknown game type", "Error");
                                                            }

                                                            // Open Lobby:
                                                            GameLobbyForm lobby = new GameLobbyForm(null);
                                                            screen.SwitchToForm(lobby);
                                                        });
                                                    }
                                                }
                                                break;
                                            case "broadcast-joined-game":
                                                {
                                                    JoinGameResponse joinGameResponse = JsonConvert.DeserializeObject<JoinGameResponse>(serverMsg.msg);
                                                    if (joinGameResponse == null)
                                                    {
                                                        MessageBox.Show("Error while joining game room", "Error");
                                                    } 
                                                    else
                                                    {
                                                        if (Globals.currentScreen is GameLobbyForm screen && Globals.gameRoom is BaseGameRoom gameRoom)
                                                        {
                                                            gameRoom.MergePlayers(joinGameResponse.players);
                                                            screen.Invoke((MethodInvoker)delegate
                                                            {
                                                                screen.RefreshLobby();
                                                            });
                                                        }
                                                    }
                                                }
                                                break;
                                            case "removed-from-game-none":
                                                {
                                                    // TODO: Do nothing
                                                }
                                                break;
                                            case "broadcast-removed-from-game-player":
                                                {
                                                    string playerName = serverMsg.msg;
                                                    if (playerName == "")
                                                    {
                                                        MessageBox.Show("Error Unexpected message while player was removed", "Error");
                                                    }
                                                    
                                                    // Remove player:
                                                    bool removed = false;
                                                    bool removedMe = false;
                                                    if (Globals.gameRoom != null)
                                                    {
                                                        if (Globals.gameRoom is WarGameRoom gameRoom)
                                                        {
                                                            removed = gameRoom.RemovePlayer(playerName);
                                                            if (playerName == gameRoom.myPlayerName)
                                                            {
                                                                removedMe = removed;
                                                            }
                                                        }
                                                        //TODO: Implement more games here.
                                                    }

                                                    if (removed)
                                                    {
                                                        if (Globals.currentScreen is GameLobbyForm screen)
                                                        {
                                                            screen.Invoke((MethodInvoker)delegate
                                                            {
                                                                screen.RefreshLobby();
                                                            });
                                                        }
                                                        else if (Globals.currentScreen is WarGameForm game)
                                                        {
                                                            game.Invoke((MethodInvoker)delegate
                                                            {
                                                                game.CloseAndBack();
                                                            });
                                                        }
                                                    }

                                                }
                                                break;
                                            case "broadcast-removed-from-game-host":
                                                {
                                                    
                                                    // Close and destroy the active game room:
                                                    if (Globals.gameRoom != null)
                                                    {
                                                        Globals.gameRoom = null;
                                                    }
                                                    
                                                    // Update UI
                                                    if (Globals.currentScreen is GameLobbyForm screen)
                                                    {
                                                        screen.Invoke((MethodInvoker)delegate
                                                        {
                                                            screen.CloseAndBack(); // Will not contact server because we destroyed the game room.
                                                        });
                                                    }
                                                    //TODO: implement for other screens:
                                                }
                                                break;
                                            case "broadcast-start-game":
                                                {
                                                    string roomCode = serverMsg.msg;
                                                    
                                                    if (roomCode == "" || Globals.gameRoom == null)
                                                    {
                                                        MessageBox.Show("Error Unexpected message while starting game", "Error");
                                                    }

                                                    if (
                                                        Globals.currentScreen is GameLobbyForm screen &&
                                                        Globals.gameRoom is BaseGameRoom room &&
                                                        room.roomCode.ToString() == roomCode
                                                        )
                                                    {
                                                        screen.Invoke((MethodInvoker)delegate
                                                        {
                                                            switch (room.gameType)
                                                            {
                                                                case "War":
                                                                    {
                                                                        WarGameForm gameForm = new WarGameForm();
                                                                        WarGameRoom gameRoom = (WarGameRoom)room;
                                                                        gameRoom.StartGame();
                                                                        screen.SwitchToForm(gameForm);
                                                                    }
                                                                    break;
                                                                case "Uno":
                                                                    {
                                                                        MessageBox.Show("Uno Game Not supported", "Error");
                                                                    }
                                                                    break;
                                                                case "Cheat":
                                                                    {
                                                                        MessageBox.Show("Cheat Game Not supported", "Error");
                                                                    }
                                                                    break;
                                                                // Any other:
                                                                default:
                                                                    {
                                                                        MessageBox.Show("Unknown game type", "Error");
                                                                    }
                                                                    break;
                                                            }
                                                        });
                                                    }
                                                }
                                                break;
                                                  
                                            case "broadcast-game-turn":
                                                {

                                                    WarRoundResponse createGameResponse = JsonConvert.DeserializeObject<WarRoundResponse>(serverMsg.msg);
                                                    if (createGameResponse == null)
                                                    {
                                                        MessageBox.Show("Error proccesing game round response", "Error");
                                                    }

                                                    if (
                                                        Globals.currentScreen is WarGameForm screen &&
                                                        Globals.gameRoom is WarGameRoom room &&
                                                        room.roomCode == createGameResponse.roomCode
                                                        )
                                                    {
                                                        screen.Invoke((MethodInvoker)delegate
                                                        {
                                                            screen.PerfomTurn(createGameResponse);
                                                        });
                                                    } else
                                                    {
                                                        MessageBox.Show("Error Unexpected message while game turn", "Error");
                                                    }
                                                }
                                                break;

                                            case "Test":
                                                MessageBox.Show(serverMsg.msg);
                                                break;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Server is away trying to reconnect again in 5 seconds");
                    Thread.Sleep(5000);
                }
            }
            this.SendMessage("", "disconnect");
            Thread.Sleep(100);
            client.Close();
        }

        //send message over TCP
        public void SendMessage(string msg, string purpose)
        {
            // check if client is disposed:
            if (client == null || client.Connected == false)
                    return;
            try
            {
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    byte[] cmdArr = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new ServerMsg() { purpose = purpose, msg = msg }));
                    stream.Write(cmdArr, 0, cmdArr.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
