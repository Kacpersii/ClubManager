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
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ClubManager.Controllers
{
    [Authorize]
    public class CoachController : Controller
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

        // GET: Coach
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var coaches = db.Coaches.Include(c => c.Club).Include(c => c.User);
            return View(coaches.ToList());
        }

        [Authorize(Roles = "Manager")]
        public ActionResult ClubCoaches()
        {
            var user = db.Users.Single(u => u.UserName == User.Identity.Name);
            var manager = db.Managers.Single(m => m.UserID == user.ID);
            var club = db.Clubs.Find(manager.ClubID);
            var coaches = db.Coaches.Where(c => c.ClubID == club.ID);

            return View("Index", coaches.ToList());
        }

        // GET: Coach/Details/5
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coach coach = db.Coaches.Find(id);
            if (coach == null)
            {
                return HttpNotFound();
            }
            return View(coach);
        }

        [Authorize(Roles = "Coach")]
        public ActionResult MyProfile()
        {
            User user = db.Users.Single(u => u.UserName == User.Identity.Name);
            Coach coach = db.Coaches.Single(p => p.UserID == user.ID);

            return View("Details", coach);
        }

        // GET: Coach/Create
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.UserID = new SelectList(db.Users, "ID", "UserName");
                ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name");
            }
            else
            {
                User user = db.Users.Single(u => u.UserName == User.Identity.Name);
                Manager manager = db.Managers.Single(m => m.UserID == user.ID);

                var coachesInOtherClubs = db.Coaches.Where(c => c.ClubID != manager.ClubID);
                var users = db.Users.Where(u => !coachesInOtherClubs.Any(c => c.UserID == u.ID));
                ViewBag.UserID = new SelectList(users, "ID", "UserName");
            }
            return View();
        }

        // POST: Coach/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create(Coach coach)
        {
            if(User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    db.Coaches.Add(coach);
                    db.SaveChanges();

                    var userModel = db.Users.Find(coach.UserID);
                    var user1 = UserManager.FindByName(userModel.UserName);
                    UserManager.AddToRole(user1.Id, "Coach");
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = coach.ID });
                }

                ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name", coach.ClubID);
                ViewBag.UserID = new SelectList(db.Users, "ID", "UserName", coach.UserID);
            }
            else
            {
                User user = db.Users.Single(u => u.UserName == User.Identity.Name);
                Manager manager = db.Managers.Single(m => m.UserID == user.ID);

                if (ModelState.IsValid)
                {
                    coach.ClubID = manager.ClubID;
                    db.Coaches.Add(coach);
                    db.SaveChanges();

                    var userModel = db.Users.Find(coach.UserID);
                    var user1 = UserManager.FindByName(userModel.UserName);
                    UserManager.AddToRole(user1.Id, "Coach");
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = coach.ID });
                }

                var coachesInOtherClubs = db.Coaches.Where(c => c.ClubID != manager.ClubID);
                var users = db.Users.Where(u => !coachesInOtherClubs.Any(c => c.UserID == u.ID));
                ViewBag.UserID = new SelectList(users, "ID", "UserName");
            }
            
            return View(coach);
        }

        // GET: Coach/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coach coach = db.Coaches.Find(id);
            if (coach == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name", coach.ClubID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "UserName", coach.UserID);
            return View(coach);
        }

        // POST: Coach/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Edit([Bind(Include = "ID,UserID,ClubID")] Coach coach)
        {
            if (ModelState.IsValid)
            {
                db.Entry(coach).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name", coach.ClubID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "UserName", coach.UserID);
            return View(coach);
        }

        // GET: Coach/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coach coach = db.Coaches.Find(id);
            if (coach == null)
            {
                return HttpNotFound();
            }
            return View(coach);
        }

        // POST: Coach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Coach coach = db.Coaches.Find(id);
            db.Coaches.Remove(coach);
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
