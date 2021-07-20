using DatingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Repositories
{
    public interface IDatingRepository
    {
        Task<IEnumerable<User>> GetAllUsers();

        Task<User> GetUser(int Id);

        void Create<T>(T Entity) where T: class;

        Task<bool> SaveAll();

        //Task Update<T>(T Entity) where T: class;

        void Delete<T>(T Entity) where T:class;

        Task<Photo> GetPhoto(int id);

        Task<Photo> GetCurrentMainUserPhoto(int userId);
    }
}
