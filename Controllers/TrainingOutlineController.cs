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
    public class TrainingOutlineController : Controller
    {
        private ClubManagerContext db = new ClubManagerContext();

        // GET: TrainingOutline
        public ActionResult Index()
        {
            var trainingOutlines = db.TrainingOutlines.Include(t => t.Author);
            return View(trainingOutlines.ToList());
        }

        // GET: TrainingOutline/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingOutline trainingOutline = db.TrainingOutlines.Find(id);
            if (trainingOutline == null)
            {
                return HttpNotFound();
            }
            return View(trainingOutline);
        }

        // GET: TrainingOutline/Create
        public ActionResult Create()
        {
            ViewBag.Exercises = db.Exercises;

            User user = db.Users.Single(u => u.UserName == User.Identity.Name);
            TrainingOutline trainingOutline = new TrainingOutline { AuthorID = user.ID };

            return View(trainingOutline);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingOutline trainingOutline, List<int> exercisesList)
        {
            if (ModelState.IsValid)
            {
                db.TrainingOutlines.Add(trainingOutline);
                db.SaveChanges();

                if (exercisesList != null && exercisesList.Count > 0)
                {
                    trainingOutline.TrainingOutlinesExercises = new List<TrainingOutlinesExercise>();
                    exercisesList.ForEach(e => db.TrainingOutlinesExercises
                        .Add(new TrainingOutlinesExercise { ExerciseID = e, TrainingOutlineID = trainingOutline.ID }));
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Exercises = db.Exercises;
            return View(trainingOutline);
        }

        // GET: TrainingOutline/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingOutline trainingOutline = db.TrainingOutlines.Find(id);
            if (trainingOutline == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorID = new SelectList(db.Users, "ID", "UserName", trainingOutline.AuthorID);
            return View(trainingOutline);
        }

        // POST: TrainingOutline/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,AuthorID")] TrainingOutline trainingOutline)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainingOutline).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorID = new SelectList(db.Users, "ID", "UserName", trainingOutline.AuthorID);
            return View(trainingOutline);
        }

        // GET: TrainingOutline/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingOutline trainingOutline = db.TrainingOutlines.Find(id);
            if (trainingOutline == null)
            {
                return HttpNotFound();
            }
            return View(trainingOutline);
        }

        // POST: TrainingOutline/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TrainingOutline trainingOutline = db.TrainingOutlines.Find(id);
            db.TrainingOutlines.Remove(trainingOutline);
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
