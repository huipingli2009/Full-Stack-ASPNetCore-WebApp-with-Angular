using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using PHO_WebApp.DataAccessLayer;
using System.Data;
using PHO_WebApp.Models;
using Newtonsoft.Json;


namespace PHO_WebApp.Controllers
{
    public class CohortController : BaseController
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

        //Load the partial view. Takes a CohortID as a parameter, -1 means new.
        public ActionResult CohortPartial(int CohortId)
        {
            Cohort model = null;
            if (CohortId > 0)
            {
                model = this.cohortRecords.GetCohort(CohortId);
            }
            else
            {
                model = this.cohortRecords.CreateCohortModel();
            }

            return PartialView("CohortModal", model);
        }

        //Post to save Cohort model
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

                return RedirectToAction("Index");
            }

            return View();
        }
    }

}