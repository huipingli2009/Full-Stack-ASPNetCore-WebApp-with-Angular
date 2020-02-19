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
    // TODO: all ADO and DI setup
    public class AlertDAL : IAlert
    {
        //private IConfiguration _config;
        private readonly ConnectionStrings _connectionStrings;
        public AlertDAL(IOptions<ConnectionStrings> options, ILogger<AlertDAL> logger)
        {
            _connectionStrings = options.Value;
        }

      
        public async Task<List<Alert>> ListActiveAlerts(int userId)
        {
            DataTable dataTable = new DataTable();
            List<Alert> alerts = new List<Alert>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB) )
            {                
                using (SqlCommand sqlCommand = new SqlCommand("spGetActiveAlerts", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    
                    await sqlConnection.OpenAsync();                      
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        await Task.Run(() => da.Fill(dataTable));
                        alerts = (from DataRow dr in dataTable.Rows
                                    select new Alert()
                                    {
                                        AlertId = Convert.ToInt32(dr["AlertId"]),
                                        AlertScheduleId = Convert.ToInt32(dr["Alert_ScheduleId"]),
                                        Message = dr["AlertMessage"].ToString(),
                                        Url = dr["URL"].ToString(),
                                        LinkText = dr["URL_Label"].ToString(),
                                        Definition = dr["AlertDefinition"].ToString()
                                    }

                            ).ToList();
                            
                    }
                   
                    return alerts;
                }
            }
                        
        }

        public async Task MarkAlertAction(int alertScheduleId, int userId, int alertActionId)
        {

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spPostAlertActivity", sqlConnection))
                {                    
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@AlertScheduleId", SqlDbType.Int).Value = alertScheduleId;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@ActionId", SqlDbType.Int).Value = alertActionId;
                   
                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    await sqlCommand.ExecuteNonQueryAsync();                   
                                      
                }
            }
        }
    }
}
