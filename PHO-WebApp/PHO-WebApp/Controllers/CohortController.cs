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
    }

}