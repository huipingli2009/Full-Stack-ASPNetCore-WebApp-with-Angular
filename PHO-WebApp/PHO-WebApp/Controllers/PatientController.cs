using PHO_WebApp.DataAccessLayer;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using PHO_WebApp.Models;
using System.Collections.Generic;
//comment

namespace PHO_WebApp.Controllers
{
    public class PatientController : BaseController
    {
        DataAccessLayer.Patients pts = new Patients();

        public ActionResult GetPatients()
        {
            List<Patient> ptList = new List<Patient>();
            int practiceId = SavedUserDetails.PracticeId;
            //DataSet ds = pt.GetPatients(); use SPA id = 37 for now

            //DataSet ds = pts.GetPatients(practiceId);
            //ViewBag.patients = ds.Tables[0];
            ptList = pts.GetPatients(practiceId);
            return View(ptList);
        }

        //public ActionResult GetPatientInfo(int id)
        public ActionResult GetPatientInfo(int id)
        {
            //Patient pt = new Patient();
            //pt = null;
            //pt = this.GetPatientInfo(id);

            //DataSet ds = pts.GetPatientInfo(id);
            //ViewBag.patient = ds.Tables[0];
            return View();
        }

        public ActionResult AddPatient()
        {
            return View();
        }

        [HttpPost]
        //public ActionResult AddPatient(FormCollection fc)
        //{
        //    Patient pt = new Patient();
        //    pt.patientId = Convert.ToInt32(fc["txtId"]);
        //    pt.FirstName = fc["txtFirstName"];
        //    pt.LastName = fc["txtLastName"];
        //    pt.DOB = Convert.ToDateTime(fc["txtPersonDOB"]);
        //    pt.AddressLine1 = fc["txtAddress"];
        //    pt.City = fc["txtCity"];
        //    //pt.State_Id = Convert.ToInt32(fc["txtStateId"]);
        //    pt.Zip = fc["txtZip"];

        //    pts.AddPatient(pt);
        //    //return View("AddPatient");
        //    return RedirectToAction("GetPatients");
        //}

        public ActionResult UpdatePatient(int id)
        {
            //DataSet ds = pts.GetPatientInfo(id);
            //ViewBag.patient = ds.Tables[0];
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePatient(int id, FormCollection fc)
        {
            Patient pt = new Patient();
            pt.FirstName = fc["txtFirstName"];
            pt.LastName = fc["txtLastName"];
            pt.DOB = Convert.ToDateTime(fc["txtDOB"]);
            pt.AddressLine1 = fc["txtAddressLine1"];
            pt.City = fc["txtCity"];
            pt.Zip = fc["txtZip"];

            pts.UpPatient(pt);

            return RedirectToAction("GetPatients");
        }
        public ActionResult DeletePatient(Patient pt)
        {
            pts.DeletePatient(pt);
            return RedirectToAction("GetPatients");
        }

        public JsonResult GetPatientLinks(string term = "")
        {
            List<Models.Patient> patientList = new List<Models.Patient>();

            //TODO: Add logic to cache and retrieve providers instead of loading them from DB everytime. Instead, check cache for providers. If exists, return from cache. If not, load from DB and then cache.

            //Get fresh from DAL
            patientList = pts.GetPatients(PracticeId);

            //use where linq where clause to filter by term
            var PatientList = patientList.Where(c => c.LookupDisplayText.ToUpper()
                            .Contains(term.ToUpper()))
                            //.Where(c => c.StaffTypeId == (int)StaffTypeEnum.Provider)
                            .Select(c => new { label = c.LookupDisplayText, val = c.patientId })
                            .Take(10)
                            .Distinct().ToList();
            return Json(PatientList, JsonRequestBehavior.AllowGet);
        }
    }
}