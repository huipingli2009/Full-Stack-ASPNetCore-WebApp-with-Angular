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
    public class SurveyController : Controller
    {
        DataAccessLayer.SurveyDAL records = new DataAccessLayer.SurveyDAL();

        // GET: Survey
        public ActionResult Index()
        {
            List<SurveySummary> model = records.GetSurveySummaries();
            return View(model);
        }

        public ActionResult ViewSurvey(int id)
        {
            SurveyForm model = records.GetSurveyQuestions(id);
            return View("Display", model);
        }

        public ActionResult DisplaySection(int id)
        {
            SurveyForm model = records.GetSurveyQuestions(id);
            return View("DisplaySection", model);
        }

    }
}