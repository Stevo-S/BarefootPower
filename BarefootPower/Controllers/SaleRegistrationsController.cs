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
    public class SaleRegistrationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SaleRegistrations
        public ActionResult Index()
        {
            return View(db.SaleRegistrations.ToList());
        }

        // GET: SaleRegistrations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SaleRegistration saleRegistration = db.SaleRegistrations.Find(id);
            if (saleRegistration == null)
            {
                return HttpNotFound();
            }
            return View(saleRegistration);
        }

        // GET: SaleRegistrations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SaleRegistrations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,GroupName,Membership,Present,Elec,Solar,Date")] SaleRegistration saleRegistration)
        {
            if (ModelState.IsValid)
            {
                db.SaleRegistrations.Add(saleRegistration);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(saleRegistration);
        }

        // GET: SaleRegistrations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SaleRegistration saleRegistration = db.SaleRegistrations.Find(id);
            if (saleRegistration == null)
            {
                return HttpNotFound();
            }
            return View(saleRegistration);
        }

        // POST: SaleRegistrations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GroupName,Membership,Present,Elec,Solar,Date")] SaleRegistration saleRegistration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(saleRegistration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(saleRegistration);
        }

        // GET: SaleRegistrations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SaleRegistration saleRegistration = db.SaleRegistrations.Find(id);
            if (saleRegistration == null)
            {
                return HttpNotFound();
            }
            return View(saleRegistration);
        }

        // POST: SaleRegistrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SaleRegistration saleRegistration = db.SaleRegistrations.Find(id);
            db.SaleRegistrations.Remove(saleRegistration);
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
