using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PHO_WebApp.DataAccessLayer;
using PHO_WebApp.Models;

namespace PHO_WebApp.Controllers
{
    public class MeasureController : Controller
    {
        DataAccessLayer.MeasureDAL MD = new MeasureDAL();
        // GET: Measure
        public ActionResult Index()
        {
            List<Measure> AllMeasureRecords = new List<Measure>();
            AllMeasureRecords = MD.getAllMeasures();
            return View(AllMeasureRecords);
        }

        //this displays content of summary page
        public PartialViewResult _MeasureDisplay()
        {
            return PartialView("_MeasureDisplay");
        } 
       
        public ActionResult QIMeasurePath(int MeasureRecordId)
        {
            Measure model = null;
            if (MeasureRecordId > 0)
            {
                model = this.MD.getQIMeasureById(MeasureRecordId);
            }
            else
            {
                model = this.MD.CreateMeasureModel();
            }

            return PartialView("_QIMeasurePath", model);
        }       

        [HttpPost]
        public ActionResult SaveQIMeasure(Measure M, string StatusListItems)
        {
            if (this.ModelState.IsValid)
            {
                if (M.MeasureId > 0)
                {
                    MD.UpdateQIMeasure(M);

                }
                else
                {
                    MD.AddQIMeasureModel(M);                 
                }

                return RedirectToAction("Index");
            }

            return View();
        }
    }
}