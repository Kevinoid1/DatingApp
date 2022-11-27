using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

        public Task<bool> UsersConnected(string username, string connectionId)
        {
            var isOnline = false;
            //Dictionary is not thread safe. so i lock it to ensure only one person can access it at a time
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                    OnlineUsers[username].Add(connectionId);

                else
                {
                    OnlineUsers.Add(username, new List<string> { connectionId });
                    isOnline= true;
                }

            }
            
            return Task.FromResult(isOnline);
        }

        public Task<bool> UsersDisconnected(string username, string connectionId)
        {
            var isOffline = false;
            lock (OnlineUsers)
            {
                if(!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                OnlineUsers[username].Remove(connectionId);

                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline= true;
                }
            }

            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetAllUsersOnline()
        {
            string[] usersOnline;
            lock (OnlineUsers)
            {
                usersOnline = OnlineUsers.OrderBy(o => o.Key).Select(o => o.Key).ToArray();
            }

            return Task.FromResult(usersOnline);
        }

        public Task<List<string>> GetUserConnections(string username)
        {
            List<string> connections;
            lock (OnlineUsers)
            {
                connections = OnlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connections);
        }
    }
}
