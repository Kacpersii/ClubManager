using ClubManager.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ClubManager.DAL
{
    public class DBInitializer : DropCreateDatabaseIfModelChanges<ClubManagerContext>
    {
        protected override void Seed(ClubManagerContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            roleManager.Create(new IdentityRole("Admin"));
            roleManager.Create(new IdentityRole("Manager"));
            roleManager.Create(new IdentityRole("Coach"));
            roleManager.Create(new IdentityRole("Player"));

            var admin = new ApplicationUser { UserName = "admin@admin.com" };
            string adminPass = "Admin.123";
            userManager.Create(admin, adminPass);
            userManager.AddToRole(admin.Id, "Admin");

            var manager1 = new ApplicationUser { UserName = "manager1@manager.com" };
            string password0 = "Manager123.";
            userManager.Create(manager1, password0);
            userManager.AddToRole(manager1.Id, "Manager");

            var manager2 = new ApplicationUser { UserName = "manager2@manager.com" };
            userManager.Create(manager2, password0);
            userManager.AddToRole(manager2.Id, "Manager");

            var coach1 = new ApplicationUser { UserName = "coach1@coach.com" };
            string password1 = "Coach123.";
            userManager.Create(coach1, password1);
            userManager.AddToRole(coach1.Id, "Coach");
            userManager.AddToRole(coach1.Id, "Player");

            var coach2 = new ApplicationUser { UserName = "coach2@coach.com" };
            userManager.Create(coach2, password1);
            userManager.AddToRole(coach2.Id, "Coach");

            var player1 = new ApplicationUser { UserName = "player1@player.com" };
            string password2 = "Player123.";
            userManager.Create(player1, password2);
            userManager.AddToRole(player1.Id, "Player");

            var player2 = new ApplicationUser { UserName = "player2@player.com" };
            userManager.Create(player2, password2);
            userManager.AddToRole(player2.Id, "Player");

            var player3 = new ApplicationUser { UserName = "player3@player.com" };
            userManager.Create(player3, password2);
            userManager.AddToRole(player3.Id, "Player");

            var clubs = new List<Club>
            {
                new Club { Name = "FC Warszawa",  },
                new Club { Name = "Pogoń Lublin" }
            };

            var users = new List<User>
            {
                new User { UserName = manager1.UserName, FirstName = "Artur", LastName = "Kowalczyk", BirthDate = DateTime.Parse("1974-01-23"), PhoneNumber = "516325698", ClubID = clubs[0].ID },
                new User { UserName = manager2.UserName, FirstName = "Krzystof", LastName = "Nowak", BirthDate = DateTime.Parse("1986-01-23"), PhoneNumber = "523658296", ClubID = clubs[1].ID },
                new User { UserName = coach1.UserName, FirstName = "Michał", LastName = "Florek", BirthDate = DateTime.Parse("1983-10-27"), PhoneNumber = "513478568", ClubID = clubs[0].ID },
                new User { UserName = coach2.UserName, FirstName = "Kamil", LastName = "Lewandowski", BirthDate = DateTime.Parse("1990-03-13"), PhoneNumber = "798623458", ClubID = clubs[1].ID },
                new User { UserName = player1.UserName, FirstName = "Jan", LastName = "Bulik", BirthDate = DateTime.Parse("1992-12-30"), PhoneNumber = "512658101", ClubID = clubs[0].ID },
                new User { UserName = player2.UserName, FirstName = "Adrian", LastName = "Pawlak", BirthDate = DateTime.Parse("1997-10-26"), PhoneNumber = "526987547", ClubID = clubs[0].ID },
                new User { UserName = player3.UserName, FirstName = "Michał", LastName = "Kruk", BirthDate = DateTime.Parse("1995-03-09"), PhoneNumber = "728965250", ClubID = clubs[0].ID },
            };
            users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();

            clubs[0].Managers.Add(users[0]);
            clubs[1].Managers.Add(users[1]);
            clubs.ForEach(c => context.Clubs.Add(c));
            context.SaveChanges();

            var coaches = new List<Coach>
            {
                new Coach { UserID = users[2].ID },
                new Coach { UserID = users[3].ID }
            };
            coaches.ForEach(c => context.Coaches.Add(c));
            context.SaveChanges();

            var teams = new List<Team>
            {
                new Team { Name = clubs[0].Name, CoachID = coaches[0].ID },
                new Team { Name = clubs[1].Name, CoachID = coaches[1].ID }
            };
            teams.ForEach(t => context.Teams.Add(t));
            context.SaveChanges();

            var players = new List<Player>
            {
                new Player { UserID = users[2].ID, TeamID = teams[0].ID, Height = 183, Weight = 78, LeadingLeg = LeadingLeg.Right, MainPosition = "Bramkarz", ShirtsNumber = 1 },
                new Player { UserID = users[4].ID, TeamID = teams[0].ID, Height = 187, Weight = 82, LeadingLeg = LeadingLeg.Right, MainPosition = "Napastnik", ShirtsNumber = 10 },
                new Player { UserID = users[5].ID, TeamID = teams[0].ID, Height = 178, Weight = 73, LeadingLeg = LeadingLeg.Left, MainPosition = "Lewy Obrońca", ShirtsNumber = 2 },
                new Player { UserID = users[6].ID, TeamID = teams[0].ID, Height = 181, Weight = 76, LeadingLeg = LeadingLeg.Both, MainPosition = "Środkowy Pomocnik", ShirtsNumber = 6 },
            };
            players.ForEach(p => context.Players.Add(p));
            context.SaveChanges();


        }
    }
}