using PHO_WebApp.DataAccessLayer;
using System;
using System.Data;
using System.Web.Mvc;
using PHO_WebApp.Models;
using System.Collections.Generic;

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
        public ActionResult AddPatient(FormCollection fc)
        {
            Patient pt = new Patient();
            pt.Id = Convert.ToInt32(fc["txtId"]);
            pt.FirstName = fc["txtFirstName"];
            pt.LastName = fc["txtLastName"];
            pt.DOB = Convert.ToDateTime(fc["txtPersonDOB"]);
            pt.AddressLine1 = fc["txtAddress"];
            pt.City = fc["txtCity"];
            //pt.State_Id = Convert.ToInt32(fc["txtStateId"]);
            pt.Zip = fc["txtZip"];

            pts.AddPatient(pt);           
            //return View("AddPatient");
            return RedirectToAction("GetPatients");
        }

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
    }
}