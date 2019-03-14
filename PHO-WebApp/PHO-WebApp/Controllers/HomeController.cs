using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PHO_WebApp.DataAccessLayer;
using System.Data;


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

    }
}