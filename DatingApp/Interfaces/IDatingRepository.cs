using DatingApp.Helpers;
using DatingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Interfaces
{
    public interface IDatingRepository
    {
        Task<PagedList<User>> GetAllUsers(UserParams userParams);

        Task<User> GetUser(int Id);
        Task<User> GetUserByUsernameAsync(string username);

        void Create<T>(T Entity) where T : class;

        Task<bool> SaveAll();

        //Task Update<T>(T Entity) where T: class;

        void Delete<T>(T Entity) where T : class;

        Task<Photo> GetPhoto(int id);

        Task<Photo> GetCurrentMainUserPhoto(int userId);
        Task<User> GetUser(string username);
    }
}
