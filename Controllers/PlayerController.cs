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

namespace ClubManager.Controllers
{

    [Authorize]
    public class PlayerController : Controller
    {
        private ClubManagerContext db = new ClubManagerContext();

        // GET: Player
        [Authorize(Roles = "Admin,Manager")]
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
        [Authorize(Roles = "Admin,Manager")]
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
        public ActionResult Create()
        {
            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name");
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name");
            ViewBag.UserID = new SelectList(db.Users, "ID", "UserName");
            return View();
        }

        // POST: Player/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,ClubID,TeamID,LeadingLeg,Height,Weight,ShirtsNumber,MainPosition,SecondPosition")] Player player)
        {
            if (ModelState.IsValid)
            {
                db.Players.Add(player);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name", player.ClubID);
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", player.TeamID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "UserName", player.UserID);
            return View(player);
        }

        // GET: Player/Edit/5
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
