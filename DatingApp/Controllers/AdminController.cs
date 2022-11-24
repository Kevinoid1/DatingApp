using DatingApp.Helpers;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{

    public class AdminController : BaseApiController
    {
        private readonly UserManager<User> userManager;

        public AdminController(UserManager<User> _userManager)
        {
            userManager = _userManager;
        }


        [Authorize(Policy = PolicyEnum.AdminRole)]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await userManager.Users
                            .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                            .OrderBy(u => u.UserName)
                            .Select(u => new
                            {
                                u.Id,
                                Username = u.UserName,
                                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                            }).ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = PolicyEnum.AdminRole)]
        [HttpPut("roles/{username}")]
        public async Task<ActionResult> EditUserRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(","); 

            var user = await userManager.FindByNameAsync(username);

            if (user == null) return NotFound("Could not find User");

            var userRoles = await userManager.GetRolesAsync(user);

            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if(!result.Succeeded) return BadRequest(result.Errors);

            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(await userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = PolicyEnum.ModeratorRole)]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Modeartors and Admins can see this");
        }
    }
}
