using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHO_WebApp.Controllers
{
    public class HomeController : Controller
    {
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
            return View("Patient");
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

    }
}