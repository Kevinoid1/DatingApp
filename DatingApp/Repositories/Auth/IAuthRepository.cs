using DatingApp.DTOs;
using DatingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Repositories.Auth
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);

        Task<User> Login(UserLoginDto user);

        Task<bool> UserExists(string username);
    }
}
