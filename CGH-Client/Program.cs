using CGH_Client.Forms;
using CGH_Client.Utility;
using CRS_Client.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGH_Client
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Globals.ServerConnector = new Client(Globals.serverIP, 1000);
            Globals.ServerConnector.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SelectGameForm());
        }
    }
}
