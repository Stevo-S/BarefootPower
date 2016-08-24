using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BarefootPower.Models;
using PagedList;

namespace BarefootPower.Controllers
{
    public class InboundMessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: InboundMessages
        public ActionResult Index(String phoneNumber, String messageText, string formatted, DateTime? startDate, DateTime? endDate, int? page)
        {
            var messages = from m in db.InboundMessages
                           select m;
            messages = messages.OrderByDescending(m => m.Id);

            if (!String.IsNullOrEmpty(phoneNumber))
            {
                messages = messages.Where(m => m.Sender.Contains(phoneNumber));
                ViewBag.phoneFilter = phoneNumber;
            }

            if (!String.IsNullOrEmpty(messageText))
            {
                messages = messages.Where(m => m.Message.Contains(messageText));
                ViewBag.messageFilter = messageText;
            }

            if (startDate != null)
            {
                if (endDate < startDate || endDate == null)
                {
                    endDate = DateTime.Now;
                }
                messages = messages.Where(m => m.Timestamp > startDate && m.Timestamp < endDate);
                ViewBag.startDateFilter = startDate.Value.ToString("s");
                ViewBag.endDateFilter = endDate.Value.ToString("s");
            }

            if (!string.IsNullOrEmpty(formatted))
            {
                if (formatted == "incorrectly")
                {
                    messages = messages.Where(m => m.PassedValidation);
                }
                ViewBag.formattedFilter = formatted;
            }
            
            int pageNumber = (page ?? 1);
            int pageSize = 25;
            ViewBag.Total = messages.Count();
            return View(messages.ToPagedList(pageNumber, pageSize));
        }

        // GET: InboundMessages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InboundMessage inboundMessage = db.InboundMessages.Find(id);
            if (inboundMessage == null)
            {
                return HttpNotFound();
            }
            return View(inboundMessage);
        }

        // GET: InboundMessages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InboundMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Message,Sender,ServiceId,LinkId,TraceUniqueId,Correlator,ShortCode,Timestamp")] InboundMessage inboundMessage)
        {
            if (ModelState.IsValid)
            {
                db.InboundMessages.Add(inboundMessage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(inboundMessage);
        }

        // GET: InboundMessages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InboundMessage inboundMessage = db.InboundMessages.Find(id);
            if (inboundMessage == null)
            {
                return HttpNotFound();
            }
            return View(inboundMessage);
        }

        // POST: InboundMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Message,Sender,ServiceId,LinkId,TraceUniqueId,Correlator,ShortCode,Timestamp")] InboundMessage inboundMessage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inboundMessage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inboundMessage);
        }

        // GET: InboundMessages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InboundMessage inboundMessage = db.InboundMessages.Find(id);
            if (inboundMessage == null)
            {
                return HttpNotFound();
            }
            return View(inboundMessage);
        }

        // POST: InboundMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InboundMessage inboundMessage = db.InboundMessages.Find(id);
            db.InboundMessages.Remove(inboundMessage);
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
