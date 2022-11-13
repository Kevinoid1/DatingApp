using DatingApp.Helpers;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Data
{
    public class DatingAppContext : DbContext
    {
        public DatingAppContext(DbContextOptions<DatingAppContext> options):base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<UserLike> UserLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserLike>().HasKey(a => new { a.LoggedInUserId, a.LikedUserId });

            modelBuilder.Entity<UserLike>().HasOne(s => s.LoggedInUser)
                .WithMany(u => u.LikedByLoggedInUser)
                .HasForeignKey(u => u.LoggedInUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserLike>().HasOne(u => u.LikedUser)
                .WithMany(s => s.UsersThatLikedLoggedInUser)
                .HasForeignKey(u => u.LikedUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }


    }
}
