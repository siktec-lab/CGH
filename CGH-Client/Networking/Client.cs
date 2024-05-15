using CGH_Client.Forms;
using CGH_Client.Utility;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace CRS_Client.Networking
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
                                                SendMessage("", "Connected");
                                                isListenToServer = false;
                                                Globals.ServerConnector = new Client(Globals.serverIP, int.Parse(serverMsg.msg));
                                                Globals.ServerConnector.Start();
                                                break;
                                            case "Connected To Router":
                                                SendMessage("", "Connect Me");
                                                break;

                                            case "gameCreated":

                                                Globals.isGameCreated = true;
                                                Globals.gameID = serverMsg.msg;
                                                string[] msgP = Globals.gameID.Split('-');
                                                Globals.gameCode = int.Parse(msgP[1]);

                                                break;

                                            case "isRoomAvailable":

                                                if (serverMsg.msg == "True")
                                                {
                                                    Globals.isRoomAvailable = "True";
                                                    Thread.Sleep(1000);
                                                    Globals.isRoomAvailable = "waiting";
                                                }

                                                else
                                                {
                                                    Globals.isRoomAvailable = "False";
                                                    Thread.Sleep(1000);
                                                    Globals.isRoomAvailable = "waiting";
                                                }

                                                break;

                                            case "returnedGameLobby":

                                                Globals.globalGameRoom = JsonConvert.DeserializeObject<GameRoom>(serverMsg.msg);

                                                break;

                                            case "hostDisconnected":

                                                if (Globals.gameChoosed + "-" + Globals.gameCode == serverMsg.msg)
                                                {
                                                    Environment.Exit(0);
                                                }

                                                break;

                                            case "gameDeleted":

                                                if (Globals.gameChoosed + "-" + Globals.gameCode == serverMsg.msg)
                                                {
                                                    Environment.Exit(0);
                                                }

                                                break;

                                            case "gameStarted":
                                                if (Globals.gameChoosed + "-" + Globals.gameCode == serverMsg.msg)
                                                {
                                                    Globals.gameLobbyForm.Invoke((MethodInvoker)delegate
                                                    {
                                                        Globals.gameLobbyForm.isRefreshing = false;
                                                        Globals.gameLobbyForm.Hide();
                                                    });

                                                    // Create and show the main game form on the UI thread
                                                    Globals.gameLobbyForm.Invoke((MethodInvoker)delegate
                                                    {
                                                        MainGameForm mainGameForm = new MainGameForm();
                                                        mainGameForm.Show();
                                                    });
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
                    MessageBox.Show("Error: " + e);
                    Thread.Sleep(1000);
                }
            }
            client.Close();
        }

        //send message over TCP
        public void SendMessage(string msg, string purpose)
        {
            if (client == null)
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
