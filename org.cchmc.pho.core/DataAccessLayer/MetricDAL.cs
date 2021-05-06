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
using System.Runtime.InteropServices;

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
                                       ConditionId = (dr["ConditionId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ConditionId"].ToString())),
                                       MeasureDesc = dr["MeasureDesc"].ToString(),
                                       MeasureType = dr["MeasureType"].ToString(),
                                       PracticeTotal = Convert.ToInt32(dr["PracticeTotal"]),
                                       NetworkTotal = Convert.ToInt32(dr["NetworkTotal"]),
                                       OpDefURL = dr["OpDefURL"].ToString()
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

        public async Task<WebChart> ListWebChart(int userId, int? chartId, int? measureId, int? filterId)
        {
            DataSet dataSet = new DataSet();
            WebChart chart = new WebChart();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetDashboardChart", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@ChartID", SqlDbType.Int).Value = (chartId.HasValue ? chartId : (int?)null); 
                    sqlCommand.Parameters.Add("@MeasureID", SqlDbType.Int).Value = (measureId.HasValue ? measureId : (int?)null);
                    sqlCommand.Parameters.Add("@FilterID", SqlDbType.Int).Value = (filterId.HasValue ? filterId : (int?)null); 

                    await sqlConnection.OpenAsync();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataSet);
                        var headerTable = dataSet.Tables[0];
                        DataTable finalTable = dataSet.Tables[1];

                        DataRow hr = headerTable.Rows[0];
                        chart = (new WebChart(
                            Convert.ToInt32(hr["PracticeID"]), 
                            hr["ChartTitle"].ToString(), 
                            hr["HeaderText"].ToString()
                            ));

                        if (hr["VerticalMax"] != DBNull.Value)
                        {
                            chart.VerticalMax = Convert.ToDecimal(hr["VerticalMax"].ToString());
                        }

                        //Split the final table into a list, using the DataSetIndex as the groupby.
                        List<DataTable> dataSets = finalTable.AsEnumerable()
                        .GroupBy(row => row.Field<double>("DataSetIndex"))
                        .Select(g => g.CopyToDataTable())
                        .ToList();

                        foreach(DataTable ds in dataSets)
                        { 
                            //create new dataset object
                            WebChartDataSet curDataSet = new WebChartDataSet()
                            {
                                Type = ds.Rows[0]["ChartType"].ToString(),
                                Legend = ds.Rows[0]["Legend"].ToString(),
                                BackgroundColor = ds.Rows[0]["BackgroundColor"].ToString(),
                                BackgroundHoverColor = ds.Rows[0]["BackgroundHoverColor"].ToString(),
                                BorderColor = ds.Rows[0]["BorderColor"].ToString(),
                                Fill = Convert.ToBoolean(ds.Rows[0]["Fill"].ToString()),
                                ShowLine = (ds.Rows[0]["ShowLine"] == DBNull.Value ? false : Convert.ToBoolean(ds.Rows[0]["ShowLine"].ToString())),
                                BorderDash = (ds.Rows[0]["BorderDash"] == DBNull.Value ? new int[0] : Array.ConvertAll(ds.Rows[0]["BorderDash"].ToString().Split(','), int.Parse)),
                                PointStyle = (ds.Rows[0]["PointStyle"] == DBNull.Value ? "" : ds.Rows[0]["PointStyle"].ToString()),
                                PointRadius = (ds.Rows[0]["PointRadius"] == DBNull.Value ? 3 : Convert.ToInt32(ds.Rows[0]["PointRadius"].ToString())),
                            };

                            DataView dv = ds.DefaultView;
                            dv.Sort = "SortOrder asc";
                            DataTable sortedDT = dv.ToTable();

                            //Extract label and value arrays
                            List<string> xAxisLabels = new List<string>();
                            List<decimal> chartValues = new List<decimal>();
                            foreach(DataRow row in sortedDT.Rows)
                            {
                                xAxisLabels.Add(row["DataPointLabel"].ToString());
                                chartValues.Add(Convert.ToDecimal(row["ChartValue"].ToString()));
                            }
                            curDataSet.XAxisLabels = xAxisLabels.ToArray();
                            curDataSet.Values = chartValues.ToArray();

                            //Add the set to the parent object
                            chart.DataSets.Add(curDataSet);
                        }

                    }
                }
            }

            return chart;
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

        //GetWebChartFilters
        public async Task<List<WebChartFilters>> GetWebChartFilters(int chartId, int measureId)
        {
            DataTable dataTable = new DataTable();
            List<WebChartFilters> webchartfilters = new List<WebChartFilters>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetChartFilters", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@ChartID", SqlDbType.Int).Value = chartId;
                    sqlCommand.Parameters.Add("@MeasureID", SqlDbType.Int).Value = measureId;

                    await sqlConnection.OpenAsync();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var record = new WebChartFilters()
                            {
                                FilterId = (dr["FilterID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["FilterID"].ToString())),
                                FilterLabel = dr["FilterLabel"].ToString()
                            };
                            webchartfilters.Add(record);
                        }
                    }
                    return webchartfilters;
                }
            }
        }

    }
}
