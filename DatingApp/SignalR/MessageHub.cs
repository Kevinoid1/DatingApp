using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.Migrations;
using DatingApp.Models;
using DatingApp.Repositories;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IDatingRepository datingRepository;
        private readonly IMapper mapper;
        private readonly IHubContext<PresenceHub> presenceContext;
        private readonly PresenceTracker presenceTracker;

        public MessageHub(IMessageRepository _messageRepository, IDatingRepository _datingRepository, IMapper _mapper,
                          IHubContext<PresenceHub> _presenceContext, PresenceTracker _presenceTracker)
        {
            messageRepository = _messageRepository;
            datingRepository = _datingRepository;
            mapper = _mapper;
            presenceContext = _presenceContext;
            presenceTracker = _presenceTracker;
        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var user = httpContext.Request.Query["user"].ToString();

            var groupName = GetGroupName(Context.User.GetUsername(), user);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var group = await AddToGroup(groupName);

            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);

            var messages = await messageRepository.GetMessageThreadAsync(Context.User.GetUsername(), user);

            await Clients.Caller.SendAsync("ReceivedMessageThread", messages);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveConnectionFromGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage (CreateMessageDto entity)
        {
            var username = Context.User.GetUsername();

            if (username.ToLower() == entity.RecipientUsername.ToLower()) throw new HubException("You cannot send messages to yourself");

            var sender = await datingRepository.GetUserByUsernameAsync(username);
            var recipient = await datingRepository.GetUserByUsernameAsync(entity.RecipientUsername);

            if (recipient == null) throw new HubException("User not found");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = entity.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var group = await messageRepository.GetGroupByName(groupName);

            if(group.Connections.Any(c => c.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }

            else //recipient not in the group
            {
                var connections = await presenceTracker.GetUserConnections(recipient.UserName); //remember we wired it up for the message hub to be connected when user is on messages. Thats why we are using the presence hub instead
                if(connections != null) //if the recipient is online
                {
                    await presenceContext.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { userId = sender.Id, knownAs = sender.KnownAs });
                }
            }

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
            {
               await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
            }

        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await messageRepository.GetGroupByName(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if(group == null)
            {
                group = new Group(groupName);

                messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);
            if (await messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to join");
        }

        private async Task<Group> RemoveConnectionFromGroup()
        {
            var group = await messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = await messageRepository.GetConnectionById(Context.ConnectionId);
            messageRepository.RemoveConnection(connection);
            if(await messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to leave group");
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
