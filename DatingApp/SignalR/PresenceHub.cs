using DatingApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace DatingApp.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker presenceTracker;

        public PresenceHub(PresenceTracker _presenceTracker)
        {
            presenceTracker = _presenceTracker;
        }
        public override async Task OnConnectedAsync()
        {
           var isOnline = await presenceTracker.UsersConnected(Context.User.GetUsername(), Context.ConnectionId);

            if (isOnline)
            {
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
            }

            var onlineUsers = await presenceTracker.GetAllUsersOnline();

            await Clients.Caller.SendAsync("GetAllOnlineUsers", onlineUsers);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline = await presenceTracker.UsersDisconnected(Context.User.GetUsername(), Context.ConnectionId);

            if (isOffline)
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            //var onlineUsers = await presenceTracker.GetAllUsersOnline();
            //await Clients.All.SendAsync("GetAllOnlineUsers", onlineUsers);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
