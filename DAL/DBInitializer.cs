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

            var users = new List<User>
            {
                new User { UserName = manager1.UserName, FirstName = "Artur", LastName = "Kowalczyk", BirthDate = DateTime.Parse("1974-01-23"), PhoneNumber = "516325698" },
                new User { UserName = manager2.UserName, FirstName = "Krzystof", LastName = "Nowak", BirthDate = DateTime.Parse("1986-01-23"), PhoneNumber = "523658296" },
                new User { UserName = coach1.UserName, FirstName = "Michał", LastName = "Florek", BirthDate = DateTime.Parse("1983-10-27"), PhoneNumber = "513478568" },
                new User { UserName = coach2.UserName, FirstName = "Kamil", LastName = "Lewandowski", BirthDate = DateTime.Parse("1990-03-13"), PhoneNumber = "798623458" },
                new User { UserName = player1.UserName, FirstName = "Jan", LastName = "Bulik", BirthDate = DateTime.Parse("1992-12-30"), PhoneNumber = "512658101" },
                new User { UserName = player2.UserName, FirstName = "Adrian", LastName = "Pawlak", BirthDate = DateTime.Parse("1997-10-26"), PhoneNumber = "526987547" },
                new User { UserName = player3.UserName, FirstName = "Michał", LastName = "Kruk", BirthDate = DateTime.Parse("1995-03-09"), PhoneNumber = "728965250" },
            };
            users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();

            var clubs = new List<Club>
            {
                new Club { Name = "FC Warszawa", Logo = "FC Warszawa.jpg",  },
                new Club { Name = "Pogoń Lublin" }
            };
            clubs.ForEach(c => context.Clubs.Add(c));
            context.SaveChanges();
            var managers = new List<Manager>
            {
                new Manager { UserID = users[0].ID, ClubID = clubs[0].ID },
                new Manager { UserID = users[1].ID, ClubID = clubs[1].ID }
            };
            managers.ForEach(m => context.Managers.Add(m));
            context.SaveChanges();

            var coaches = new List<Coach>
            {
                new Coach { UserID = users[2].ID, ClubID = clubs[0].ID },
                new Coach { UserID = users[3].ID, ClubID = clubs[1].ID }
            };
            coaches.ForEach(c => context.Coaches.Add(c));
            context.SaveChanges();

            var teams = new List<Team>
            {
                new Team { Name = clubs[0].Name, CoachID = coaches[0].ID, ClubID = clubs[0].ID },
                new Team { Name = clubs[1].Name, CoachID = coaches[1].ID, ClubID = clubs[1].ID }
            };
            teams.ForEach(t => context.Teams.Add(t));
            context.SaveChanges();

            var players = new List<Player>
            {
                new Player { UserID = users[2].ID, ClubID = clubs[0].ID, TeamID = teams[0].ID, Height = 183, Weight = 78, LeadingLeg = LeadingLeg.Right, MainPosition = "Bramkarz", ShirtsNumber = 1 },
                new Player { UserID = users[4].ID, ClubID = clubs[0].ID, TeamID = teams[0].ID, Height = 187, Weight = 82, LeadingLeg = LeadingLeg.Right, MainPosition = "Napastnik", ShirtsNumber = 10 },
                new Player { UserID = users[5].ID, ClubID = clubs[0].ID, TeamID = teams[0].ID, Height = 178, Weight = 73, LeadingLeg = LeadingLeg.Left, MainPosition = "Lewy Obrońca", ShirtsNumber = 2 },
                new Player { UserID = users[6].ID, ClubID = clubs[0].ID, TeamID = teams[0].ID, Height = 181, Weight = 76, LeadingLeg = LeadingLeg.Both, MainPosition = "Środkowy Pomocnik", ShirtsNumber = 6 },
            };
            players.ForEach(p => context.Players.Add(p));
            context.SaveChanges();

            var messages = new List<Message>
            {
                new Message { Content = "Odwolany trening", SenderID = users[2].ID, ReceiverID = users[4].ID, SendDate = DateTime.Parse("01-02-2021 17:01")},
                new Message { Content = "Odwolany trening", SenderID = users[2].ID, ReceiverID = users[5].ID, SendDate = DateTime.Parse("01-02-2021 17:01")},
                new Message { Content = "Odwolany trening", SenderID = users[2].ID, ReceiverID = users[6].ID, SendDate = DateTime.Parse("01-02-2021 17:01")},
                new Message { Content = "Nieobecność na najbliższym meczu", SenderID = users[4].ID, ReceiverID = users[2].ID, SendDate = DateTime.Parse("04-02-2021 13:51")},
            };
            messages.ForEach(m => context.Messages.Add(m));
            context.SaveChanges();

            var exercises = new List<Exercise>
            {
                new Exercise { Name = "Rozgrzewka", NumberOfPlayers = 1, ExecutionTime = 10, ExerciseScheme = "", Description = "Dokładne rozgrzanie całego ciała, wszystkich partii mięśniowych, w celu uniknięcia kontuzji" },
                new Exercise { Name = "Gra w dziadka", NumberOfPlayers = 4, ExecutionTime = 5, ExerciseScheme = "", Description = "Gra w dziadka" },
                new Exercise { Name = "Wyprowadzenie piłki ze strefy obrony.", NumberOfPlayers = 8, ExecutionTime = 15, ExerciseScheme = "schemat3.jpg", Description = "To kompleksowe ćwiczenie łączy w sobie dwa elementy taktyczne; wyprowadzenie piłki ze strefy obrony i rozegranie ataku w środkowym pasie. Sprzyja doskonaleniu techniki podań bez przyjęcia i  kształtowaniu szybkości działania. Schemat pokazuje ćwiczenie wykonywane w jednej bocznej strefie, ale wykonuje się je na przemian w dwóch.Przebieg ćwiczenia: Zawodnik A podaje piłkę do partnera B i wykonuje start w kierunku linii bocznej. Po otrzymaniu piłki podanej od B zagrywa także bez przyjęcia do C. Kolejne podania wymieniają pomiędzy sobą zawodnicy C i B, po których zawodnik C podaje do D i kończy fazę wyprowadzenia piłki ze strefy obrony. D podaje do E i po odegraniu przez niego piłki wykonuje strzał na bramkę.Zmiany wykonywane są „za piłką” na miejsce kolejnego partnera. Ćwiczenie wykonywane jest na przemian z dwóch stron. Zawodnik wykonujący ćwiczenie z prawej strony po oddaniu strzału ustawia się na końcu drugiej grupy. Stosujemy szybkie podania bez przyjęcia wewnętrzną częścią stopy." },
                new Exercise { Name = "Podanie i przyjęcie na sygnał „Czas i plecy”", NumberOfPlayers = 2, ExecutionTime = 8, ExerciseScheme = "schemat1.png", Description = "Zawodnicy dobrani w dwójki w odległości 8-10 m. Na komendę „plecy” ćwiczący podają bez przyjęcia, na komendę „czas” podają oraz przyjmują wewnętrzną częścią stopy. Zawodnik bez piłki ustawiony nisko na nogach (unikamy bezsensownego truchtu w miejscu).  Możecie dodać gesty – partner pokazuje ręką kierunek podania. Wymuszam na moich podopiecznych komunikację w zespole, jestem uczulony na ciszę podczas gry. Zawodnicy MUSZĄ podpowiadać sobie, krzyczeć, informować o wolnej pozycji itd. Zwracam na to szczególną uwagę, to zawodnicy są głównymi kreatorami podczas meczu, trener ma ich tylko i aż wspierać, wspomagać w działaniu." },
                new Exercise { Name = "Gra „Do 10 podań”", NumberOfPlayers = 12, ExerciseScheme = "", ExecutionTime = 12, Description = "Zawodnicy podzieleni na dwa zespoły, wymieniają między sobą podania (głośno licząc). Drużyna przeciwna stara się przejąć piłkę. Po przejęciu piłki, wymieniają podania między sobą( nie ma podań powrotnych). Drużyna zdobywa punkt, gdy wykona 10 podań z rzędu. Wygrywa zespół, który zdobędzie więcej punktów." },
                new Exercise { Name = "Podania między pachołkami", NumberOfPlayers = 2, ExecutionTime = 8, ExerciseScheme = "schemat2.png", Description = "Zawodnicy ustawieni w dwójkach w odległości 5-8m. Zadaniem ćwiczących jest podanie między pachołkami. Możemy wprowadzić element rywalizacji – która dwójka wykona więcej podań w określonym czasie? Punkty karne przyznawane są za każdorazowe trafienie w pachołek. Przyjęcia wewnętrzną częścią stopy, podania bez przyjęcia. W zależności od poziomu grupy – zwiększamy/zmniejszamy odległość między pachołkami. Możemy pobawić się formą – różne ustawienie pachołków." },
            };
            exercises.ForEach(e => context.Exercises.Add(e));
            context.SaveChanges();

            var trainingOutlines = new List<TrainingOutline>
            {
                new TrainingOutline { Name = "Trening utrzymania przy piłce", AuthorID = users[2].ID, Exercises = new List<Exercise>{ exercises[0], exercises[3], exercises[5], exercises[1], exercises[4] } },
            };
            trainingOutlines.ForEach(o => context.TrainingOutlines.Add(o));
            context.SaveChanges();

        }
    }
}