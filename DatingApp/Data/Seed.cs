using DatingApp.Helpers;
using DatingApp.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Data
{
    public static class Seed
    {
        public static async Task SeedUsers(UserManager<User> userManager, RoleManager<AppRole> roleManager)
        {
            if (!userManager.Users.Any())
            {
                var userData = await File.ReadAllTextAsync("Data/UsersSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                if (users == null) return;

                var roles = new List<AppRole>()
                {
                    new AppRole { Name = RolesEnum.Member },
                    new AppRole { Name = RolesEnum.Admin },
                    new AppRole { Name = RolesEnum.Moderator }
                };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
                foreach (var user in users)
                {
                    user.UserName = user.UserName.ToLower();
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                    await userManager.AddToRoleAsync(user, RolesEnum.Member);
                }

                var admin = new User
                {
                    UserName = "admin",
                };

                await userManager.CreateAsync(admin, "Pa$$w0rd");
                await userManager.AddToRolesAsync(admin, new string[] { RolesEnum.Admin, RolesEnum.Moderator });
            }
        }
    }
}
