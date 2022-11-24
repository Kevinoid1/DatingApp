using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likeUserId);
        Task<User> GetUserWithLikes(int userId);
        Task<PagedList<LikeDto>> GetUserLikes(string predicate, int userId, PaginationParams parameters);
    }
}
