using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using PHO_WebApp.Models;

namespace PHO_WebApp.DataAccessLayer
{
    public class SurveyDAL
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public List<SurveySummary> GetSurveySummaries()
        {
            List<SurveySummary> returnObject = new List<SurveySummary>();

            SqlCommand com = new SqlCommand("spGetSurveySummaries", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            //Cohorts = ds.Tables[0].
            da.Fill(ds);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    SurveySummary summary = CreateSurveySummaryModel(
                            SharedLogic.ParseNumeric(r["FormId"].ToString()),
                            r["Survey_Title"].ToString(),
                            r["Description"].ToString());

                    returnObject.Add(summary);
                }
            }

            return returnObject;
        }

        public SurveyForm GetSurveyQuestions(int id)
        {
            SurveyForm returnObject = null;

            SqlCommand com = new SqlCommand("spGetSurveyQuestions", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add(new SqlParameter("@FormId", id));

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            //Cohorts = ds.Tables[0].
            da.Fill(ds);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                //track various ids
                int formId = 0;
                int formSectionId = 0;
                int sectionId = 0;
                int sectionQuestionId = 0;
                int questionId = 0;
                //int questionAnswerOptionsId = 0;

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    if (formId != SharedLogic.ParseNumeric(r["FormId"].ToString()))
                    {
                        //new form
                        formId = SharedLogic.ParseNumeric(r["FormId"].ToString());
                        //Create new SurveyForm model
                        returnObject = CreateSurveyFormModel((int?)r["FormId"], r["Survey_Title"].ToString());

                    }
                    if (formSectionId != SharedLogic.ParseNumeric(r["FormSectionId"].ToString()))
                    {
                        //new form section
                        formSectionId = SharedLogic.ParseNumeric(r["FormSectionId"].ToString());
                        FormSection newFS = CreateFormSectionModel(
                            SharedLogic.ParseNumeric(r["FormSectionId"].ToString()), 
                            SharedLogic.ParseNumeric(r["FormSectionOrder"].ToString()));
                                               
                        returnObject.FormSections.Add(newFS);
                    }
                    if (sectionId != SharedLogic.ParseNumeric(r["SectionId"].ToString()))
                    {
                        sectionId = SharedLogic.ParseNumeric(r["SectionId"].ToString());
                        Section newS = CreateSectionModel(
                            SharedLogic.ParseNumeric(r["SectionId"].ToString()), 
                            r["SectionDescription"].ToString());
                        returnObject.LastFormSection.Sections.Add(newS);
                    }
                    if (sectionQuestionId != SharedLogic.ParseNumeric(r["SectionQuestionsId"].ToString()))
                    {
                        sectionQuestionId = SharedLogic.ParseNumeric(r["SectionQuestionsId"].ToString());
                        SectionQuestion newSQ = CreateSectionQuestionModel(
                            SharedLogic.ParseNumeric(r["SectionQuestionsId"].ToString()), 
                            SharedLogic.ParseNumeric(r["SectionQuestionOrder"].ToString()));
                        returnObject.LastFormSection.LastSection.SectionQuestions.Add(newSQ);
                    }
                    if (questionId != SharedLogic.ParseNumeric(r["QuestionId"].ToString()))
                    {
                        questionId = SharedLogic.ParseNumeric(r["QuestionId"].ToString());
                        Question newQ = CreateQuestionModel(
                            SharedLogic.ParseNumeric(r["QuestionId"].ToString()), 
                            SharedLogic.ParseNumeric(r["QuestionTypeId"].ToString()), 
                            r["QuestionType"].ToString(), 
                            r["Flag_Required"].ToString(),
                            r["QuestionLabel"].ToString(),
                            r["Label_Code"].ToString(), 
                            r["Javascript"].ToString());
                        returnObject.LastFormSection.LastSection.LastSectionQuestion.Questions.Add(newQ);
                    }
                }


            }

            return returnObject;
        }


        public SurveySummary CreateSurveySummaryModel(int? SurveyFormId, string Survey_Title, string Description)
        {
            SurveySummary c = new SurveySummary();
            if (SurveyFormId.HasValue)
            {
                c.FormId = SurveyFormId.Value;
            }
            c.Survey_Title = Survey_Title;
            c.Description = Description;
            return c;
        }


        public SurveyForm CreateSurveyFormModel(int? SurveyFormId, string Survey_Title)
        {
            SurveyForm c = new SurveyForm();
            if (SurveyFormId.HasValue)
            {
                c.FormId = SurveyFormId.Value;
            }
            c.Survey_Title = Survey_Title;

            return c;
        }

        public FormSection CreateFormSectionModel(int? FormSectionId, int Order)
        {
            FormSection c = new FormSection();
            if (FormSectionId.HasValue)
            {
                c.FormSectionId = FormSectionId.Value;
            }
            c.Order = Order;
            

            return c;
        }

        public Section CreateSectionModel(int? SectionId, string SectionDescription)
        {
            Section c = new Section();
            if (SectionId.HasValue)
            {
                c.SectionId = SectionId.Value;
            }
            c.SectionDescription = SectionDescription;

            return c;
        }

        public SectionQuestion CreateSectionQuestionModel(int? SectionQuestionId, int Order)
        {
            SectionQuestion c = new SectionQuestion();
            if (SectionQuestionId.HasValue)
            {
                c.SectionQuestionId = SectionQuestionId.Value;
            }
            c.Order = Order;


            return c;
        }

        public Question CreateQuestionModel(int? QuestionId, int? QuestionTypeId, string QuestionType, string Flag_Required, string QuestionLabel, string LabelCode, string Javascript)
        {
            Question c = new Question();
            if (QuestionId.HasValue)
            {
                c.QuestionId = QuestionId.Value;
            }
            if (QuestionTypeId.HasValue)
            {
                c.QuestionTypeId = QuestionTypeId.Value;
            }

            c.Flag_Required = Flag_Required;
            c.QuestionLabel = QuestionLabel;
            c.LabelCode = LabelCode;
            c.Javascript = Javascript;

            return c;
        }


    }
}