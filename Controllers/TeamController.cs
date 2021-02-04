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
    public class TeamController : Controller
    {
        private ClubManagerContext db = new ClubManagerContext();

        // GET: Team
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var teams = db.Teams.Include(t => t.Club).Include(t => t.Coach);
            return View(teams.ToList());
        }

        [Authorize(Roles = "Manager")]
        public ActionResult ClubTeams()
        {
            var user = db.Users.Single(u => u.UserName == User.Identity.Name);
            var manager = db.Managers.Single(m => m.UserID == user.ID);
            var club = db.Clubs.Find(manager.ClubID);
            var teams = db.Teams.Where(t => t.ClubID == club.ID);

            return View("Index", teams.ToList());
        }

        // GET: Team/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        [Authorize(Roles = "Player,Coach")]
        public ActionResult MyTeam()
        {
            var user = db.Users.Single(u => u.UserName == User.Identity.Name);

            Team team;
            if (db.Coaches.Any(c => c.UserID == user.ID))
            {
                team = db.Teams.Single(t => t.Coach.UserID == user.ID);
            } 
            else
            {
                team = db.Teams.Single(t => t.Players.Any(p => p.UserID == user.ID));
            }

            return View("Details", team);
        }

        // GET: Team/Create
        public ActionResult Create()
        {
            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name");
            ViewBag.CoachID = new SelectList(db.Coaches, "ID", "ID");
            return View();
        }

        // POST: Team/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,ClubID,CoachID")] Team team)
        {
            if (ModelState.IsValid)
            {
                db.Teams.Add(team);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name", team.ClubID);
            ViewBag.CoachID = new SelectList(db.Coaches, "ID", "ID", team.CoachID);
            return View(team);
        }

        // GET: Team/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name", team.ClubID);
            ViewBag.CoachID = new SelectList(db.Coaches, "ID", "ID", team.CoachID);
            return View(team);
        }

        // POST: Team/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,ClubID,CoachID")] Team team)
        {
            if (ModelState.IsValid)
            {
                db.Entry(team).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name", team.ClubID);
            ViewBag.CoachID = new SelectList(db.Coaches, "ID", "ID", team.CoachID);
            return View(team);
        }

        // GET: Team/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Team/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Team team = db.Teams.Find(id);
            db.Teams.Remove(team);
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
