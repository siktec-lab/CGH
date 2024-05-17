using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CGH_Server.Networking
{
    public class PhoneBook
    {
        // A dictionary that maps a string to TcpClient
        private Dictionary<string, TCP> phoneBook;
        
        public PhoneBook() {
            this.phoneBook = new Dictionary<string, TCP>();
        }
        
        public bool AddAddress(int gameCode, string user, TCP transport)
        {
            return phoneBook.TryAdd(gameCode.ToString() + "-" + user, transport);
        }

        public bool RemoveAddress(int gameCode, string user)
        {
            return phoneBook.Remove(gameCode.ToString() + "-" + user);
        }

        public TCP? GetAddress(int gameCode, string user)
        {
            _ = phoneBook.TryGetValue(gameCode.ToString() + "-" + user, out TCP? transport);
            return transport;
        }

        public bool ContainsAddress(int gameCode, string user)
        {
            return phoneBook.ContainsKey(gameCode.ToString() + "-" + user);
        }
    }
}
