using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Repositories
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DatingAppContext context;

        public LikesRepository(DatingAppContext _context)
        {
            context = _context;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int likeUserId)
        {
           return await context.UserLikes.FindAsync(sourceUserId, likeUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(string predicate, int userId, PaginationParams parameters)
        {
            var users = context.Users.OrderBy(u => u.Username).AsQueryable();
            var likes = context.UserLikes.AsQueryable();

            if(predicate == "liked")
            {
                likes = likes.Where(like => like.LoggedInUserId == userId);
                users = likes.Select(l => l.LikedUser);
            }

            if(predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == userId);
                users = likes.Select(l => l.LoggedInUser);
            }

            var userLikes = users.Select(u => new LikeDto
            {
                Username = u.Username,
                KnownAs = u.KnownAs,
                PhotoUrl = u.Photos.Where(p => p.IsMain == true).Select(p => p.Url).FirstOrDefault(),
                City = u.City,
                Id = u.Id,
                Age = u.DateOfBirth.CalculateAge()
            });

            return await PagedList<LikeDto>.CreateAsync(userLikes, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<User> GetUserWithLikes(int userId)
        {
            //same as
            //var user = await (from u in context.Users
            //                  //join l in context.UserLikes on u.Id equals l.LikedUserId
            //                  where u.Id == userId
            //                  select new User
            //                  {
            //                      Id = u.Id,
            //                      City = u.City,
            //                      Country = u.Country,
            //                      DateOfBirth = u.DateOfBirth,
            //                      Created = u.Created,
            //                      Gender = u.Gender,
            //                      Interests = u.Interests,
            //                      Introduction = u.Introduction,
            //                      Username = u.Username,
            //                      LookingFor = u.LookingFor,
            //                      LastActive = u.LastActive,
            //                      KnownAs = u.KnownAs,
            //                      UsersThatLikedLoggedInUser = context.UserLikes.Where(l => l.LikedUserId == userId).ToList()
            //                  }).FirstOrDefaultAsync();

            //return user;
            
                              
            return await context.Users.Include(u => u.LikedByLoggedInUser).FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
