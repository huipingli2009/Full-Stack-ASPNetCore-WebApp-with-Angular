using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Data;
using System.Linq;


namespace org.cchmc.pho.core.DataAccessLayer
{

    public class MetricDAL : IMetric
    {
        //private IConfiguration _config;
        private ConnectionStrings _connectionStrings;
        public MetricDAL(IOptions<ConnectionStrings> options, ILogger<AlertDAL> logger)
        {
            _connectionStrings = options.Value;
        }

        public async Task<List<Metric>> ListDashboardMetrics(int userId)
        {
            DataTable dataTable = new DataTable();
            List<Metric> metrics = new List<Metric>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetDashboardMetrics", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    await sqlConnection.OpenAsync();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        metrics = (from DataRow dr in dataTable.Rows
                                   select new Metric()
                                   {
                                       PracticeId = Convert.ToInt32(dr["PracticeId"]),
                                       DashboardLabel = dr["DashboardLabel"].ToString(),
                                       MeasureDesc = dr["MeasureDesc"].ToString(),
                                       MeasureType = dr["MeasureType"].ToString(),
                                       PracticeTotal = Convert.ToInt32(dr["PracticeTotal"]),
                                       NetworkTotal = Convert.ToInt32(dr["NetworkTotal"])
                                   }
                            ).ToList();
                    }
                    return metrics;
                }
            }
        }

        public async Task<List<EDChart>> ListEDChart(int userId)
        {
            DataTable dataTable = new DataTable();
            List<EDChart> edCharts = new List<EDChart>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetDashboardEDChart", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                    await sqlConnection.OpenAsync();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        edCharts = (from DataRow dr in dataTable.Rows
                                   select new EDChart()
                                   {
                                       PracticeId = Convert.ToInt32(dr["PracticeId"]),
                                       AdmitDate = Convert.ToDateTime(dr["AdmitDate"]),
                                       ChartLabel = dr["ChartLabel"].ToString(),
                                       ChartTitle = dr["ChartTitle"].ToString(),
                                       EDVisits = Convert.ToInt32(dr["EDVisits"])
                                   }
                            ).ToList();

                    }
                    return edCharts;
                }
            }
        }

        public async Task<List<EDDetail>> ListEDDetails(int userId, DateTime admitDate)
        {            
            DataTable dataTable = new DataTable();
            List<EDDetail> details = new List<EDDetail>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetDashboardEDDet", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@AdmitDate", SqlDbType.Date).Value = admitDate;

                    await sqlConnection.OpenAsync();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        details = (from DataRow dr in dataTable.Rows
                                   select new EDDetail()
                                   {
                                       PAT_MRN_ID = dr["PAT_MRN_ID"].ToString(),
                                       PAT_ENC_CSN_ID = dr["PAT_ENC_CSN_ID"].ToString(),
                                       Facility = dr["Facility"].ToString(),
                                       PatientName = dr["PatientName"].ToString(),
                                       PatientDOB = (dr["PatientDOB"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["PatientDOB"].ToString())),
                                       PCP = dr["PCP"].ToString(),
                                       HOSP_ADMSN_TIME = (dr["HOSP_ADMSN_TIME"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["HOSP_ADMSN_TIME"].ToString())),
                                       HOSP_DISCH_TIME = (dr["HOSP_DISCH_TIME"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["HOSP_DISCH_TIME"].ToString())),
                                       VisitType = dr["VisitType"].ToString(),
                                       PrimaryDX = dr["PrimaryDX"].ToString(),
                                       PrimaryDX_10Code = dr["PrimaryDX_10Code"].ToString(),
                                       DX2 = dr["DX2"].ToString(),
                                       DX2_10Code = dr["DX2_10Code"].ToString()
                                   }
                            ).ToList();
                    }
                    return details;
                }
            }

        }

    }
}
