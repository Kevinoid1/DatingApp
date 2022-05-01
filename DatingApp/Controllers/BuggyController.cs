using DatingApp.Data;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuggyController : ControllerBase
    {
        private DatingAppContext context;

        public BuggyController(DatingAppContext _context)
        {
            context = _context;
        }

        [HttpGet("not-found")]
        public async Task<ActionResult<User>> GetNotFound()
        {
            var user = await context.Users.FindAsync(-1);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("server-error")]
        public async Task<string> ServerError()
        {
            var user = await context.Users.FindAsync(-1);

            var thing = user.ToString();
            return thing;
        }

        [Authorize]
        [HttpGet("auth")]
        public string GetUnauthorized()
        {
            return "secret";
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This is a bad request");
        }

        [HttpGet("unauthorized")]
        public ActionResult GetSecondUnauthorized()
        {
            return Unauthorized("This is an unauthorized request");
        }
    }
}
