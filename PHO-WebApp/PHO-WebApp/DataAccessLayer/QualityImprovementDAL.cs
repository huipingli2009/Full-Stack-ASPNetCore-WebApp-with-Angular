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
    public class QualityImprovementDAL
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public QualityImprovement getAllQualityImprovementsForPractice(int userId)
        {
            SqlCommand com = new SqlCommand("spQI_Summary", con);
            com.CommandType = CommandType.StoredProcedure;

            //Add practice Id parameter
            SqlParameter parameterUserId = new SqlParameter("@UserId", SqlDbType.Int, 4);
            parameterUserId.Value = userId;
            com.Parameters.Add(parameterUserId);

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            da.Fill(ds);         

            List<QualityImprovement> QualityImprovementRecords = new List<QualityImprovement>();

            DataAccessLayer.SurveyDAL SurveyDAL = new SurveyDAL();
            DataAccessLayer.CohortDAL CohortDAL = new CohortDAL();
            DataAccessLayer.InitiativeDAL InitDAL = new InitiativeDAL();

            //track various ids
            int initiativeId = 0;
            int cohortId = 0;
            int practiceId = 0;

            QualityImprovement QIC = new QualityImprovement();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                //Set the practiceID
                if (ds.Tables[0].Rows[i]["PracticeID"] != null)
                {
                    practiceId = SharedLogic.ParseNumeric(ds.Tables[0].Rows[i]["PracticeID"].ToString());
                    QIC.PracticeId = practiceId;                    
                }

                //Pull Init
                if (ds.Tables[0].Rows[i]["InitiativeId"] != null)
                {
                    int curInitiativeId = SharedLogic.ParseNumeric(ds.Tables[0].Rows[i]["InitiativeId"].ToString());

                    if (curInitiativeId != initiativeId)
                    {
                        initiativeId = curInitiativeId;
                        Initiative intObj = InitDAL.CreateInitiativeModel(ds.Tables[0].Rows[i]["InitiativeId"], ds.Tables[0].Rows[i]["Initiative"], ds.Tables[0].Rows[i]["Initiative"], ds.Tables[0].Rows[i]["InitiativeDesc"], null, null, null, null, null, ds.Tables[0].Rows[i]["InitiativeStatus"], null, ds.Tables[0].Rows[i]["InitiativeOwner"]);
                        QIC.Initiatives.Add(intObj);
                    }
                }
                //Pull Cohort
                if (ds.Tables[0].Rows[i]["CohortId"] != null)
                {
                    int curCohortId = SharedLogic.ParseNumeric(ds.Tables[0].Rows[i]["CohortId"].ToString());

                    if (curCohortId != cohortId)
                    {
                        cohortId = curCohortId;
                        Cohort cohortObj = CohortDAL.CreateCohortModel(ds.Tables[0].Rows[i]["CohortId"], ds.Tables[0].Rows[i]["Cohort"], ds.Tables[0].Rows[i]["Cohort"], ds.Tables[0].Rows[i]["CohortDesc"],  null, null, null, null, ds.Tables[0].Rows[i]["CohortDataSources"], ds.Tables[0].Rows[i]["CohortLookback"], ds.Tables[0].Rows[i]["CohortStatus"], null, null, null, null);
                        QIC.Cohorts.Add(cohortObj);
                    }
                }

                //Survey Summaries
                QIC.SurveySummaries = SurveyDAL.GetSurveySummaries(practiceId);
            }

            return QIC;
        }

        public QualityImprovement CreateQualityImprovementModel(DataRow dr)
        {
            QualityImprovement QI = new QualityImprovement();

            if (dr["Initiative"] != null && !string.IsNullOrWhiteSpace(dr["Initiative"].ToString()))
            {                 
                QI.Name = dr["Name"].ToString();
                QI.Desc = dr["Desc"].ToString();
                QI.Initiative = dr["Initiative"].ToString();
                QI.InitiativeDesc = dr["InitiativeDesc"].ToString();
                QI.InitiativeOwner = dr["InitiativeOwner"].ToString();
                QI.InitiativeStatus = dr["InitiativeStatus"].ToString();

                QI.CohortName = dr["Cohort"].ToString();
                QI.CohortDesc = dr["CohortDesc"].ToString();
                QI.CohortLookback = dr["CohortLookback"].ToString();
                QI.CohortStatus = dr["CohortStatus"].ToString();

                QI.MeasureId = int.Parse(dr["MeasureId"].ToString());
                QI.MeasureName = dr["MeasureName"].ToString();
                QI.MeasureDesc = dr["MeasureDesc"].ToString();
                //QI.MeasureStatus  = dr["MeasureStatus"].ToString();
                //QI.MeasureFrequency = dr["MeasureFrequency"].ToString();
                QI.MeasureNumeratorDesc = dr["Numerator"].ToString();
                QI.MeasureDenominatorDesc = dr["Denominator"].ToString();

                QI.MeasueNumerator = int.Parse(dr["MeasureNumerator"].ToString());
                QI.N = dr["(n)"].ToString();
                QI.Factor = int.Parse(dr["Factor"].ToString());

                //QI.MeasureDenominator = int.Parse(dr["MeasureDenominator"].ToString());

                if (dr["LastMeasureDate"].ToString() != "")
                {
                    QI.LastMeasureDate = Convert.ToDateTime(dr["LastMeasureDate"].ToString());
                }

                QI.MeasueValue = decimal.Parse(dr["MeasureValue"].ToString());
                QI.MeasureN = int.Parse(dr["MeasureN"].ToString());
            }

            return QI;
        }
    }    
}