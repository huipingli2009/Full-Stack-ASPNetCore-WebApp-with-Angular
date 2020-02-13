using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace org.cchmc.pho.core.DataAccessLayer
{
    // TODO: all ADO and DI setup
    public class AlertDAL : IAlert
    {
        private IConfiguration _config;
        private string _connectionString;
        public AlertDAL(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("pho-db");
        }

      
        public async Task<List<Alert>> ListActiveAlerts(int userId)
        {
            DataTable dataTable = new DataTable();
            List<Alert> alerts = new List<Alert>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString) )
            {                
                using (SqlCommand sqlCommand = new SqlCommand("spGetActiveAlerts", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    try
                    {
                        sqlConnection.Open();
                        // Define the data adapter and fill the dataset
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                        {
                            da.Fill(dataTable);
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
                    }

                   
                    catch (Exception exception)
                    {

                    }
                    finally
                    {
                        sqlCommand.Dispose();

                        if (sqlConnection != null)
                        {
                            sqlConnection.Close();
                        }
                    }
                    return alerts;
                }
            }

            
                // this is where the code goes to return the alerts by user
                throw new NotImplementedException();
        }

        public async Task MarkAlertAction(int alertScheduleId, int userId, int alertActionId)
        {

            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spPostAlertActivity", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@AlertScheduleId", SqlDbType.Int).Value = alertScheduleId;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@ActionId", SqlDbType.Int).Value = alertActionId;
                    try
                    {
                        sqlConnection.Open();

                        //Execute Stored Procedure
                        sqlCommand.ExecuteNonQuery();                        
                    }

                    catch (Exception exception)
                    {

                    }
                    finally
                    {
                        sqlCommand.Dispose();

                        if (sqlConnection != null)
                        {
                            sqlConnection.Close();
                        }
                    }
                }
            }
        }
    }
}
