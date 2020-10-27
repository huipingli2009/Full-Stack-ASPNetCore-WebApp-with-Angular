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
        private readonly ConnectionStrings _connectionStrings;
        public MetricDAL(IOptions<ConnectionStrings> options, ILogger<MetricDAL> logger)
        {
            _connectionStrings = options.Value;
        }

        public async Task<List<DashboardMetric>> ListDashboardMetrics(int userId)
        {
            DataTable dataTable = new DataTable();
            List<DashboardMetric> metrics;
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
                                   select new DashboardMetric()
                                   {
                                       PracticeId = Convert.ToInt32(dr["PracticeId"]),
                                       DashboardLabel = dr["DashboardLabel"].ToString(),
                                       MeasureId = (dr["MeasureId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MeasureId"].ToString())),
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
        public async Task<List<PopulationMetric>> ListPopulationMetrics()
        {
            DataTable dataTable = new DataTable();
            List<PopulationMetric> metrics;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPopulationMetricList", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    await sqlConnection.OpenAsync();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        metrics = (from DataRow dr in dataTable.Rows
                                   select new PopulationMetric()
                                   {
                                       Id = Convert.ToInt32(dr["MeasureId"]),
                                       Label = dr["DashboardLabel"].ToString()
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
                                       PatientId = Convert.ToInt32(dr["PatientID"]),
                                       PatientMRN = dr["PAT_MRN_ID"].ToString(),
                                       PatientEncounterID = dr["PAT_ENC_CSN_ID"].ToString(),
                                       Facility = dr["Facility"].ToString(),
                                       PatientName = dr["PatientName"].ToString(),
                                       PatientDOB = (dr["PatientDOB"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["PatientDOB"].ToString())),
                                       PCP = dr["PCP"].ToString(),
                                       HospitalAdmission = (dr["HOSP_ADMSN_TIME"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["HOSP_ADMSN_TIME"].ToString())),
                                       HospitalDischarge = (dr["HOSP_DISCH_TIME"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["HOSP_DISCH_TIME"].ToString())),
                                       VisitType = dr["VisitType"].ToString(),
                                       PrimaryDX = dr["PrimaryDX"].ToString(),
                                       PrimaryDX_10Code = dr["PrimaryDX_10Code"].ToString(),
                                       DX2 = dr["DX2"].ToString(),
                                       DX2_10Code = dr["DX2_10Code"].ToString(),
                                       InpatientVisit = dr["InpatientVisit"].ToString(),
                                       EDVisitCount = dr["EDVisitCount"].ToString(),
                                       UCVisitCount = dr["UCVisitCount"].ToString(),
                                       AdmitCount = dr["AdmitCount"].ToString()
                                   }
                            ).ToList();
                    }
                    return details;
                }
            }

        }
        public async Task<DrillthruMetricTable> GetDrillthruTable(int userId, int measureId, int filterId)
        {
            DataTable dataTable = new DataTable();
            DrillthruMetricTable drillthruTable = new DrillthruMetricTable();
            drillthruTable.Rows = new List<DrillthruRow>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetDrillthruData", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@MeasureID", SqlDbType.Int).Value = measureId;
                    if (filterId > 0)
                    {
                        sqlCommand.Parameters.Add("@FilterID", SqlDbType.Int).Value = filterId;
                    }

                    await sqlConnection.OpenAsync();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);

                        foreach (DataRow dr in dataTable.DefaultView.ToTable(true, "RowNumber").Rows)
                        {
                            DrillthruRow row = new DrillthruRow()
                            {
                                RowNumber = Convert.ToInt32(dr[0]),
                                Columns = new List<DrillthruColumn>()
                            };
                            drillthruTable.Rows.Add(row);
                        }
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            int currentRow = Convert.ToInt32(dr["RowNumber"]);

                            var column = new DrillthruColumn()
                            {
                                ColumnName =dr["ColumnName"].ToString(),
                                Value = dr["Value"].ToString()
                            };

                            drillthruTable.Rows.Find(x => x.RowNumber.Equals(currentRow)).Columns.Add(column);
                        }

                    }
                    return drillthruTable;
                }
            }

        }
        public async Task<List<PopulationOutcomeMetric>> GetPopulationOutcomeMetrics()
        {
            DataTable dataTable = new DataTable();
            List<PopulationOutcomeMetric> outcomemetrics;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetOutcomeMetricList", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    await sqlConnection.OpenAsync();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        outcomemetrics = (from DataRow dr in dataTable.Rows
                                   select new PopulationOutcomeMetric()
                                   {
                                       MeasureId = Convert.ToInt32(dr["MeasureId"]),
                                       DashboardLabel = dr["DashboardLabel"].ToString()
                                   }
                            ).ToList();
                    }
                    return outcomemetrics;
                }
            }
        }

    }
}
