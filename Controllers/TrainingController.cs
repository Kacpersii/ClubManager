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

namespace ClubManager.Controllers
{
    [Authorize]
    public class TrainingController : Controller
    {
        private ClubManagerContext db = new ClubManagerContext();

        // GET: Training
        public ActionResult Index()
        {
            var trainings = db.Trainings.Where(t => t.Date >= DateTime.Now).OrderBy(t => t.Date).Include(t => t.Team).Include(t => t.TrainingOutline);
            return View(trainings.ToList());
        }

        public ActionResult FinishedTrainings()
        {
            var trainings = db.Trainings.Where(t => t.Date < DateTime.Now).OrderByDescending(t => t.Date).Include(t => t.Team).Include(t => t.TrainingOutline);
            return View(trainings.ToList());
        }

        // GET: Training/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training training = db.Trainings.Find(id);
            if (training == null)
            {
                return HttpNotFound();
            }
            return View(training);
        }

        public ActionResult CheckAttendance(int id)
        {
            Training training = db.Trainings.Find(id);
            return View(training);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckAttendance(int trainingID, List<int> playersList)
        {
            Training training = db.Trainings.Find(trainingID);
            training.IsAttendanceListChecked = true;

            Team team = db.Teams.Single(t => t.ID == training.TeamID);
            if (team.Players.Count > 0)
            {
                team.Players.ForEach(p => db.Attendances.Add(new Attendance { PlayerID = p.ID, TrainingID = trainingID, WasPresent = false }));
                db.SaveChanges();
            }

            if(playersList.Count > 0)
            {
                playersList.ForEach(p => db.Attendances.Single(a => a.PlayerID == p && a.TrainingID == trainingID).WasPresent = true);
                db.SaveChanges();
            }
            
            return RedirectToAction("Details", new { id = trainingID });
        }

        // GET: Training/Create
        public ActionResult Create()
        {
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name");
            ViewBag.TrainingOutlineID = new SelectList(db.TrainingOutlines, "ID", "Name");
            return View();
        }

        // POST: Training/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Date,Place,TrainingOutlineID,TeamID")] Training training)
        {
            if (ModelState.IsValid)
            {
                db.Trainings.Add(training);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", training.TeamID);
            ViewBag.TrainingOutlineID = new SelectList(db.TrainingOutlines, "ID", "Name", training.TrainingOutlineID);
            return View(training);
        }

        // GET: Training/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training training = db.Trainings.Find(id);
            if (training == null)
            {
                return HttpNotFound();
            }
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", training.TeamID);
            ViewBag.TrainingOutlineID = new SelectList(db.TrainingOutlines, "ID", "Name", training.TrainingOutlineID);
            return View(training);
        }

        // POST: Training/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Date,Place,TrainingOutlineID,TeamID")] Training training)
        {
            if (ModelState.IsValid)
            {
                db.Entry(training).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", training.TeamID);
            ViewBag.TrainingOutlineID = new SelectList(db.TrainingOutlines, "ID", "Name", training.TrainingOutlineID);
            return View(training);
        }

        // GET: Training/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training training = db.Trainings.Find(id);
            if (training == null)
            {
                return HttpNotFound();
            }
            return View(training);
        }

        // POST: Training/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Training training = db.Trainings.Find(id);
            db.Trainings.Remove(training);
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
