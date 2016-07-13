using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BarefootPower.Models;
using OfficeOpenXml;
using System.IO;
using System.Globalization;
using System.Drawing;

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


        [HttpPost]
        public ActionResult Index(string groupName, string submit, DateTime? startDate, DateTime? endDate)
        {
            //// Get the registered sales
            var filteredSales = from fs in db.SaleRegistrations
                                select fs;

            if (!string.IsNullOrEmpty(groupName))
            {
                filteredSales = filteredSales.Where(fs => fs.GroupName.Contains(groupName));
            }

            if (startDate != null)
            {
                if (endDate < startDate || endDate == null)
                {
                    endDate = DateTime.Now;
                }
                filteredSales = filteredSales.Where(fs => fs.Date > startDate && fs.Date < endDate);
            }


            if (!string.IsNullOrEmpty(submit) && submit.ToLower().Equals("export"))
            {
                var salesRegistered = from sr in filteredSales
                                      select new SalesRegistrationExport()
                                      {
                                          Date = sr.Date,
                                          Location = sr.Agent.Location,
                                          REA = (sr.Agent.FirstName + " " + sr.Agent.LastName),
                                          GroupName = sr.GroupName,
                                          TotalMembership = sr.Membership,
                                          Present = sr.Present,
                                          WithElec = sr.Elec,
                                          WithSolar = sr.Solar,
                                          Sales = sr.Sales,
                                          Agent = sr.Agent
                                      };
                return Export(salesRegistered);
            }
            else
            {
                ViewBag.GroupNameFilter = groupName;
                if (startDate != null)
                {
                    ViewBag.StartDateFilter = startDate.Value.ToString("s");
                    ViewBag.EndDateFilter = endDate.Value.ToString("s");
                }
                return View(filteredSales.ToList());
            }
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


        private FileResult Export(IQueryable<SalesRegistrationExport> exportedSalesRegistration)
        {
            int registeredCount = exportedSalesRegistration.Count();

            // Get the customers for the registered sales
            var detailedSales = exportedSalesRegistration.SelectMany(ds => ds.Sales);
            var exportedDetailedSales = from ds in detailedSales
                                        select new
                                        {
                                            Date = ds.SaleRegistration.Date,
                                            Location = ds.SaleRegistration.Agent.Location,
                                            REA = (ds.SaleRegistration.Agent.FirstName + " " + ds.SaleRegistration.Agent.LastName),
                                            GroupName = ds.SaleRegistration.GroupName,
                                            ClientName = ds.ClientName,
                                            ClientPhone = ds.ClientPhone,
                                            C600 = ds.C600,
                                            C3000 = ds.C3000,
                                            C3000TV = ds.C3000TV
                                        };
            int detailedCount = exportedDetailedSales.Count();

            var exportedSalesProjection = from esr in exportedSalesRegistration
                                          select new
                                          {
                                              Date = esr.Date,
                                              Location = esr.Agent.Location,
                                              REA = (esr.Agent.FirstName + " " + esr.Agent.LastName),
                                              GroupName = esr.GroupName,
                                              TotalMembership = esr.TotalMembership,
                                              Present = esr.Present,
                                              WithElec = esr.WithElec,
                                              WithSolar = esr.WithSolar
                                          };

            MemoryStream stream;
            using (var excelPackage = new ExcelPackage(new MemoryStream()))
            {
                excelPackage.Workbook.Properties.Author = "Barefoot Power SMS Platform";
                excelPackage.Workbook.Properties.Title = "Barefoot Power Sales Report";
                excelPackage.Workbook.Properties.Company = "Better SMS LTD.";
                excelPackage.Workbook.Properties.Comments = "Report generated by the system based on the sales registered via SMS";

                var salesRegistrationsSheet = excelPackage.Workbook.Worksheets.Add("Sales Registrations Sheet");
                var customerSheet = excelPackage.Workbook.Worksheets.Add("Customer Sheet");

                // Populate Sales Registration Sheet
                salesRegistrationsSheet.DefaultColWidth = 16;
                salesRegistrationsSheet.Cells[1, 1].Value = "Date";
                salesRegistrationsSheet.Cells[1, 2].Value = "Location";
                salesRegistrationsSheet.Cells[1, 3].Value = "REA";
                salesRegistrationsSheet.Cells[1, 4].Value = "Group Name";
                salesRegistrationsSheet.Cells[1, 5].Value = "Total Membership";
                salesRegistrationsSheet.Cells[1, 6].Value = "Present";
                salesRegistrationsSheet.Cells[1, 7].Value = "With Elec";
                salesRegistrationsSheet.Cells[1, 8].Value = "With Solar";
                salesRegistrationsSheet.Cells[1, 9].Value = "Total Sales";

                if (registeredCount > 0)
                {
                    salesRegistrationsSheet.Cells[2, 9, registeredCount + 1, 9].Formula = "SUM(H2)";
                    salesRegistrationsSheet.Cells[2, 1].LoadFromCollection(exportedSalesProjection.ToList());

                    using (var range = salesRegistrationsSheet.Cells["A1:I1"])
                    {
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Font.Bold = true;
                        range.Style.Fill.BackgroundColor.SetColor(Color.PaleTurquoise);
                    }

                    salesRegistrationsSheet.Cells[2, 1, registeredCount + 1, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                }

                // Populate Customer Sheet
                customerSheet.DefaultColWidth = 16;
                customerSheet.Cells[1, 1].Value = "Date";
                customerSheet.Cells[1, 2].Value = "Location";
                customerSheet.Cells[1, 3].Value = "REA";
                customerSheet.Cells[1, 4].Value = "Group Name";
                customerSheet.Cells[1, 5].Value = "Client Name";
                customerSheet.Cells[1, 6].Value = "Client Number";
                customerSheet.Cells[1, 7].Value = "C600";
                customerSheet.Cells[1, 8].Value = "C3000";
                customerSheet.Cells[1, 9].Value = "C3000TV";
                customerSheet.Cells[1, 10].Value = "Total";
                if (registeredCount > 0)
                {
                    customerSheet.Cells[2, 10, detailedCount + 1, 10].Formula = "SUM(G2:I2)";

                    customerSheet.Cells[2, 1].LoadFromCollection(exportedDetailedSales.ToList());

                    using (var range = customerSheet.Cells["A1:J1"])
                    {
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Font.Bold = true;
                        range.Style.Fill.BackgroundColor.SetColor(Color.Aquamarine);
                    }

                    customerSheet.Cells[2, 1, detailedCount + 1, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                }

                excelPackage.Save();
                stream = (MemoryStream)excelPackage.Stream;
            }

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=BarefootPowerSalesReport.xlsx");

            return File(stream.ToArray(), Response.ContentType);
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
