using DatingApp.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Helpers
{
    public static class SeedExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var userData = File.ReadAllText("Data/UsersSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            modelBuilder.Entity<User>().HasMany(u => u.Photos).WithOne(p => p.User);

            var photoData = File.ReadAllText("Data/PhotosSeedData.json");
            var photos = JsonConvert.DeserializeObject<List<Photo>>(photoData);
            foreach (var user in users)
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("password", out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                for (int i = 0; i < photos.Count; i++)
                {
                    modelBuilder.Entity<User>(u =>
                    {
                        u.HasData(user);
                        u.OwnsMany(e => e.Photos).HasData(photos[i]);

                    });
                }

            }


        }
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            }
        }
    }
}

