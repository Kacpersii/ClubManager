using ClubManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ClubManager.DAL
{
    public class ClubManagerContext : DbContext
    {
        public ClubManagerContext() : base("DefaultConnection")
        {

        }


        public DbSet<Club> Clubs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Player>().HasRequired<User>(p => p.User)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>().HasRequired<Coach>(t => t.Coach)
                .WithMany(c => c.Teams).HasForeignKey(t => t.CoachID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>().HasRequired<Team>(p => p.Team)
                .WithMany(t => t.Players).HasForeignKey(p => p.TeamID)
                .WillCascadeOnDelete(false);
        }
    }
}