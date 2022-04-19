using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Models;
using DatingApp.Repositories.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _repo = repo;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto user)
        {
            if(!await _repo.UserExists(user.Username))
            {
                var userCreate = new User()
                {
                    Username = user.Username.ToLower()

                };
              var createdUser =  await _repo.Register(userCreate, user.Password);
                return StatusCode(201); 
            }
            return BadRequest("Username already exists");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var userFromRepo = await _repo.Login(userLoginDto);
            if (userFromRepo == null)
                return Unauthorized();
            //3 steps to create a token
            //1) create token payload using a token descriptor            
            //populate payload of token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.Username)
            };
            //secret key of token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            //hash the key of the token
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //bringing the properties of the token together
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            //2)create a token handler to create the token string from the token descriptor
            //create a Json Web Token (JWT)
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //3)returning a token and user object to the client
            var userReturned = _mapper.Map<UserForListDto>(userFromRepo);

            //sending the token as a cookie to the client. The cookie will be automatically saved by the client
            HttpContext.Response.Cookies.Append("jwt", tokenHandler.WriteToken(token), new CookieOptions 
            {
                HttpOnly = true, //this option makes the cookie not to be editable by the client
                Expires = DateTime.Now.AddDays(1)
            });
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                userReturned
            });

        }
    }
}
