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
    public class MessageController : Controller
    {
        private ClubManagerContext db = new ClubManagerContext();

        // GET: Message
        public ActionResult Contacts()
        {
            int userID = db.Users.Single(u => u.UserName == User.Identity.Name).ID;
            var contacts = db.Users
                .Where(u => u.SendedMessages.Any(m => m.ReceiverID == userID) || u.ReceivedMessages.Any(m => m.SenderID == userID))
                .ToList();

            return View(contacts);
        }


        public ActionResult Messages(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int userID = db.Users.Single(u => u.UserName == User.Identity.Name).ID;
            var messages = db.Messages
                .Where(m => (m.SenderID == userID && m.ReceiverID == id ) || ( m.SenderID == id && m.ReceiverID == userID))
                .OrderByDescending(m => m.SendDate)
                .Include(m => m.Receiver).Include(m => m.Sender)
                .ToList();

            ViewBag.Receiver = db.Users.Find(id);

            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Message message)
        {
            if (ModelState.IsValid)
            {
                message.SenderID = db.Users.Single(u => u.UserName == User.Identity.Name).ID;
                message.SendDate = DateTime.Now;
                db.Messages.Add(message);
                db.SaveChanges();
            }

            ViewBag.Receiver = db.Users.Find(message.ReceiverID);
            return RedirectToAction("Messages", new { id = message.ReceiverID });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWithContact(Message message, string ReceiverUserName)
        {
            if (ModelState.IsValid)
            {
                if (!db.Users.Any(u => u.UserName == ReceiverUserName))
                {
                    ViewBag.ErrorMessage = "Nie ma użytkownika o takiej nazwie";
                    return RedirectToAction("Contacts");
                }
                var receiver = db.Users.Single(u => u.UserName == ReceiverUserName);
                message.ReceiverID = receiver.ID;
                message.SenderID = db.Users.Single(u => u.UserName == User.Identity.Name).ID;
                message.SendDate = DateTime.Now;
                db.Messages.Add(message);
                db.SaveChanges();

                ViewBag.Receiver = db.Users.Find(message.ReceiverID);
                return RedirectToAction("Messages", new { id = message.ReceiverID });
            }

            return RedirectToAction("Contacts");
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
