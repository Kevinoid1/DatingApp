using DatingApp.Helpers;
using DatingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Data
{
    public class DatingAppContext : IdentityDbContext<User,AppRole, int, IdentityUserClaim<int>, AppUserRole, 
        IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DatingAppContext(DbContextOptions<DatingAppContext> options):base(options)
        {

        }

        //public DbSet<User> Users { get; set; }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<UserLike> UserLikes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            //interesting. Don't do it this way because you want the entity AppRole to be required and not AppUserRole
            //modelBuilder.Entity<AppUserRole>().HasOne(ap => ap.Role)
            //    .WithMany(r => r.UserRoles).HasForeignKey(ap => ap.RoleId).IsRequired();

            modelBuilder.Entity<AppRole>().HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            modelBuilder.Entity<UserLike>().HasKey(a => new { a.LoggedInUserId, a.LikedUserId });

            modelBuilder.Entity<UserLike>().HasOne(s => s.LoggedInUser)
                .WithMany(u => u.LikedByLoggedInUser)
                .HasForeignKey(u => u.LoggedInUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserLike>().HasOne(u => u.LikedUser)
                .WithMany(s => s.UsersThatLikedLoggedInUser)
                .HasForeignKey(u => u.LikedUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>().HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSent)
                .HasForeignKey(k => k.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Message>().HasOne(m => m.Recipient)
                .WithMany(u => u.MessagesReceived)
                .HasForeignKey(k => k.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
