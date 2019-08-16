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
            
            da.Fill(ds);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                //track various ids
                int formId = 0;
                int formSectionId = 0;
                int sectionId = 0;
                int sectionQuestionId = 0;
                int questionId = 0;
                int questionAnswerOptionsId = 0;

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
                            Convert.ToBoolean(r["Flag_Required"].ToString()),
                            r["QuestionLabel"].ToString(),
                            r["Label_Code"].ToString(), 
                            r["Javascript"].ToString());
                        returnObject.LastFormSection.LastSection.LastSectionQuestion.Question = newQ;
                    }
                    if (r["QuestionAnswerOptionsId"] != null && !string.IsNullOrWhiteSpace(r["QuestionAnswerOptionsId"].ToString()) && questionAnswerOptionsId != SharedLogic.ParseNumeric(r["QuestionAnswerOptionsId"].ToString()))
                    {
                        questionAnswerOptionsId = SharedLogic.ParseNumeric(r["QuestionAnswerOptionsId"].ToString());
                        QuestionAnswerOption newQAO = CreateQuestionAnswerOption(
                            SharedLogic.ParseNumeric(r["QuestionId"].ToString()),
                            SharedLogic.ParseNumeric(r["QuestionAnswerOptionsId"].ToString()),
                            SharedLogic.ParseNumeric(r["AnswerOptionId"].ToString()),
                            SharedLogic.ParseNumeric(r["QuestionAnswerOptionOrder"].ToString()),
                            r["AnswerOptionLabel"].ToString());
                        if (returnObject.LastFormSection.LastSection.LastSectionQuestion.Question.QuestionAnswerOptions == null)
                        {
                            returnObject.LastFormSection.LastSection.LastSectionQuestion.Question.QuestionAnswerOptions = new List<QuestionAnswerOption>();
                        }
                        returnObject.LastFormSection.LastSection.LastSectionQuestion.Question.QuestionAnswerOptions.Add(newQAO);
                    }
                }


            }

            return returnObject;
        }


        public int InsertSurveyResponse(Response model)
        {
            int returnValue = 0;

            SqlCommand com = new SqlCommand("spInsertSurveyResponse", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add("@QuestionId", SqlDbType.Int);
            com.Parameters.Add("@ResponseText", SqlDbType.VarChar);

            if (model.QuestionId > 0)
            {
                com.Parameters["@QuestionId"].Value = model.QuestionId;
            }
            else
            {
                com.Parameters["@QuestionId"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Response_Text))
            {
                com.Parameters["@ResponseText"].Value = model.Response_Text;
            }
            else
            {
                com.Parameters["@ResponseText"].Value = DBNull.Value;
            }
            
            con.Open();
            returnValue = (Int32)com.ExecuteScalar();
            con.Close();

            return returnValue;
        }


        public int InsertSurveyResponseAnswer(ResponseAnswer model)
        {
            int returnValue = 0;

            SqlCommand com = new SqlCommand("spInsertSurveyResponseAnswer", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add("@ResponseId", SqlDbType.Int);
            com.Parameters.Add("@AnswerOptionId", SqlDbType.Int);

            if (model.ResponseId > 0)
            {
                com.Parameters["@ResponseId"].Value = model.ResponseId;
            }
            else
            {
                com.Parameters["@ResponseId"].Value = DBNull.Value;
            }
            if (model.AnswerOptionId > 0)
            {
                com.Parameters["@AnswerOptionId"].Value = model.AnswerOptionId;
            }
            else
            {
                com.Parameters["@AnswerOptionId"].Value = DBNull.Value;
            }

            con.Open();
            returnValue = (Int32)com.ExecuteScalar();
            con.Close();

            return returnValue;
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

        public Question CreateQuestionModel(int? QuestionId, int? QuestionTypeId, string QuestionType, bool Flag_Required, string QuestionLabel, string LabelCode, string Javascript)
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
            c.QuestionType = QuestionType;
            c.Flag_Required = Flag_Required;
            c.QuestionLabel = QuestionLabel;
            c.LabelCode = LabelCode;
            c.Javascript = Javascript;

            c.Response = CreateResponseModel(QuestionId, string.Empty, Flag_Required);

            if (c.IsListQuestionType)
            {
                c.Response.ResponseAnswer = CreateResponseAnswerModel(0, 0);
            }

            return c;
        }

        public Response CreateResponseModel(int? QuestionId, string Response_Text, bool required)
        {
            Response r = new Response();
            r.QuestionId = QuestionId.Value;
            r.Response_Text = Response_Text;
            r.Required = required;
            return r;
        }

        public ResponseAnswer CreateResponseAnswerModel(int? ResponseId, int? AnswerOptionId)
        {
            ResponseAnswer r = new ResponseAnswer();
            r.ResponseId = AnswerOptionId.Value;
            r.AnswerOptionId = AnswerOptionId.Value;
            return r;
        }

        public QuestionAnswerOption CreateQuestionAnswerOption(int? QuestionId, int? QuestionAnswerOptionsId, int? AnswerOptionId, int? order, string label)
        {
            QuestionAnswerOption qao = new QuestionAnswerOption();

            qao.QuestionAnswerOptionId = QuestionAnswerOptionsId.Value;
            qao.QuestionId = QuestionId.Value;
            qao.AnswerOptionId = AnswerOptionId.Value;
            qao.QuestionAnswerOptionLabel = label;

            return qao;
        }


    }
}