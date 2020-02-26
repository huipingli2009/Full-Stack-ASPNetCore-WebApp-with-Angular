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

        public async Task<List<Patient>> ListActivePatient(int userId, int staffID, int popmeasureID, bool watch, bool chronic, string conditionIDs, string namesearch, string sortcolumn, int pagenumber, int rowspage)
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

                    if (watch == true)
                    {
                        sqlCommand.Parameters.Add("@Watch", SqlDbType.Int).Value = watch; 
                    }
                    else
                    {
                        sqlCommand.Parameters.Add("@Watch", SqlDbType.Int).Value = false;
                    }

                    if (chronic == true)
                    {
                        sqlCommand.Parameters.Add("@Chronic", SqlDbType.Int).Value = chronic; 
                    }
                    else
                    {
                        sqlCommand.Parameters.Add("@Chronic", SqlDbType.Int).Value = false;
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

                    //using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    //{
                    //    da.Fill(dataTable);

                    //    patients = (from DataRow dr in dataTable.Rows
                    //                select new Patient()
                    //                {
                    //                    //SortCol = Convert.ToInt32(dr["SortCol"]),
                    //                    PatientId = Convert.ToInt32(dr["PatientId"]),
                    //                    FirstName = dr["FirstName"].ToString(),
                    //                    LastName = dr["LastName"].ToString(),
                    //                    PCP_StaffID = Convert.ToInt32(dr["PCP_StaffID"]),
                    //                    PracticeID = Convert.ToInt32(dr["PracticeID"]),
                    //                    DOB = (dr["DOB"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["DOB"].ToString())),
                    //                    ActiveStatus = int.Parse(dr["ActiveStatus"].ToString()),
                    //                    LastEDVisit = (dr["LastEDVisit"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["LastEDVisit"].ToString())),
                    //                    Chronic = bool.Parse(dr["Chronic"].ToString()),
                    //                    //WatchFlag = Convert.ToInt32(dr["WatchFlag"].ToString()),
                    //                    //SortColumn = dr["SortColumn"].ToString(),  //no need to track from data part                                     

                    //                    ConditionIDs = dr["ConditionIDs"].ToString()
                    //                }

                    //      ).ToList();
                    //}
                }

                return patients;
            }

        }


        public async Task<PatientDetails> GetPatientDetails(int patientId)
        {
            DataTable dataTable = new DataTable();
            PatientDetails details = new PatientDetails();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPatientSummary", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = patientId;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        details = (from DataRow dr in dataTable.Rows
                                   select new PatientDetails()
                                   {
                                       Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                       PatientMRNId = dr["PAT_MRN_ID"].ToString(),
                                       PatId = dr["PAT_ID"].ToString(),
                                       PracticeId = (dr["PracticeID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PracticeID"].ToString())),
                                       FirstName = dr["FirstName"].ToString(),
                                       MiddleName = dr["MiddleName"].ToString(),
                                       LastName = dr["LastName"].ToString(),
                                       PatientDOB = (dr["PatientDOB"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["PatientDOB"].ToString())),
                                       PCPId = (dr["PCP_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PCP_ID"].ToString())),
                                       PCPFirstName = dr["PCP_FirstName"].ToString(),
                                       PCPLastName = dr["PCP_LastName"].ToString(),
                                       InsuranceId = (dr["InsId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["InsId"].ToString())),
                                       InsuranceName = dr["InsName"].ToString(),
                                       AddressLine1 = dr["AddressLine1"].ToString(),
                                       AddressLine2 = dr["AddressLine2"].ToString(),
                                       City = dr["City"].ToString(),
                                       State = dr["State"].ToString(),
                                       Zip = dr["Zip"].ToString(),
                                       ConditionIds = dr["ConditionIDs"].ToString(),
                                       Conditions = dr["Condition"].ToString(),
                                       PMCAScore = (dr["PMCAScore"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PMCAScore"].ToString())),
                                       ProviderPMCAScore = (dr["ProviderPMCAScore"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ProviderPMCAScore"].ToString())),
                                       ProviderNotes = dr["ProviderNotes"].ToString(),
                                       ActiveStatus = (dr["ActiveStatus"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ActiveStatus"].ToString())),
                                       ActiveStatusName = dr["ActiveStatusName"].ToString(),
                                       GenderId = (dr["GenderID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GenderID"].ToString())),
                                       Gender = dr["Gender"].ToString(),
                                       Email = dr["Email"].ToString(),
                                       Phone1 = dr["Phone1"].ToString(),
                                       Phone2 = dr["Phone2"].ToString(),
                                       PracticeVisits =  (dr["PracticeVisits"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PracticeVisits"].ToString())),
                                       CCHMCEncounters = (dr["CCHMCEncounters"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CCHMCEncounters"].ToString())),
                                       HealthBridgeEncounters = (dr["HealthBridgeEncounters"] == DBNull.Value ? 0 : Convert.ToInt32(dr["HealthBridgeEncounters"].ToString())),
                                       UniqueDXs = (dr["UniqueDXs"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UniqueDXs"].ToString())),
                                       UniqueCPTCodes = (dr["UniqueCPTCodes"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UniqueCPTCodes"].ToString())),
                                       LastPracticeVisit = (dr["LastPracticeVisit"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["LastPracticeVisit"].ToString())),
                                       LastCCHMCAdmit = (dr["LastCCHMCAdmit"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["LastCCHMCAdmit"].ToString())),
                                       LastHealthBridgeAdmit = (dr["LastHBAdmit"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["LastHBAdmit"].ToString())),
                                       LastDiagnosis = dr["LastDiagnosis"].ToString(),
                                       CCHMCAppointment = (dr["CCHMCAppt"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["CCHMCAppt"].ToString()))
                                   }
                            ).SingleOrDefault();
                    }
                    return details;
                }
            }

        }
    }
}
    
