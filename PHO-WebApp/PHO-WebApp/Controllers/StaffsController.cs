using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PHO_WebApp.Models;
using PHO_WebApp.DataAccessLayer;

namespace PHO_WebApp.Controllers
{
    public class StaffsController : BaseController
    {
        private QualityContext db = new QualityContext();
        StaffDAL SD = new StaffDAL();

        // GET: Staffs
        public ActionResult Index()
        {
            return View(SD.getPracticeStaffs(PracticeId));
        }

        // GET: Staffs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // GET: Staffs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StaffId,FirstName,LastName,EmailAddress,ActiveFlag,DeletedFlag,StaffTypeId,CreatedbyId,CreatedOnDate,ModifiedbyId,ModifiedDate")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                db.Staffs.Add(staff);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(staff);
        }

        // GET: Staffs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StaffId,FirstName,LastName,EmailAddress,ActiveFlag,DeletedFlag,StaffTypeId,CreatedbyId,CreatedOnDate,ModifiedbyId,ModifiedDate")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                db.Entry(staff).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(staff);
        }

        // GET: Staffs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Staff staff = db.Staffs.Find(id);
            db.Staffs.Remove(staff);
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
        
        public JsonResult GetPhysicianLinks(string term = "")
        {
            List<Staff> staffList = new List<Staff>();

            //TODO: Add logic to cache and retrieve providers instead of loading them from DB everytime. Instead, check cache for providers. If exists, return from cache. If not, load from DB and then cache.

            //Get fresh from DAL
            staffList = SD.getPracticeProviders(PracticeId);

            //use where linq where clause to filter by term
            var physicianList = staffList.Where(c => c.DeletedFlag == false)
                            .Where(c => c.LookupDisplayText.ToUpper()
                            .Contains(term.ToUpper()))
                            .Where(c => c.StaffTypeId == (int)StaffTypeEnum.Provider)
                            .Select(c => new { label = c.LookupDisplayText, val = c.StaffId })
                            .Distinct().ToList();
            return Json(physicianList, JsonRequestBehavior.AllowGet);
        }
    }
}
