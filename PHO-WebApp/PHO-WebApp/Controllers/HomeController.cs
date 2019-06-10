using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PHO_WebApp.DataAccessLayer;
using System.Data;
using PHO_WebApp.Models;
using Newtonsoft.Json;
//using PHO_WebApp.Models;


namespace PHO_WebApp.Controllers
{
    public class HomeController : Controller
    {
        Models.UserDetails user = null;
        DataAccessLayer.DataDictionary records = new DataAccessLayer.DataDictionary();
        //DataAccessLayer.CohortDAL cohortRecords = new CohortDAL();


        public bool IsLoggedIn()
        {
            bool returnValue = false;

            if (Session["UserDetails"] != null)
            {
                returnValue = true;
            }

            return returnValue;
        }

        public ActionResult Index()
        {
            return View("Index");
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Security()
        {
            //return View("Security");
            return RedirectToAction("Login", "Login");
        }

        public ActionResult Patient()
        {
            //return View();
            return RedirectToAction("GetPatients", "Patient");
        }

        public ActionResult Staff()
        {
            return View("Staff");
        }

        public ActionResult Population()
        {
            return View("Population");

        }

        public ActionResult Initiative()
        {
            return View("Initiative");
        }

        public ActionResult Content()
        {
            return View("Content");
        }


        public ActionResult DataDictionary()
        {
            //Execute Query
            DataSet ds = records.GetDataDictionaryRecords();
            ViewBag.DataDictionary = ds.Tables[0];

            //Build drop down lists
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "", Value = "" });
            li.Add(new SelectListItem { Text = "Yes", Value = "1" });
            li.Add(new SelectListItem { Text = "No", Value = "0" });
            ViewBag.PHIOptions = li;

            List<SelectListItem> inli = new List<SelectListItem>();
            inli.Add(new SelectListItem { Text = "", Value = "" });
            inli.Add(new SelectListItem { Text = "Yes", Value = "YES" });
            inli.Add(new SelectListItem { Text = "No", Value = "NO" });
            ViewBag.IsNullableOptions = inli;

            List<SelectListItem> dbli = new List<SelectListItem>();
            dbli.Add(new SelectListItem { Text = "", Value = "" });
            dbli.Add(new SelectListItem { Text = "PHO", Value = "PHO" });
            dbli.Add(new SelectListItem { Text = "PHO_DW", Value = "PHO_DW" });
            ViewBag.DatabaseNameOptions = dbli;

            return View("DataDictionary");
        }

        //public ActionResult UpdateDataDictionary(int id)
        //{
        //    return View("");
        //}
        [HttpPost]        
        public ActionResult UpdateDataDictionary(PHO_WebApp.Models.DataDictionary dd)
        {
            PHO_WebApp.Models.DataDictionary dataDictionary = new PHO_WebApp.Models.DataDictionary();
            ////dataDictionary.DatabaseName = dd.DatabaseName ;
            //dataDictionary.SchemaName = dd.SchemaName;
            //dataDictionary.ObjectName = dd.ObjectName;
            //dataDictionary.ObjectType = dd.ObjectType
            
            dataDictionary.skDataDictionary = dd.skDataDictionary;
            dataDictionary.PHIFlag = dd.PHIFlag;
            dataDictionary.BusinessDefinition = dd.BusinessDefinition;
            records.UpdateDictionaryRecords(dataDictionary);
            
            return Json(dd);       
        }
        public ActionResult DataDictionaryUpdate()
        {
            //DataSet dsNew = records.RefreshDataDictionary();
            records.RefreshDataDictionary();
            DataSet ds = records.GetDataDictionaryRecords();
            ViewBag.DataDictionary = ds.Tables[0];

            Response.Redirect(Url.Action("DataDictionary", "Home"));
            return View("");

            //return View("DataDictionary");
        }

        public ActionResult LoggedInAs()
        {
            if (Session["UserDetails"] != null)
            {
                this.user = (Models.UserDetails)Session["UserDetails"];
            }
            ViewBag.UserName = this.user.UserName;
                
            return LoggedInAs();
        }

        public ActionResult Login()
        {
            return RedirectToAction("Index", "Home");
        }

        //for other purpose
        //public string Cohort()
        //{
        //    //return View("");
        //return "This site is currently under construction!";           
        //}   

        //public ActionResult Cohort()
        //{
        //    return View();
        //}
        //public ActionResult DisplayCohort()
        //{
        //    DataSet ds = cohortRecords.getAllActiveCohortRecords();
        //    string jsonresult=  JsonConvert.SerializeObject(ds.Tables[0]);
        //    //foreach(var a in ds)
        //    //      {

        //    //  }
        //    //  ViewBag.Cohort = ds.Tables[0];
        //    //  return Json(new { Name = "bar", ShortName = "Blech",Description= "testDes", Owner ="est",ModifiedDate = "tes" }, JsonRequestBehavior.AllowGet);
        //    //  return View();
        //    //return Json(
        //    //   jsonresult
        //    //    , JsonRequestBehavior.AllowGet);
        //    return Json(JsonConvert.SerializeObject(new { item = ds.Tables[0] }), JsonRequestBehavior.AllowGet);
        //    //JsonRequestBehavior.AllowGet);


        //    //      { "data": "Id" },
        //    //{ "data": "Name" },
        //    //{ "data": "ShortName" },
        //    //{ "data": "Description"},
        //    //{ "data": "Owner"},
        //    //{ "data": "ModifiedDate"}

        //}

    }
}