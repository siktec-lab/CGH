using CGH_Server.Networking;
using CGH_Server.Utility;

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
