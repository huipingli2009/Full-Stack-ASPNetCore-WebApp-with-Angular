using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PHO_WebApp.Models;
using PHO_WebApp.DataAccessLayer;


namespace PHO_WebApp.Controllers
{
    public class MeasureQualityImprovementController : Controller
    {
        MeasureDAL MDAL = new MeasureDAL();
        QualityImprovementDAL QIDAL = new QualityImprovementDAL(); 
        
        // GET: MeasureQualityImprovement
        public ActionResult Index()
        {
            int userId = 2;

            //get all QuealityImprovements
            QualityImprovement QI = QIDAL.getAllQualityImprovementsForPractice(userId);
                       
            MeasurePracticeUserModels model = new MeasurePracticeUserModels();
            model.MeasureDetail = new Measure();
            model.QualityImprovementDetail = new QualityImprovement();                     

            //get QualityImprovementDetail first before moving to next level
            QIDAL.getAllQualityImprovementsForPractice(userId);
            
            return View(model);
        }
    }
}