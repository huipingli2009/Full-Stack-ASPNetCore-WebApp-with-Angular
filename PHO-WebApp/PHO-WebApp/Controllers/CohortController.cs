using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PHO_WebApp.DataAccessLayer;
using System.Data;
using PHO_WebApp.Models;
using Newtonsoft.Json;


namespace PHO_WebApp.Controllers
{
    public class CohortController : Controller
    {
        DataAccessLayer.CohortDAL cohortRecords = new CohortDAL();
     
            
        // GET: Cohort
        public ActionResult Index()
        {
            List<Cohort> CohortNew = new List<Cohort>();
            CohortNew = cohortRecords.getAllActiveCohortRecords().OrderByDescending(c => c.ModifiedDate).ToList();

            //var results = cohortRecords.getAllActiveCohortRecords();
            //return View(results);
            return View(CohortNew);
        }

        public PartialViewResult CohortDisplay()
        {
            return PartialView("CohortDisplay");
        }

        public ActionResult CreateCohortPartial()
        {
            Cohort model = new Cohort();

            return PartialView("CohortModal", model);
        }

        public ActionResult EditCohortPartial(int CohortId)
        {
            Cohort model = this.cohortRecords.GetCohort(CohortId);

            return PartialView("CohortModal", model);
        }

        [HttpPost]
        public ActionResult Save(Cohort model, string CohortStatusListItems)
        {
            if (ModelState.IsValid)
            {
                //Do some stuff
                if (model.id > 0)
                {
                    //Update
                    cohortRecords.UpdateCohort(model);
                }
                else
                {
                    //Insert
                    cohortRecords.InsertCohort(model);
                }
            }
            return RedirectToAction("Index");
        }
    }

}