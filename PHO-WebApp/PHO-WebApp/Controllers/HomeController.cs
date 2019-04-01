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
            return View("Security");
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
            DataSet ds = records.GetDataDictionaryRecords();
            ViewBag.DataDictionary = ds.Tables[0];

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

            return View("DataDictionary");           
        }
    }
}