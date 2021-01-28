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

        // GET: Club/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            ViewBag.ManagerID = new SelectList(db.Managers.Select(m => new { ID = m.ID, Name = m.User.UserName }), "ID", "Name");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Club club)
        {
            if (ModelState.IsValid)
            {
                var userID = db.Users.Single(u => u.UserName == User.Identity.Name).ID;
                Manager manager = new Manager { UserID = userID };
                club.Managers.Add(manager);
                db.Clubs.Add(club);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = club.ID });
            }

            return View(club);
        }

        // GET: Club/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Club/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Club club)
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

                var user = db.Users.Single(u => u.UserName == User.Identity.Name);
                Manager manager = new Manager { UserID = user.ID, ClubID = club.ID };
                db.Managers.Add(manager);
                db.SaveChanges();

                try
                {
                    var user1 = UserManager.FindByName(user.UserName);
                    UserManager.AddToRole(user1.Id, "Manager");
                    db.SaveChanges();
                }
                catch
                {
                    throw;
                }

                return RedirectToAction("Details", new { id = club.ID });
            }

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
