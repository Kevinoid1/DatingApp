using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
   
    public class AuthController : BaseApiController
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ITokenService tokenService;
        private readonly IMapper _mapper;

        public AuthController(UserManager<User> _userManager, SignInManager<User> _signInManager, ITokenService _tokenService, IMapper mapper)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            tokenService = _tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(UserRegisterDto user)
        {
            if(await UserExists(user.Username)) return BadRequest("Username already exists");

            var newUser = _mapper.Map<User>(user);
            newUser.UserName = newUser.UserName.ToLower();
            var result = await userManager.CreateAsync(newUser, user.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await userManager.AddToRoleAsync(newUser, RolesEnum.Member);

            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);
            
            return new UserDto
            {
                Username = newUser.UserName,
                Gender = newUser.Gender,
                KnownAs = newUser.KnownAs,
                Token = await tokenService.CreateToken(newUser),
                Id = newUser.Id
            }; 
           
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(UserLoginDto userLoginDto)
        {
            var retrievedUser = await userManager.Users.Include(u => u.Photos).SingleOrDefaultAsync(u => u.UserName == userLoginDto.Username.ToLower());
            if (retrievedUser == null)
                return Unauthorized();

            var result = await signInManager.CheckPasswordSignInAsync(retrievedUser, userLoginDto.Password, false);

            if (!result.Succeeded) return Unauthorized();

           
            return new UserDto
            {
                Username = retrievedUser.UserName,
                Gender = retrievedUser.Gender,
                KnownAs = retrievedUser.KnownAs,
                Token = await tokenService.CreateToken(retrievedUser),
                PhotoUrl = retrievedUser.Photos.Where(p => p.IsMain).Select(p => p.Url).FirstOrDefault(),
                Id = retrievedUser.Id
            };

        }

        public async Task<bool> UserExists(string username)
        {
            var user = await userManager.Users.AnyAsync(u => u.UserName == username);
            if (user)
                return true;

            return false;

        }
    }
}
