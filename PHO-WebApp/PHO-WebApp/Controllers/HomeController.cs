using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PHO_WebApp.DataAccessLayer;
using System.Data;
//using PHO_WebApp.Models;


namespace PHO_WebApp.Controllers
{
    public class HomeController : Controller
    {
        DataAccessLayer.DataDictionary records = new DataDictionary();
        public ActionResult Index()
        {
            return View();
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
        //public ActionResult DataDictionary()
        //{
        //    DataSet ds = records.GetDataDictionaryRecords();
        //    ViewBag.DataDictionary = ds.Tables[0];

        //    return View("DataDictionary");
        //}

        public ActionResult DataDictionary(string DatabaseName, string SchemaName, string ObjectName, string ObjectType, string ColumnName, string SQLColumnDescription, string IsNullable, string DataType, string PHIFlag, string BusinessDefiniton)
        {
            //Build PHI Flag
            bool? bPHIFlag = null;
            if (!string.IsNullOrWhiteSpace(PHIFlag))
            {
                if (PHIFlag == "1")
                    bPHIFlag = true;
                if (PHIFlag == "0")
                    bPHIFlag = false;
            }

            //Execute Query
            DataSet ds = records.GetDataDictionaryRecordsWithSearchCriteria(DatabaseName, SchemaName, ObjectName, ObjectType, ColumnName, SQLColumnDescription, IsNullable, DataType, bPHIFlag, BusinessDefiniton);
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

            //Response.Redirect(Url.Action("DataDictionary", "HomeController"));
            //return View("");

            return View("DataDictionary");

        }
    }
}