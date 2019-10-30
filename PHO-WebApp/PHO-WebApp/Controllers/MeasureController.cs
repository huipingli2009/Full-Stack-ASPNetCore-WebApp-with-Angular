using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PHO_WebApp.DataAccessLayer;
using PHO_WebApp.Models;
using System.Dynamic;   //new reference

namespace PHO_WebApp.Controllers
{
    public class MeasureController : BaseController
    {
        DataAccessLayer.MeasureDAL MD = new MeasureDAL();
        DataAccessLayer.LoginDAL UL = new LoginDAL();
        DataAccessLayer.QualityImprovementDAL QIDAL = new QualityImprovementDAL();
        // GET: Measure
        public ActionResult Index()
        {
            List<Measure> AllMeasureRecords = new List<Measure>();
            AllMeasureRecords = MD.getAllMeasures(SavedUserDetails);

            ////New development secion starts here ----------------------
            //int loginId = 2;
            //var model = new MeasurePracticeUserModels();
            //model.MeasureDetail = MD.getAllMeasures();
            //model.QualityImprovementDetail = QIDAL.getAllQualityImprovementsForPractice(loginId);



            ////model.UD = UL.GetPersonLoginForLoginId(loginId);
            ////model.UD = UL.GetPersonLoginForLoginId_new();
            //return View(model);

            ////New development secion ends here ----------------------

            //List<Measure> AllMeasureRecords = new List<Measure>();
            //AllMeasureRecords = MD.getAllMeasures();
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