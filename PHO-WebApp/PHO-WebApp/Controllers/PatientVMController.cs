using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PHO_WebApp.Models;
using PHO_WebApp.ViewModel;

namespace PHO_WebApp.Controllers
{
    public class PatientVMController : BaseController
    { 
        // GET: PatientVM
        public ActionResult GetPatients()
        {
            PatientVM ptVM = new PatientVM();
            ptVM.UserLogin = SavedUserDetails;                    

            ptVM.HandleRequest();

            ModelState.Clear();
            return View(ptVM);
        }

        // GET: PatientVM/Details/5
        [HttpPost]
        public ActionResult GetPatients(PatientVM ptVM)
        {
            //PatientVM ptVM = new PatientVM();

            ptVM.IsValid = ModelState.IsValid;
            //ptVM.GetPatient(id);
            //ptVM.EventCommand = "SelectPatient";
            //ptVM.ptEntity = ptVM.GetPatient(id);
            ptVM.HandleRequest();         

            ModelState.Clear();

            return View(ptVM);
        }        

        // GET: PatientVM/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PatientVM/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PatientVM/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PatientVM/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PatientVM/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PatientVM/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
