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

        public ActionResult CompleteNewSurvey(int id)
        {
            SurveyForm model = records.LoadSurveyQuestions(id, 0);
            return View("Display", model);
        }
        public ActionResult LoadExistingSurvey(int id)
        {
            SurveyForm model = records.LoadSurveyQuestions(id, 0);
            return View("Display", model);
        }
        public ActionResult ViewSurveyDetails(int id)
        {
            SurveySummary model = records.GetSurveySummary(id);
            ViewBag.CompletedSurveys = records.GetSurveyResponses(id, true);
            ViewBag.InProgressSurveys = records.GetSurveyResponses(id, false);
            return View("Details", model);
        }

        public ActionResult DisplaySection(int id)
        {
            SurveyForm model = records.LoadSurveyQuestions(id, 0);
            return View("DisplaySection", model);
        }

        [HttpPost]
        public ActionResult Save(SurveyForm model)
        {
            if (ModelState.IsValid && model != null && model.Responses != null)
            {
                if (model.FormResponseId < 1)
                {
                    model.FormResponseId = records.InsertSurveyFormResponse(model.FormId, 0, false);
                }

                foreach(QuestionResponse response in model.Responses)
                {
                    int responseId = records.InsertSurveyResponse(response, model.FormResponseId);

                    if(response.IsListQuestionType && response.ResponseAnswer != null && response.ResponseAnswer.AnswerOptionId > 0)
                    {
                        response.ResponseAnswer.ResponseId = responseId;
                        records.InsertSurveyResponseAnswer(response.ResponseAnswer);
                    }
                }

                return View("SurveyCompleted");
            }
            else
            {
                return View(model);
            }
        }

    }
}