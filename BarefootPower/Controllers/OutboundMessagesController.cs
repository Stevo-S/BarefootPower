using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BarefootPower.Models;

namespace BarefootPower.Controllers
{
    public class OutboundMessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OutboundMessages
        public ActionResult Index()
        {
            return View(db.OutboundMessages.ToList());
        }

        // GET: OutboundMessages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OutboundMessage outboundMessage = db.OutboundMessages.Find(id);
            if (outboundMessage == null)
            {
                return HttpNotFound();
            }
            return View(outboundMessage);
        }

        // GET: OutboundMessages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OutboundMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Destination,Message,ServiceId,LinkId,Correlator")] OutboundMessage outboundMessage)
        {
            if (ModelState.IsValid)
            {
                db.OutboundMessages.Add(outboundMessage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(outboundMessage);
        }

        // GET: OutboundMessages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OutboundMessage outboundMessage = db.OutboundMessages.Find(id);
            if (outboundMessage == null)
            {
                return HttpNotFound();
            }
            return View(outboundMessage);
        }

        // POST: OutboundMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Destination,Message,ServiceId,LinkId,Correlator")] OutboundMessage outboundMessage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(outboundMessage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(outboundMessage);
        }

        // GET: OutboundMessages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OutboundMessage outboundMessage = db.OutboundMessages.Find(id);
            if (outboundMessage == null)
            {
                return HttpNotFound();
            }
            return View(outboundMessage);
        }

        // POST: OutboundMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OutboundMessage outboundMessage = db.OutboundMessages.Find(id);
            db.OutboundMessages.Remove(outboundMessage);
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
