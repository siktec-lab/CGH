using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using CGH_Server.Utility;

namespace CRS_Server.Networking
{
    public class Router
    {
        private const int BUFFER_SIZE = 2000000;//2MB

        TcpClient client;
        private TcpListener listener;
        private Thread listenerThread;
        private int port;
        string clientIp;
        bool isAvailable;
        public List<Port> ports { get; set; }


        /*Constructor*/
        public Router()
        {
            ports = new List<Port>();
            ports.Add(new Port(1001));
            port = 1000;
            Globals.ClientTCPS = new List<TCP>();
            Start();
        }

        // Establishes TCP connection
        private void Start()
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            listenerThread = new Thread(new ThreadStart(ListenForClients));
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        // Listens for incoming clients and starts a new thread for each client
        private void ListenForClients()
        {
            while (true)
            {
                client = listener.AcceptTcpClient();
                Thread clientThread = new Thread(new ThreadStart(HandleClientComm));
                clientThread.Start();
            }
        }

        // Handles communication with a client
        private void HandleClientComm()
        {
            TcpClient tcpClient = client;
            using (NetworkStream clientStream = tcpClient.GetStream())
            {
                clientIp = Convert.ToString(((IPEndPoint)client.Client.RemoteEndPoint).Address);
                Console.ResetColor();
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Router");
                Console.ResetColor();
                Console.WriteLine($"] {clientIp}: Looking for port");
                byte[] buffer = new byte[BUFFER_SIZE];
                int bytesRead;
                try
                {
                    SendMessage("", "Connected To Router");
                    while ((bytesRead = clientStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        string msgReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        ClientMsg clientMsg = JsonSerializer.Deserialize<ClientMsg>(msgReceived);
                        if (clientMsg.purpose == "Connect Me")
                        {
                            for (int i = 0; i < ports.Count; i++)
                            {
                                if (ports[i].isAvailable)
                                {
                                    SendMessage(Convert.ToString(1001 + i), "Connect Here");
                                    ports[i].isAvailable = false;
                                    Console.ResetColor();
                                    Console.Write("[");
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.Write("Router");
                                    Console.ResetColor();
                                    Console.WriteLine($"] Connected {clientIp} To {1001 + i}");
                                    try
                                    {
                                        if (ports[i + 1] == null)
                                        {
                                            TCP clientTcp = new TCP(1001 + i);
                                            Globals.ClientTCPS.Add(clientTcp);
                                            ports.Add(new Port(1001 + i));
                                            break;
                                        }
                                        for (int j = i + 1; !ports[j].isAvailable; j++)
                                        {
                                            if (ports.Count == j + 1)
                                            {
                                                TCP clientTcp = new TCP(1001 + i);
                                                Globals.ClientTCPS.Add(clientTcp);
                                                ports.Add(new Port(1001 + j + 1));
                                                break;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        TCP clientTcp = new TCP(1001 + i);
                                        Globals.ClientTCPS.Add(clientTcp);
                                        ports.Add(new Port(1001 + i));
                                        break;
                                    }
                                    break;
                                }
                            }
                        }
                        else if (clientMsg.purpose == "Connected")
                        {
                            client.Close();
                        }
                    }
                }
                catch { }
            }
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
                    byte[] msgArr = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(new ClientMsg() { purpose = purpose, msg = msg }));
                    stream.Write(msgArr, 0, msgArr.Length);
                }

            }
            catch { }
        }

    }
}
