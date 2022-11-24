using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IDatingRepository repo;
        private readonly ILikesRepository likesRepo;

        public LikesController(IDatingRepository _repo, ILikesRepository _likesRepo)
        {
            repo = _repo;
            likesRepo = _likesRepo;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
           // var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var loggedInUserId = User.GetUserId();

            var likedUser = await repo.GetUser(username);
            var sourceUser = await likesRepo.GetUserWithLikes(loggedInUserId);

            if (sourceUser.UserName == username) return BadRequest("You cannot like youself");

            if (likedUser == null) return NotFound();
            
            //check if user has liked this user already
            var userLiked = await likesRepo.GetUserLike(loggedInUserId, likedUser.Id);

            if(userLiked != null) return BadRequest("You have already liked this user");

            var entity = new UserLike
            {
                LoggedInUserId = loggedInUserId,
                LikedUserId = likedUser.Id,
            };

            sourceUser.LikedByLoggedInUser.Add(entity);

            if (await repo.SaveAll())
                return Ok();

            return BadRequest("Failed to like User");
            
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]string predicate, [FromQuery]PaginationParams paginationParams)
        {
            var users = await likesRepo.GetUserLikes(predicate, User.GetUserId(), paginationParams);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }
    }
}
