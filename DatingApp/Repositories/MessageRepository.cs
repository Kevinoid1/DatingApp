using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DatingAppContext context;
        private readonly IMapper mapper;

        public MessageRepository(DatingAppContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            return await context.Messages.FindAsync(id);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currentUsername, string recipientUsername)
        {
            var messages = await context.Messages
                            .Include(m => m.Sender).ThenInclude(p => p.Photos) //eagerly load photos for sender
                            .Include(m => m.Recipient).ThenInclude(p => p.Photos) // eagerly load photos for recipient
                            .Where(m => m.RecipientUsername == currentUsername && m.SenderUsername == recipientUsername && m.RecipientDeleted == false
                                || m.RecipientUsername == recipientUsername && m.SenderUsername == currentUsername && m.SenderDeleted == false
                            )
                            .OrderBy(m => m.DateSent)
                            .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();

            if(unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.DateRead = DateTime.Now; //mark the messages as read
                }

                await context.SaveChangesAsync();
            }

            return mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<PagedList<MessageDto>> GetUserMessagesAsync(MessageParams messageParams)
        {
           var query = context.Messages.OrderByDescending(m => m.DateSent).AsQueryable();

            //commented out to try a  different approach for rendering the messages
            //query = messageParams.Container switch
            //{
            //    "Inbox" => query.Where(m => m.RecipientUsername.ToLower() == messageParams.Username.ToLower()),
            //    "Outbox" => query.Where(m => m.SenderUsername.ToLower() == messageParams.Username.ToLower()),
            //    _ => query.Where(m => m.RecipientUsername.ToLower() == messageParams.Username.ToLower() && m.DateRead == null)
            //};

            query = query.Where(m => m.RecipientUsername.ToLower() == messageParams.Username.ToLower() && m.RecipientDeleted == false 
                            || m.SenderUsername.ToLower() == messageParams.Username.ToLower() && m.SenderDeleted == false);


            var source = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider, new {username = messageParams.Username})
                               ;

            var msgLst = new List<MessageDto>();
            foreach (var msg in await source.ToListAsync())
            {
                if (!msgLst.Any(m => (m.RecipientUsername == msg.RecipientUsername && m.SenderUsername == msg.SenderUsername) && m.DateSent > msg.DateSent
                  || (m.RecipientUsername == msg.SenderUsername && m.SenderUsername == msg.RecipientUsername && m.DateSent > msg.DateSent))
                    )
                {
                    msgLst.Add(msg);
                }
            }




            return PagedList<MessageDto>.Create(msgLst, messageParams.PageNumber, messageParams.PageSize);
        }

        public void RemoveMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
