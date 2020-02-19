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

    }
}
