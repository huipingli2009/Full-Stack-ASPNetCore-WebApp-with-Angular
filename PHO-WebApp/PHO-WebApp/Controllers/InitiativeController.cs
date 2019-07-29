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
    public class InitiativeController : BaseController
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

        public ActionResult InitiativePartialPath(int InitiativeId)
        {
            Initiative model = null;
            if (InitiativeId > 0)
            {
                model = this.initiativeRecords.getInitiativeRecordById(InitiativeId);
            }
            else
            {
                model = this.initiativeRecords.CreateInitiativeModel();
            }

            return PartialView("_InitiativePartialPath", model);
            //return PartialView("InitiativeModal", model);
        }  
        
        [HttpPost]
        public ActionResult SaveInitiative(Initiative I, string CohortStatusListItems)
        {
            if (this.ModelState.IsValid)
            {
                if (I.id > 0)
                {
                    initiativeRecords.UpdateInitiative(I);

                }
                else
                {
                    initiativeRecords.AddInitiativeModel(I);
                }

                return RedirectToAction("Index");
            }

            return View();            
        }
    }
}