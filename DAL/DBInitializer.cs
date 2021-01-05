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

            roleManager.Create(new IdentityRole("ADMIN"));
            roleManager.Create(new IdentityRole("Manager"));
            roleManager.Create(new IdentityRole("Coach"));
            roleManager.Create(new IdentityRole("Player"));

            var admin = new ApplicationUser { UserName = "admin@admin.com" };
            string adminPass = "Admin.123";
            userManager.Create(admin, adminPass);
            userManager.AddToRole(admin.Id, "ADMIN");

            var user0 = new ApplicationUser { UserName = "manager1@manager1.com" };
            string password0 = "Manager1.";
            userManager.Create(user0, password0);
            userManager.AddToRole(user0.Id, "Manager");

            var user1 = new ApplicationUser { UserName = "manager2@manager2.com" };
            string password1 = "Manager2.";
            userManager.Create(user1, password1);
            userManager.AddToRole(user1.Id, "Manager");

            var clubs = new List<Club>
            {
                new Club { Name = "FC Warszawa" },
                new Club { Name = "Pogoń Lublin" },
                new Club { Name = "Kolektyw Siedlce" }
            };
            clubs.ForEach(c => context.Clubs.Add(c));
            context.SaveChanges();


        }
    }
}