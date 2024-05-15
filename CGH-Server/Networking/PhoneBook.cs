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
        private Dictionary<string, TcpClient> phoneBook;

        public PhoneBook() {
            this.phoneBook = new Dictionary<string, TcpClient>();
        }
        
        public bool AddAddress(int gameCode, string user, TcpClient client)
        {
            return phoneBook.TryAdd(gameCode.ToString() + "-" + user, client);
        }

        public bool RemoveAddress(int gameCode, string user)
        {
            return phoneBook.Remove(gameCode.ToString() + "-" + user);
        }

        public TcpClient? GetAddress(int gameCode, string user)
        {
            _ = phoneBook.TryGetValue(gameCode.ToString() + "-" + user, out TcpClient? client);
            return client;
        }

        public bool ContainsAddress(int gameCode, string user)
        {
            return phoneBook.ContainsKey(gameCode.ToString() + "-" + user);
        }
    }
}
