using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ClubManager.DAL;
using ClubManager.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace ClubManager.Controllers
{
    [Authorize]
    public class ClubController : Controller
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

        // GET: Club
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Clubs.ToList());
        }

        // GET: Club/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
        }

        [Authorize(Roles = "Manager")]
        public ActionResult ManagersClub()
        {
            User user = db.Users.Single(u => u.UserName == User.Identity.Name);
            Club club = db.Clubs.Single(c => c.Managers.Any(p => p.UserID == user.ID));
            if (club == null)
            {
                return HttpNotFound();
            }
            return View("Details", club);
        }

        [Authorize(Roles = "Coach")]
        public ActionResult CoachsClub()
        {
            User user = db.Users.Single(u => u.UserName == User.Identity.Name);
            Club club = db.Clubs.Single(c => c.Coaches.Any(p => p.UserID == user.ID));
            if (club == null)
            {
                return HttpNotFound();
            }
            return View("Details", club);
        }

        [Authorize(Roles = "Player")]
        public ActionResult PlayersClub()
        {
            User user = db.Users.Single(u => u.UserName == User.Identity.Name);
            Club club = db.Clubs.Single(c => c.Players.Any(p => p.UserID == user.ID));
            if (club == null)
            {
                return HttpNotFound();
            }
            return View("Details", club);
        }

        // GET: Club/Create
        [Authorize(Roles = "Admin")]
        public ActionResult AddClub()
        {
            var users = db.Users
                .Where(u => !db.Managers.Any(m => m.UserID == u.ID))
                .Select(u => new { ID = u.ID, Name = u.UserName });
            ViewBag.userID = new SelectList(users, "ID", "Name");
            
            return View("Create");
        }

        // GET: Club/Create
        [Authorize(Roles = "Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Club/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create(Club club, int? userID)
        {
            if (ModelState.IsValid)
            {

                HttpPostedFileBase file = Request.Files["Logo"];
                if (file != null && file.ContentLength > 0)
                {
                    club.Logo = System.Guid.NewGuid().ToString() + file.FileName;
                    file.SaveAs(HttpContext.Server.MapPath("~/Images/Logos/") + club.Logo);
                }

                db.Clubs.Add(club);
                db.SaveChanges();

                User user;
                if (User.IsInRole("Admin"))
                {
                    user = db.Users.Single(u => u.ID == userID);
                    Manager createdManager = new Manager { UserID = (int)userID, ClubID = club.ID };
                    db.Managers.Add(createdManager);
                    db.SaveChanges();

                } 
                else
                {
                    user = db.Users.Single(u => u.UserName == User.Identity.Name);
                    Manager manager = new Manager { UserID = user.ID, ClubID = club.ID };
                    db.Managers.Add(manager);
                    db.SaveChanges();
                }

                var user1 = UserManager.FindByName(user.UserName);
                UserManager.AddToRole(user1.Id, "Manager");
                db.SaveChanges();

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Details", new { id = club.ID });
                }
                else
                {
                    return RedirectToAction("ManagersClub");
                }
                    
            }

            var users = db.Users
                .Where(u => !db.Managers.Any(m => m.UserID == u.ID))
                .Select(u => new { ID = u.ID, Name = u.UserName });
            ViewBag.userID = new SelectList(users, "ID", "Name");

            return View(club);
        }

        // GET: Club/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
        }

        // POST: Club/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Stadium,Logo")] Club club)
        {
            if (ModelState.IsValid)
            {
                db.Entry(club).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(club);
        }

        // GET: Club/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
        }

        // POST: Club/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Club club = db.Clubs.Find(id);
            db.Clubs.Remove(club);
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
