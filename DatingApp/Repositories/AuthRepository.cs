using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Interfaces;
using DatingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AuthRepository(UserManager<User> _userManager, SignInManager<User> _signInManager)
        {
            userManager = _userManager;
            signInManager = _signInManager;
        }
        public async Task<User> Login(UserLoginDto user)
        {

            var userRetrieved = await userManager.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.UserName == user.Username);
            if (userRetrieved == null)
                return null;

            //if(!VerifyPasswordHash(user.Password, userRetrieved.PasswordHash, userRetrieved.PasswordSalt))
            //{
            //    return null;
            //}

            return userRetrieved;
        }

        //private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        //{
        //    using(var hmac = new HMACSHA512(passwordSalt))
        //    {
        //        var computedPassHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        //        for (int i = 0; i < computedPassHash.Length; i++)
        //        {
        //            if (computedPassHash[i] != passwordHash[i])
        //                return false;

        //        }
        //        return true;
        //    }

        //}

        public async Task<User> Register(User user, string password)
        {
            //byte[] passwordHash, passwordSalt;
            //CreatePasswordHash(password,out passwordHash,out passwordSalt);

            //user.PasswordHash = passwordHash;
            //user.PasswordSalt = passwordSalt;

            //await _context.Users.AddAsync(user);
            //await _context.SaveChangesAsync();

            return user;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            }
        }


    }
}
