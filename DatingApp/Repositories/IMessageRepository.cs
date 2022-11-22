using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Repositories
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void RemoveMessage(Message message);
        Task<Message> GetMessageAsync(int id);
        Task<PagedList<MessageDto>> GetUserMessagesAsync(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currentUsername, string recipientUsername);
        Task<bool> SaveAllAsync();
    }
}
