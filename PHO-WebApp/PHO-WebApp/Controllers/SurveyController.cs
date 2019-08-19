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

        [HttpPost]
        public ActionResult Save(SurveyForm model)
        {
            if (ModelState.IsValid && model != null && model.Responses != null)
            {
                foreach(Response response in model.Responses)
                {
                    int responseId = records.InsertSurveyResponse(response);

                    if(response.ResponseAnswer != null)
                    {
                        response.ResponseAnswer.ResponseId = responseId;
                        records.InsertSurveyResponseAnswer(response.ResponseAnswer);
                    }
                }
            }
            return View("SurveyCompleted");
        }

    }
}