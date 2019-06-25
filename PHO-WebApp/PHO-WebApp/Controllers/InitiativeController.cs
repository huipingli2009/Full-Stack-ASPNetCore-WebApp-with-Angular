using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PHO_WebApp.DataAccessLayer;
using System.Data;
using PHO_WebApp.Models;
//comment

namespace PHO_WebApp.Controllers
{
    public class InitiativeController : Controller
    {
        DataAccessLayer.InitiativeDAL initiativeRecords = new InitiativeDAL();
        // GET: Initiative
        public ActionResult Index()
        {
            List<Initiative> PHOInitiatives = new List<Initiative>();
            PHOInitiatives = initiativeRecords.getAllInitiatives();

            return View(PHOInitiatives);
        }

        public PartialViewResult _InitiativeDisplay()
        {
            return PartialView ("_InitiativeDisplay");
        }
    }
}