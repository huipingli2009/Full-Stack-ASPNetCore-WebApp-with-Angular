using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace org.cchmc.pho.core.DataAccessLayer
{
    public class PatientDAL : IPatient
    {
        private readonly ConnectionStrings _connectionStrings;

        public PatientDAL(IOptions<ConnectionStrings> options, ILogger<PatientDAL> logger)
        {
            _connectionStrings = options.Value;
        }

        public async Task<List<Patient>> ListActivePatient(int userId, int staffID, int popmeasureID, int watch, int chronic, string conditionIDs, string namesearch, string sortcolumn, int pagenumber, int rowspage)
        {
            DataTable dataTable = new DataTable();
            List<Patient> patients = new List<Patient>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPracticePatients", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                   

                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;                  

                    if (staffID != 0)
                    {
                        sqlCommand.Parameters.Add("@PCP_StaffID", SqlDbType.Int).Value = staffID; 
                    }
                    else
                    {
                        sqlCommand.Parameters.Add("@PCP_StaffID", SqlDbType.Int).Value = DBNull.Value;
                    }   
                  

                    if (popmeasureID != 0)
                    {
                        sqlCommand.Parameters.Add("@PopMeasureID", SqlDbType.Int).Value = popmeasureID; 
                    }
                    else
                    {
                        sqlCommand.Parameters.Add("@PopMeasureID", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (watch != 0)
                    {
                        sqlCommand.Parameters.Add("@Watch", SqlDbType.Int).Value = watch; 
                    }
                    else
                    {
                        sqlCommand.Parameters.Add("@Watch", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (chronic != 0)
                    {
                        sqlCommand.Parameters.Add("@Chronic", SqlDbType.Int).Value = chronic; 
                    }
                    else
                    {
                        sqlCommand.Parameters.Add("@Chronic", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (conditionIDs != null)
                    {
                        sqlCommand.Parameters.Add("@ConditionIDs", SqlDbType.VarChar, 50).Value = conditionIDs; 
                    }
                    else
                    {
                        sqlCommand.Parameters.Add("@ConditionIDs", SqlDbType.VarChar, 50).Value = DBNull.Value;
                    }

                    if (namesearch != null)
                    {
                        sqlCommand.Parameters.Add("@NameSearch", SqlDbType.VarChar, 100).Value = namesearch; 
                    }
                    else
                    {
                        sqlCommand.Parameters.Add("@NameSearch", SqlDbType.VarChar, 100).Value = DBNull.Value;
                    }

                    if (sortcolumn != null)
                    {
                        sqlCommand.Parameters.Add("@SortColumn", SqlDbType.VarChar, 50).Value = sortcolumn;
                    }
                    else
                    {
                        sqlCommand.Parameters.Add("@SortColumn", SqlDbType.VarChar, 50).Value = DBNull.Value;
                    }

                    if (pagenumber != 0)
                    {
                        sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pagenumber;
                    }
                    else
                    {
                        sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (rowspage != 0)
                    {
                        sqlCommand.Parameters.Add("@RowspPage", SqlDbType.Int).Value = rowspage;
                    }
                    else
                    {
                        sqlCommand.Parameters.Add("@RowspPage", SqlDbType.Int).Value = DBNull.Value;
                    }

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);

                        patients = (from DataRow dr in dataTable.Rows
                                    select new Patient()
                                    {
                                        //SortCol = Convert.ToInt32(dr["SortCol"]),
                                        PatientId = Convert.ToInt32(dr["PatientId"]),
                                        FirstName = dr["FirstName"].ToString(),
                                        LastName = dr["LastName"].ToString(),
                                        PCP_StaffID = Convert.ToInt32(dr["PCP_StaffID"]),
                                        PracticeID = Convert.ToInt32(dr["PracticeID"]),
                                        DOB = (dr["DOB"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["DOB"].ToString())),
                                        ActiveStatus = int.Parse(dr["ActiveStatus"].ToString())
                                    }

                          ).ToList();
                    }

                }

                return patients;
            }

        }
    }
}
    
