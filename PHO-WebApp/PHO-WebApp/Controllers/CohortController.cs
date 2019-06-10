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
        //DataSet ds = cohortRecords.getAllActiveCohortRecords();
        //ViewBag.Cohort = ds.Tables[0];
        

       
            
        // GET: Cohort
        public ActionResult Index()
        {
            List<Cohort> CohortNew = new List<Cohort>();
            CohortNew = cohortRecords.getAllActiveCohortRecords();

            //var results = cohortRecords.getAllActiveCohortRecords();
            //return View(results);
            return View(CohortNew);
        }

        //public ActionResult DisplayCohort()
        //{
        //    DataSet ds = cohortRecords.getAllActiveCohortRecords();
        //    return View();
        //    //string jsonresult = JsonConvert.SerializeObject(ds.Tables[0]);
        //    ////foreach(var a in ds)
        //    ////      {

        //    ////  }
        //    ////  ViewBag.Cohort = ds.Tables[0];
        //    ////  return Json(new { Name = "bar", ShortName = "Blech",Description= "testDes", Owner ="est",ModifiedDate = "tes" }, JsonRequestBehavior.AllowGet);
        //    ////  return View();
        //    ////return Json(
        //    ////   jsonresult
        //    ////    , JsonRequestBehavior.AllowGet);
        //    //return Json(JsonConvert.SerializeObject(new { item = ds.Tables[0] }), JsonRequestBehavior.AllowGet);
        //    ////JsonRequestBehavior.AllowGet);


        //    ////      { "data": "Id" },
        //    ////{ "data": "Name" },
        //    ////{ "data": "ShortName" },
        //    ////{ "data": "Description"},
        //    ////{ "data": "Owner"},
        //    //{ "data": "ModifiedDate"}

        //}

        //public ActionResult DisplayCohort()
        //{
        //    //DataSet ds = cohortRecords.getAllActiveCohortRecords();
        //    //ViewBag.Cohort = ds.Tables[0];

        //    //Response.Redirect(Url.Action("DisplayCohort", "Cohort"));
        //    //return View("DisplayCohort");

        //    //return View("DataDictionary");

        //}
        public PartialViewResult CohortDisplay()
        {
            //DataSet ds = cohortRecords.getAllActiveCohortRecords();
            //ViewBag.Cohort = ds.Tables[0];

            ////Response.Redirect(Url.Action("DisplayCohort", "Cohort"));
            return PartialView("CohortDisplay");
            ////return PartialView();
        }
    }

}