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
using OfficeOpenXml;

namespace BarefootPower.Controllers
{
    public class AgentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Agents
        public ActionResult Index(int? page)
        {
            var agents = from a in db.Agents
                         select a;

            agents = agents.OrderBy(a => a.Id);

            int pageNumber = (page ?? 1);
            int pageSize = 16;
            ViewBag.Total = agents.Count();
            return View(agents.ToPagedList(pageNumber, pageSize));
        }

        // GET: Agents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agent agent = db.Agents.Find(id);
            if (agent == null)
            {
                return HttpNotFound();
            }
            return View(agent);
        }

        // GET: Agents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Agents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,MiddleName,LastName,Phone,Location,Email,isActive")] Agent agent)
        {
            if (ModelState.IsValid)
            {
                db.Agents.Add(agent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(agent);
        }

        // GET: Agents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agent agent = db.Agents.Find(id);
            if (agent == null)
            {
                return HttpNotFound();
            }
            return View(agent);
        }

        // POST: Agents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,MiddleName,LastName,Phone,Location,Email,isActive")] Agent agent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(agent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(agent);
        }

        // GET: Agents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agent agent = db.Agents.Find(id);
            if (agent == null)
            {
                return HttpNotFound();
            }
            return View(agent);
        }

        // POST: Agents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Agent agent = db.Agents.Find(id);
            db.Agents.Remove(agent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET 
        public ActionResult Upload()
        {
            ViewBag.hasErrors = false;
            return View();
        }

        // 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase AgentsFile)
        {
            //TODO: Implement Upload Agents via Excel sheet
            // Read uploaded Microsoft Excel file and create tips based on file content
            // Assumes the tips are in the first column starting from the second row.
            
            var ErrorMessages = new List<string>() { };
            var SuccessMessages = new List<string>() { };

            try
            {
                ViewBag.hasErrors = false;
                int duplicates = 0; 
                if ((AgentsFile != null) && (AgentsFile.ContentLength > 0) && !string.IsNullOrEmpty(AgentsFile.FileName))
                {
                    using (var package = new ExcelPackage(AgentsFile.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var worksheet = currentSheet.First();
                        var numberOfColumns = worksheet.Dimension.End.Column;
                        var numberOfRows = worksheet.Dimension.End.Row;

                        string firstNameColumn, middleNameColumn, lastNameColumn, phoneColumn, locationColumn, emailColumn, activeColumn;

                        using (var headers = worksheet.Cells[1, 1, 1, numberOfColumns])
                        {
                            var expectedHeaders = new[] { "first name", "middle name", "last name",
                                                          "phone",
                                                          "email",
                                                          "active"
                                                        };
                            if (!expectedHeaders.All(eh => headers.Any(h => h.Value.ToString().ToLower().StartsWith(eh))))
                            {
                                ErrorMessages.Add("Missing and/or incorrectly named fields");
                                ViewBag.hasErrors = true;
                                return View();
                            }

                            firstNameColumn = headers.First(h => h.Value.ToString().ToLower().StartsWith("first name")).Address[0].ToString();
                            middleNameColumn = headers.First(h => h.Value.ToString().ToLower().StartsWith("middle name")).Address[0].ToString();
                            lastNameColumn = headers.First(h => h.Value.ToString().ToLower().StartsWith("last name")).Address[0].ToString();
                            phoneColumn = headers.First(h => h.Value.ToString().ToLower().StartsWith("phone")).Address[0].ToString();
                            emailColumn = headers.First(h => h.Value.ToString().ToLower().StartsWith("email")).Address[0].ToString();
                            locationColumn = headers.First(h => h.Value.ToString().ToLower().StartsWith("location")).Address[0].ToString();
                            activeColumn = headers.First(h => h.Value.ToString().ToLower().StartsWith("active")).Address[0].ToString();
                        }
                        for (int row = 2; row <= numberOfRows; row++)
                        {
                            var firstName = worksheet.Cells[firstNameColumn + row].Value.ToString();
                            var middleName = worksheet.Cells[middleNameColumn + row].Value != null ? worksheet.Cells[middleNameColumn + row].Value.ToString() : "";
                            var lastName = worksheet.Cells[lastNameColumn + row].Value.ToString();
                            var phone = worksheet.Cells[phoneColumn + row].Value.ToString();
                            var location = worksheet.Cells[locationColumn + row].Value.ToString();
                            var email = worksheet.Cells[emailColumn + row].Value != null ? worksheet.Cells[emailColumn + row].Value.ToString() : "";
                            var activeStatus = worksheet.Cells[activeColumn + row].Value.ToString().ToLower().Equals("yes") ? true : false;

                            // Ignore records with the phone number already existing in the database
                            if (db.Agents.Where(a => a.Phone.EndsWith(phone.Substring(phone.Length - 9))).Any())
                            {
                                duplicates++;
                                continue;
                            }

                            var agent = new Agent()
                            {
                                FirstName = firstName,
                                MiddleName = middleName,
                                LastName = lastName,
                                Phone = phone,
                                Location = location,
                                Email = email,
                                isActive = activeStatus
                            };

                            db.Agents.Add(agent);
                        }

                        SuccessMessages.Add("Upload Successful.");
                        if (duplicates > 0)
                        {
                            SuccessMessages.Add(duplicates.ToString() + " duplicate/existing phone numbers detected. They have been ignored.");
                        }
                    }

                    db.SaveChanges();
                    TempData["SuccessNotifications"] = SuccessMessages;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.hasErrors = true;
                ErrorMessages.Add("Could not add agents into the database. The Uploaded file is not properly formatted.");
                ViewBag.ErrorNotifications = ErrorMessages;
                return View();
            }

            return View();
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
