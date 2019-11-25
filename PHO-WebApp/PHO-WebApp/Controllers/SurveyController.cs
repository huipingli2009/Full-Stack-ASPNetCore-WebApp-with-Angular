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
    public class SurveyController : BaseController
    {
        DataAccessLayer.SurveyDAL records = new DataAccessLayer.SurveyDAL();
        DataAccessLayer.StaffDAL staffData = new DataAccessLayer.StaffDAL();
        DataAccessLayer.Patients patientData = new DataAccessLayer.Patients();

        // GET: Survey
        public ActionResult Index()
        {
            List<SurveySummary> model = records.GetSurveySummaries(7);
            return View(model);
        }

        public ActionResult Display(int id, int? formResponseId)
        {
            //Get the entire survey tree structure from DB, load existing responses if applies
            SurveyForm model = records.LoadSurveyQuestions(id, formResponseId);

            //if this came in null, reset it to 0 for now. We need a value for the sake of caching.
            if (formResponseId == null)
            {
                formResponseId = 0;
            }

            //Put in cache so we don't lose model data between postbacks.
            Session["CachedSurvey_" + id.ToString() + "_" + formResponseId.ToString()] = model;

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


        [HttpPost]
        public ActionResult CloneSection(FormCollection fc, SurveyForm model, string sectionUniqueId)
        {
            model = PopulateDependencies(model);
            model = ProcessPhysicianLink(model, fc);

            model.CloneSection(sectionUniqueId);

            ModelState.Clear();
            return View("Display", model);
        }

        [HttpPost]
        public ActionResult Submit(FormCollection fc, SurveyForm model)
        {
            model = ProcessPhysicianLink(model, fc);
            model = ProcessPatientLink(model, fc);

            if (ModelState.IsValid && model != null && model.Responses != null)
            {
                SaveForm(model);

                return ViewSurveyDetails(model.FormId, "Data collection was saved successfully. ID = " + model.FormResponseId.ToString());
            }
            else
            {
                PopulateDependencies(model);
                return View("Display", model);
            }
        }

        private SurveyForm PopulateDependencies(SurveyForm model)
        {
            if (model != null && Session["CachedSurvey_" + model.FormId.ToString() + "_" + model.FormResponseId.ToString()] != null)
            {
                SurveyForm cachedSurvey = (SurveyForm)Session["CachedSurvey_" + model.FormId.ToString() + "_" + model.FormResponseId.ToString()];
                model.RefreshListFields(cachedSurvey);
            }
            return model;
        }

        private SurveyForm ProcessPhysicianLink(SurveyForm model, FormCollection fc)
        {
            Dictionary<string, string> dictionary = fc.AllKeys
            .Where(k => k.StartsWith("PhysicianStaffId"))
            .ToDictionary(k => k, k => fc[k]);

            List<Staff> staffList = staffData.getPracticeProviders(PracticeId);

            model.AssignPhysicianLinkKeys(dictionary, staffList);

            return model;
        }

        private SurveyForm ProcessPatientLink(SurveyForm model, FormCollection fc)
        {
            Dictionary<string, string> dictionary = fc.AllKeys
            .Where(k => k.StartsWith("PatientId"))
            .ToDictionary(k => k, k => fc[k]);

            List<Models.Patient> patientList = patientData.GetPatients(PracticeId);

            var PatientList = patientList.Where(c => dictionary.ContainsKey(c.patientId.ToString()))
                .Distinct().ToList();

            model.AssignPatientLinkKeys(dictionary, patientList);

            return model;
        }

        public void SaveForm(SurveyForm model)
        {
            if (model != null && model.Responses != null)
            {
                model.FormResponseId = records.SaveSurveyFormResponse(model.FormId, model.FormResponseId, model.PercentComplete, false, PracticeId);


                //Make sure an assigned staffID for the section is saved to the response as well
                if (model.FormSections != null)
                {
                    foreach (FormSection fs in model.FormSections)
                    {
                        if (fs.Sections != null)
                        {
                            foreach(Section s in fs.Sections)
                            {
                                if (s.SectionQuestions != null && s.PhysicianStaffId > 0)
                                {
                                    foreach (SectionQuestion sq in s.SectionQuestions)
                                    {
                                        if (sq.Question != null)
                                        {
                                            sq.Question.PhysicianStaffId = s.PhysicianStaffId;
                                        }
                                    }
                                }
                                if (s.SectionQuestions != null && s.PatientId > 0)
                                {
                                    foreach (SectionQuestion sq in s.SectionQuestions)
                                    {
                                        if (sq.Question != null)
                                        {
                                            sq.Question.PatientId = s.PatientId;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

                //Save the responses
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
}