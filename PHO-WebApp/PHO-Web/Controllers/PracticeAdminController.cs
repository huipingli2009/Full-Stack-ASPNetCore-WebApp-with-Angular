using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using PHO_WebApp.ViewModel;


namespace PHO_WebApp.Controllers
{
    public class PracticeAdminController : Controller
    {
        //StaffDAL SD = new StaffDAL();
       
        // GET: PracticeAdmin
        public ActionResult GetPracticeStaff ()
        {
            PracticeAdmin pracAdmin = new PracticeAdmin();
            //pracAdmin.PracStaff = SD.getPracticeStaffs();
            pracAdmin.HandleRequest();
            return View(pracAdmin);
        }

        // GET: Staffs/Create
        public ActionResult Create()
        {
            return View();
        }

        //// POST: Staffs/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,StaffId,FirstName,LastName,EmailAddress,ActiveFlag,DeletedFlag,StaffTypeId,CreatedbyId,CreatedOnDate,ModifiedbyId,ModifiedDate")] Staff staff)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Staffs.Add(staff);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(staff);
        //}
    }
}