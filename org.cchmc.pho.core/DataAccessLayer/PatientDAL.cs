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
    }
}
    
