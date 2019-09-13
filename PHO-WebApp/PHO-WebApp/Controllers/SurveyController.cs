﻿using System;
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
        public ActionResult LoadExistingSurvey(int id, int formResponseId)
        {
            SurveyForm model = records.LoadSurveyQuestions(id, formResponseId);
            return View("Display", model);
        }
        public ActionResult ViewSurveyDetails(int id, string message)
        {
            SurveySummary model = records.GetSurveySummary(id);
            ViewBag.CompletedSurveys = records.GetSurveyResponses(id, true);
            ViewBag.InProgressSurveys = records.GetSurveyResponses(id, false);

            if (!string.IsNullOrWhiteSpace(message))
            {
                ViewBag.Message = message;
            }
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
                SaveForm(model);

                return ViewSurveyDetails(model.FormId, "Data collection was saved successfully. ID = " + model.FormResponseId.ToString());
            }
            else
            {
                return View("Display", model);
            }
        }


        public void SaveForm(SurveyForm model)
        {
            model.FormResponseId = records.SaveSurveyFormResponse(model.FormId, model.FormResponseId, model.PercentComplete, false);

            foreach (QuestionResponse response in model.Responses)
            {
                int responseId = records.SaveSurveyResponse(response, model.FormResponseId);

                if (response.IsListQuestionType && response.ResponseAnswer != null && response.ResponseAnswer.AnswerOptionId > 0)
                {
                    response.ResponseAnswer.ResponseId = responseId;
                    records.SaveSurveyResponseAnswer(response.ResponseAnswer);
                }
            }
        }

    }
}