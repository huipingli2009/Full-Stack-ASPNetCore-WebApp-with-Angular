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

        public List<SurveySummary> GetSurveySummaries(int practiceId)
        {
            List<SurveySummary> returnObject = new List<SurveySummary>();

            SqlCommand com = new SqlCommand("spGetSurveySummaries", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add(new SqlParameter("@PracticeId", practiceId));

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            
            da.Fill(ds);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    int formId = SharedLogic.ParseNumeric(r["FormId"].ToString());
                    SurveySummary summary = CreateSurveySummaryModel(
                            formId,
                            r["Survey_Title"].ToString(),
                            r["Description"].ToString(),
                            SharedLogic.ParseNumeric(r["TotalResponses"].ToString()),
                            SharedLogic.ParseNumeric(r["CompletedResponses"].ToString()),
                            SharedLogic.ParseNumeric(r["InProgressResponses"].ToString()));

                    summary.RecentInProgress = GetSurveyResponses(formId, false).OrderByDescending(c => c.DateModified).Take(5).ToList();

                    returnObject.Add(summary);
                }
            }

            return returnObject;
        }

        public List<SurveyFormResponse> GetSurveyResponses(int formId, bool completed)
        {
            List<SurveyFormResponse> returnObject = new List<SurveyFormResponse>();

            SqlCommand com = new SqlCommand("spGetSurveyResponses", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add(new SqlParameter("@FormId", formId));
            com.Parameters.Add(new SqlParameter("@Completed", completed));


            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            
            da.Fill(ds);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    SurveyFormResponse resp = CreateSurveyFormResponse(
                            SharedLogic.ParseNumeric(r["FormId"].ToString()),
                            SharedLogic.ParseNumeric(r["FormResponseId"].ToString()),
                            SharedLogic.ParseNumeric(r["PercentCompleted"].ToString()),
                            Convert.ToBoolean(r["Completed"].ToString()),
                            SharedLogic.ParseDateTime(r["DateCompleted"].ToString()),
                            SharedLogic.ParseDateTime(r["DateCreated"].ToString()),
                            SharedLogic.ParseDateTime(r["DateModified"].ToString())
                            );

                    returnObject.Add(resp);
                }
            }

            return returnObject;
        }

        public SurveySummary GetSurveySummary(int id)
        {
            SurveySummary returnObject = new SurveySummary();

            SqlCommand com = new SqlCommand("spGetSurveySummary", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add(new SqlParameter("@FormId", id));


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
                            r["Description"].ToString(),
                            SharedLogic.ParseNumeric(r["TotalResponses"].ToString()),
                            SharedLogic.ParseNumeric(r["CompletedResponses"].ToString()),
                            SharedLogic.ParseNumeric(r["InProgressResponses"].ToString()));

                    returnObject = summary;
                }
            }

            return returnObject;
        }

        public SurveyForm LoadSurveyQuestions(int id, int? formResponseId)
        {
            SurveyForm returnObject = null;

            SqlCommand com = new SqlCommand("spGetSurveyQuestionsExisting", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add(new SqlParameter("@FormId", id));
            if (formResponseId.HasValue)
            {
                com.Parameters.Add(new SqlParameter("@FormResponseId", formResponseId));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@FormResponseId", -1));
            }

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);

            returnObject = CreateSurveyObjectStructure(ds);

            return returnObject;
        }
        public SurveyForm LoadSurveyQuestions(int id)
        {
            SurveyForm returnObject = null;

            SqlCommand com = new SqlCommand("spGetSurveyQuestionsExisting", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add(new SqlParameter("@FormId", id));
            com.Parameters.Add(new SqlParameter("@FormResponseId", -1));

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);

            returnObject = CreateSurveyObjectStructure(ds);

            return returnObject;
        }

        public SurveyForm CreateSurveyObjectStructure(DataSet ds)
        {
            SurveyForm returnObject = null;

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
                        returnObject = CreateSurveyFormModel(formId, SharedLogic.ParseNumeric(r["FormResponseID"].ToString()), r["Survey_Title"].ToString());

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
                        QuestionResponse newQ = CreateQuestionResponseModel(
                            SharedLogic.ParseNumeric(r["QuestionId"].ToString()),
                            SharedLogic.ParseNumeric(r["QuestionTypeId"].ToString()),
                            r["QuestionType"].ToString(),
                            Convert.ToBoolean(r["Flag_Required"].ToString()),
                            r["QuestionLabel"].ToString(),
                            r["Label_Code"].ToString(),
                            r["Javascript"].ToString(),
                            SharedLogic.ParseNumericNullable(r["ResponseId"].ToString()),
                            r["Response_Text"].ToString(),
                            SharedLogic.ParseNumericNullable(r["ResponseAnswersId"].ToString()),
                            SharedLogic.ParseNumericNullable(r["AnswerOptionId"].ToString()));
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


        public int SaveSurveyFormResponse(int FormId, int FormResponseId, int PercentCompleted, bool Completed)
        {
            int returnValue = 0;

            SqlCommand com = null;
            if (FormResponseId > 0)
            {
                com = new SqlCommand("spUpdateSurveyFormResponse", con);
                com.Parameters.Add("@FormResponseId", SqlDbType.Int);
                com.Parameters["@FormResponseId"].Value = FormResponseId;
            }
            else
            {
                com = new SqlCommand("spInsertSurveyFormResponse", con);
            }

            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add("@FormId", SqlDbType.Int);
            com.Parameters.Add("@PercentComplete", SqlDbType.Int);
            com.Parameters.Add("@Completed", SqlDbType.Bit);
            com.Parameters.Add("@DateCompleted", SqlDbType.DateTime);

            if (FormId > 0)
            {
                com.Parameters["@FormId"].Value = FormId;
            }
            else
            {
                com.Parameters["@FormId"].Value = DBNull.Value;
            }

            if (PercentCompleted > 0)
            {
                com.Parameters["@PercentComplete"].Value = FormId;
            }
            else
            {
                com.Parameters["@PercentComplete"].Value = 0;
            }

            if (Completed)
            {
                com.Parameters["@Completed"].Value = FormId;
                com.Parameters["@DateCompleted"].Value = DateTime.Now;
            }
            else
            {
                com.Parameters["@Completed"].Value = 0;
                com.Parameters["@DateCompleted"].Value = DBNull.Value;
            }

            con.Open();
            object obj = com.ExecuteScalar();
            returnValue = (Int32)obj;
            con.Close();

            return returnValue;
        }

        public int SaveSurveyResponse(QuestionResponse model, int FormResponseId)
        {
            int returnValue = 0;

            SqlCommand com = null;
            if (model.ResponseId > 0)
            {
                com = new SqlCommand("spUpdateSurveyResponse", con);
                com.Parameters.Add("@ResponseId", SqlDbType.Int);
                com.Parameters["@ResponseId"].Value = model.ResponseId;
            }
            else
            {
                com = new SqlCommand("spInsertSurveyResponse", con);
            }
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add("@QuestionId", SqlDbType.Int);
            com.Parameters.Add("@FormResponseId", SqlDbType.Int);
            com.Parameters.Add("@ResponseText", SqlDbType.VarChar);

            if (FormResponseId > 0)
            {
                com.Parameters["@FormResponseId"].Value = FormResponseId;
            }
            else
            {
                com.Parameters["@FormResponseId"].Value = DBNull.Value;
            }

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
            object obj = com.ExecuteScalar();
            returnValue = (Int32)obj;
            con.Close();

            return returnValue;
        }


        public int SaveSurveyResponseAnswer(ResponseAnswer model)
        {
            int returnValue = 0;

            SqlCommand com = null;
            if (model.ResponseAnswersId > 0)
            {
                com = new SqlCommand("spUpdateSurveyResponseAnswer", con);
                com.Parameters.Add("@ResponseAnswersId", SqlDbType.Int);
                com.Parameters["@ResponseAnswersId"].Value = model.ResponseAnswersId;
            }
            else
            {
                com = new SqlCommand("spInsertSurveyResponseAnswer", con);
            }
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


        public SurveySummary CreateSurveySummaryModel(int? SurveyFormId, string Survey_Title, string Description, int TotalResponses, int CompletedResponses, int InProgressResponses)
        {
            SurveySummary c = new SurveySummary();
            if (SurveyFormId.HasValue)
            {
                c.FormId = SurveyFormId.Value;
            }
            c.Survey_Title = Survey_Title;
            c.Description = Description;
            c.CompletedResponses = CompletedResponses;
            c.InProgressResponses = InProgressResponses;
            c.TotalResponses = TotalResponses;

            return c;
        }


        public SurveyForm CreateSurveyFormModel(int SurveyFormId, int FormResponseId, string Survey_Title)
        {
            SurveyForm c = new SurveyForm();
            if (SurveyFormId > 0)
            {
                c.FormId = SurveyFormId;
            }
            if (FormResponseId > 0)
            {
                c.FormResponseId = 0;
            }
            else
            {
                c.FormResponseId = 0;
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

        public QuestionResponse CreateQuestionResponseModel(int? QuestionId, int? QuestionTypeId, string QuestionType, bool Flag_Required, string QuestionLabel, string LabelCode, string Javascript, int? ResponseId, string Response_Text, int? ResponseAnswersId, int? AnswerOptionId)
        {
            QuestionResponse c = new QuestionResponse();
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

            //c.Response = CreateResponseModel(QuestionId, string.Empty, Flag_Required);
            c.Response_Text = string.Empty;
            c.ResponseId = -1;
            c.FormResponseId = -1;

            if (ResponseId.HasValue)
            {
                c.ResponseId = ResponseId.Value;
                c.Response_Text = Response_Text;
            }

            if (c.IsListQuestionType)
            {
                c.ResponseAnswer = CreateResponseAnswerModel(c.ResponseId, ResponseAnswersId, AnswerOptionId);
            }

            return c;
        }


        public ResponseAnswer CreateResponseAnswerModel(int? ResponseId, int? ResponseAnswersId, int? AnswerOptionId)
        {
            ResponseAnswer r = new ResponseAnswer();
            if (AnswerOptionId.HasValue)
            {
                r.ResponseId = AnswerOptionId.Value;
            }
            if (ResponseAnswersId.HasValue)
            {
                r.ResponseAnswersId = ResponseAnswersId.Value;
            }
            if (ResponseId.HasValue)
            {
                r.AnswerOptionId = AnswerOptionId.Value;
            }
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

        public SurveyFormResponse CreateSurveyFormResponse(int FormId, int FormResponseId, int PercentCompleted, bool Completed, DateTime DateCompleted, DateTime DateCreated, DateTime DateModified)
        {
            SurveyFormResponse sfr = new SurveyFormResponse();

            sfr.FormId = FormId;
            sfr.FormResponseId = FormResponseId;
            sfr.PercentCompleted = PercentCompleted;
            sfr.Completed = Completed;
            sfr.DateCompleted = DateCompleted;
            sfr.DateCreated = DateCreated;
            sfr.DateModified = DateModified;

            return sfr;
        }


    }
}