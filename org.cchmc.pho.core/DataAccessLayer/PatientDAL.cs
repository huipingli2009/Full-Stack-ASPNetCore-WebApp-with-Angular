using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;

namespace org.cchmc.pho.core.DataAccessLayer
{
    public class PatientDAL : IPatient
    {
        private readonly ConnectionStrings _connectionStrings;      
        private readonly ILogger<PatientDAL> _logger;

        public PatientDAL(IOptions<ConnectionStrings> options, ILogger<PatientDAL> logger)
        {
            _connectionStrings = options.Value;
            _logger = logger;
        }       

        public async Task<List<Patient>> ListActivePatient(int userId, int staffID, int popmeasureID, bool watch, bool chronic, string conditionIDs, string namesearch)
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

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);

                        List<PatientStatus> patientStatus = new List<PatientStatus>();

                        patientStatus = this.GetPatientStatusAll();

                        List<PatientCondition> patientConditions = new List<PatientCondition>();

                        patientConditions = this.GetPatientConditionsAll();

                        foreach(DataRow dr in dataTable.Rows)
                        {                 

                            var patient = new Patient()
                            {                                 
                                PatientId = Convert.ToInt32(dr["PatientId"]),
                                FirstName = dr["FirstName"].ToString(),
                                LastName = dr["LastName"].ToString(),
                                PCP_StaffID = Convert.ToInt32(dr["PCP_StaffID"]),
                                PracticeID = Convert.ToInt32(dr["PracticeID"]),
                                DOB = (dr["DOB"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["DOB"].ToString())),                              
                                LastEDVisit = (dr["LastEDVisit"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["LastEDVisit"].ToString())),
                                Chronic = bool.Parse(dr["Chronic"].ToString()),
                                Conditions = new List<PatientCondition>(),
                                Status = new List<PatientStatus>()                               
                            };                           

                            foreach (int statusId in dr["ActiveStatus"].ToString().Split(',').Select(p => int.Parse(p)))
                            {
                                if (patientStatus.Any(p => p.ID == statusId))
                                    patient.Status.Add(patientStatus.First(p => p.ID == statusId));
                                else
                                {
                                    _logger.LogError("An unmapped condition id was returned by the database ");
                                }
                            }

                            foreach (int conditionId in dr["ConditionIDs"].ToString().Split(',').Select(p => int.Parse(p)))
                            {
                                if(patientConditions.Any(p => p.ID == conditionId))
                                    patient.Conditions.Add(patientConditions.First(p => p.ID == conditionId));
                                else
                                {                                    
                                    _logger.LogError("An unmapped condition id was returned by the database ");
                                }
                            }
                            patients.Add(patient);
                        }  
                    }
                }

                return patients;
            }   
        }

        public List<PatientCondition> GetPatientConditionsAll()
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB);          

            SqlCommand sqlCommand = new SqlCommand("spGetAllConditions", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            List<PatientCondition> returnObject = null;

            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (returnObject == null)
                {
                    returnObject = new List<PatientCondition>();
                }
                PatientCondition PtCondition = CreatePatientConditionModel(ds.Tables[0].Rows[i]);
                returnObject.Add(PtCondition);
            }

            return returnObject;
        }
        public List<PatientCondition> GetPatientConditionList()
        {
            List<PatientCondition> returnObject = null;

            SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB);
            SqlCommand sqlCommand = new SqlCommand("spGetAllConditions", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (returnObject == null)
                {
                    returnObject = new List<PatientCondition>();
                }
                PatientCondition PtCondition = CreatePatientConditionModel(ds.Tables[0].Rows[i]);
                returnObject.Add(PtCondition);
            }

            return returnObject;
        }
        public PatientCondition CreatePatientConditionModel(DataRow dr)
        {
            PatientCondition c = new PatientCondition();
            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                c.ID = int.Parse(dr["Id"].ToString());
            }
            c.Name = dr["Condition"].ToString();

            return c;
        }

        public List<PatientStatus> GetPatientStatusAll()
        {
            List<PatientStatus> returnObject = null;
            SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB);

            SqlCommand sqlCommand = new SqlCommand("spGetPatientStatusAll", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;           

            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            da.Fill(ds);            

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (returnObject == null)
                {
                    returnObject = new List<PatientStatus>();
                }
                PatientStatus PtStatus = CreatePatientStatusModel(ds.Tables[0].Rows[i]);
                returnObject.Add(PtStatus);
            }

            return returnObject;
        }

        public PatientStatus CreatePatientStatusModel(DataRow dr)
        {
            PatientStatus c = new PatientStatus();
            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                c.ID = int.Parse(dr["Id"].ToString());
            }
            c.Name = dr["Name"].ToString();

            return c;
        }

        public async Task<PatientDetails> GetPatientDetails(int patientId)
        {
            DataTable dataTable = new DataTable();
            PatientDetails details;
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
                                       Conditions = ParseConditionStrings(dr["ConditionIDs"].ToString(), dr["Condition"].ToString()),
                                       PMCAScore = (dr["PMCAScore"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PMCAScore"].ToString())),
                                       ProviderPMCAScore = (dr["ProviderPMCAScore"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ProviderPMCAScore"].ToString())),
                                       ProviderNotes = dr["ProviderNotes"].ToString(),
                                       Status = new PatientStatus(dr["ActiveStatus"].ToString(), dr["ActiveStatusName"].ToString()),
                                       GenderId = (dr["GenderID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GenderID"].ToString())),
                                       Gender = dr["Gender"].ToString(),
                                       Email = dr["Email"].ToString(),
                                       Phone1 = dr["Phone1"].ToString(),
                                       Phone2 = dr["Phone2"].ToString(),
                                       PracticeVisits = (dr["PracticeVisits"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PracticeVisits"].ToString())),
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

        private List<PatientCondition> ParseConditionStrings(string ConditionIDs, string ConditionNames)
        {
            List<PatientCondition> conditions = new List<PatientCondition>();

            int[] ids = ConditionIDs.Split(',').Select(int.Parse).ToArray();
            string[] names = ConditionNames.Split(',').ToArray();

            if (ids.Length != names.Length)
            {
                throw new Exception("Condition data strings have an unequal count of delimiters");
            }

            for (int i = 0; i < ids.Length; i++)
            {
                conditions.Add(new PatientCondition(ids[i], names[i]));
            }

            return conditions;
        }
    }
}
    
