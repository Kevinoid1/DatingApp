using DatingApp.Data;
using DatingApp.Helpers;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Repositories
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DatingAppContext _context;

        public DatingRepository(DatingAppContext context)
        {
            _context = context;
        }

        public void Create<T>(T Entity) where T : class
        {
            _context.Set<T>().Add(Entity);
        }

        public void Delete<T>(T Entity) where T : class
        {
            _context.Set<T>().Remove(Entity);
        }

        public async Task<User> GetUser(int Id)
        {
            return await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == Id);
        }

        public async Task<PagedList<User>> GetAllUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(u => u.Id != userParams.UserId && u.Gender == userParams.Gender);

            if(userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDOb = DateTime.Today.AddYears(-userParams.MaxAge-1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDOb && u.DateOfBirth <= maxDob);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        break;
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo =await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<Photo> GetCurrentMainUserPhoto(int userId)
        {
            //var group = 1;
            //var b = new List<int>() { 5, 10, 15, 20, 45, 50, 55, 80, 85, 100 };

            //var res = b.Select((s, index) => new
            //{
            //    prior = index == 0 ? s : b[index - 1],
            //    item = s
            //}).GroupBy(f => (f.item - f.prior) <= 5 ? group : ++group, f => f.item);//grouping is simple
            //here what they do is that the groupby function returns IGrouping<TKey,V> and has 8 overloads
            //in this case we use the overload that selects key and items;
            //the key can be a constant, which means that the group key name will be named the constant or the key can be a field of the object
            //and the name will be the type of the field. So basically, it selects the group key and starts to select items for that group key as one group
            // if the group key changes, it closes the first group and starts to select items for the second group as the second group key name
            //and it continues like that till it is completed.

            //result of the group by for this list based on the code above is
            /*group key         output
             *  1             5,10,15,20
             *  2             45, 50, 55
             *  3             80, 85
             *  4             100
             */

            return await _context.Photos.Where(p => p.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }
    }
}
