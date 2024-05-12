using CGH_Server.Utility;
using CRS_Server.Networking;

namespace CGH_Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            Globals.RouterServer = new Router();

            Console.ReadLine();
        }
    }
}
