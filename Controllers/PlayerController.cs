using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClubManager.DAL;
using ClubManager.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ClubManager.Controllers
{

    [Authorize]
    public class PlayerController : Controller
    {
        private ClubManagerContext db = new ClubManagerContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Player
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var players = db.Players.Include(p => p.Club).Include(p => p.Team).Include(p => p.User);
            return View(players.ToList());
        }

        [Authorize(Roles = "Manager")]
        public ActionResult ClubPlayers()
        {
            var user = db.Users.Single(u => u.UserName == User.Identity.Name);
            var manager = db.Managers.Single(m => m.UserID == user.ID);
            var club = db.Clubs.Find(manager.ClubID);
            var players = db.Players.Where(p => p.ClubID == club.ID);

            return View("Index", players.ToList());
        }

        // GET: Player/Details/5
        [Authorize(Roles = "Admin,Manager,Coach")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        [Authorize(Roles = "Player")]
        public ActionResult MyProfile()
        {
            User user = db.Users.Single(u => u.UserName == User.Identity.Name);
            Player player = db.Players.Single(p => p.UserID == user.ID);
            
            return View("Details", player);
        }

        // GET: Player/Create
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name");
                ViewBag.TeamID = new SelectList(db.Teams.Select(t => new { ID = t.ID, Name = t.Name + " (" + t.Club.Name + ")" }), "ID", "Name");
                var usersNotPlayers = db.Users.Where(u => !db.Players.Any(p => p.UserID == u.ID));
                ViewBag.UserID = new SelectList(usersNotPlayers, "ID", "UserName");
            }
            else
            {
                User user = db.Users.Single(u => u.UserName == User.Identity.Name);
                Manager manager = db.Managers.Single(m => m.UserID == user.ID);
                Club club = db.Clubs.Find(manager.ClubID);

                ViewBag.TeamID = new SelectList(db.Teams.Where(t => t.ClubID == club.ID), "ID", "Name");
                var usersNotPlayers = db.Users.Where(u => !db.Players.Any(p => p.UserID == u.ID));
                ViewBag.UserID = new SelectList(usersNotPlayers, "ID", "UserName");
            }
            return View();
        }

        // POST: Player/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create(Player player)
        {
            if (User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    var club = db.Clubs.Find(player.ClubID);
                    var team = db.Teams.Find(player.TeamID);

                    if (club.ID == team.ClubID)
                    {
                        db.Players.Add(player);
                        db.SaveChanges();

                        var userModel = db.Users.Find(player.UserID);
                        var user1 = UserManager.FindByName(userModel.UserName);
                        UserManager.AddToRole(user1.Id, "Player");
                        db.SaveChanges();

                        return RedirectToAction("Details", new { id = player.ID });
                    }
                    else
                    {
                        ViewBag.ErrorMessage = $"Drużyna {team.Name} nie należy do klubu {club.Name}. Wybierz drużynę należącą do danego klubu.";
                    }
                }

                ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name", player.ClubID);
                ViewBag.TeamID = new SelectList(db.Teams.Select(t => new { ID = t.ID, Name = t.Name + " (" + t.Club.Name + ")" }), "ID", "Name", player.TeamID);
                var usersNotPlayers = db.Users.Where(u => !db.Players.Any(p => p.UserID == u.ID));
                ViewBag.UserID = new SelectList(usersNotPlayers, "ID", "UserName", player.UserID);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    Team team = db.Teams.Find(player.TeamID);
                    player.ClubID = db.Clubs.Find(team.ClubID).ID;
                    db.Players.Add(player);
                    db.SaveChanges();

                    var userModel = db.Users.Find(player.UserID);
                    var user1 = UserManager.FindByName(userModel.UserName);
                    UserManager.AddToRole(user1.Id, "Player");
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = player.ID });
                }
                User user = db.Users.Single(u => u.UserName == User.Identity.Name);
                Manager manager = db.Managers.Single(m => m.UserID == user.ID);
                Club club1 = db.Clubs.Find(manager.ClubID);

                ViewBag.TeamID = new SelectList(db.Teams.Where(t => t.ClubID == club1.ID), "ID", "Name");
                var usersNotPlayers1 = db.Users.Where(u => !db.Players.Any(p => p.UserID == u.ID));
                ViewBag.UserID = new SelectList(usersNotPlayers1, "ID", "UserName");
            }

            return View(player);
        }

        // GET: Player/Edit/5
        [Authorize(Roles = "Admin,Manager,Coach")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name", player.ClubID);
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", player.TeamID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "UserName", player.UserID);
            return View(player);
        }

        // POST: Player/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Coach")]
        public ActionResult Edit([Bind(Include = "ID,UserID,ClubID,TeamID,LeadingLeg,Height,Weight,ShirtsNumber,MainPosition,SecondPosition")] Player player)
        {
            if (ModelState.IsValid)
            {
                db.Entry(player).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name", player.ClubID);
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", player.TeamID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "UserName", player.UserID);
            return View(player);
        }

        // GET: Player/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: Player/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Player player = db.Players.Find(id);
            db.Players.Remove(player);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
