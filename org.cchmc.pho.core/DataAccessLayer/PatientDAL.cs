﻿using Microsoft.Extensions.Logging;
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
using org.cchmc.pho.core.Models;

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

        public async Task<SearchResults<Patient>> ListActivePatient(int userId, int? staffID, int? popmeasureID, bool? watch, bool? chronic, string conditionIDs, string namesearch, string sortcolumn, string sortdirection, int? pagenumber, int? rowsperpage)
        {
            DataTable patientListDataTable;
            DataTable patientListCountDataTable;
            DataSet dataSet = new DataSet();
            SearchResults<Patient> searchResults = new SearchResults<Patient>();
            searchResults.Results = new List<Patient>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPracticePatients", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;          
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;                                      
                    sqlCommand.Parameters.Add("@PCP_StaffID", SqlDbType.Int).Value = staffID; 
                    sqlCommand.Parameters.Add("@PopMeasureID", SqlDbType.Int).Value = popmeasureID; 
                    sqlCommand.Parameters.Add("@Watch", SqlDbType.Bit).Value = watch; 
                    sqlCommand.Parameters.Add("@Chronic", SqlDbType.Bit).Value = chronic; 
                    sqlCommand.Parameters.Add("@ConditionIDs", SqlDbType.VarChar, 50).Value = conditionIDs; 
                    sqlCommand.Parameters.Add("@NameSearch", SqlDbType.VarChar, 100).Value = namesearch;
                    sqlCommand.Parameters.Add("@SortColumn", SqlDbType.VarChar, 50).Value = sortcolumn;
                    sqlCommand.Parameters.Add("@SortDirection", SqlDbType.VarChar, 100).Value = sortdirection;
                    sqlCommand.Parameters.Add("@PageNumber", SqlDbType.VarChar, 50).Value = pagenumber;
                    sqlCommand.Parameters.Add("@RowspPage", SqlDbType.VarChar, 100).Value = rowsperpage;

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataSet,"PatientsDataset");
                        patientListDataTable = dataSet.Tables[0];
                        patientListCountDataTable = dataSet.Tables[1];                        
                        
                        List<PatientCondition> patientConditions = await GetPatientConditionsAll();                     
                        
                        foreach(DataRow dr in patientListDataTable.Rows)
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
                                WatchFlag = bool.Parse(dr["WatchFlag"].ToString()),
                                Conditions = new List<PatientCondition>(),
                                ActiveStatus = bool.Parse(dr["ActiveStatus"].ToString()),
                                PotentiallyActiveStatus = bool.Parse(dr["PotentiallyActive"].ToString()),
                                TotalRecords = Convert.ToInt32(dr["TotalRecords"])
                            };

                            if (!string.IsNullOrWhiteSpace(dr["ConditionIDs"].ToString()))
                            {
                                foreach (int conditionId in dr["ConditionIDs"].ToString().Split(',').Select(p => int.Parse(p)))
                                {
                                    if (patientConditions.Any(p => p.ID == conditionId))
                                        patient.Conditions.Add(patientConditions.First(p => p.ID == conditionId));
                                    else
                                        _logger.LogError("An unmapped patient condition id was returned by the database ");
                                }
                            }
                            searchResults.Results.Add(patient);
                        }  
                        foreach(DataRow dr in patientListCountDataTable.Rows)
                        {
                            searchResults.ResultCount = Convert.ToInt32(dr["TotalRecords"].ToString());
                        }
                    }
                }

                return searchResults;
            }   
        }

        public async Task<List<SimplifiedPatient>> SearchSimplifiedPatients(int userId, string search)
        {
            DataTable dataTable = new DataTable();
            List<SimplifiedPatient> searchResults = new List<SimplifiedPatient>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPatientList", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@NameSearch", SqlDbType.VarChar).Value = search;

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);

                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var workbookspt = new SimplifiedPatient()
                            {
                                PatientId = int.Parse(dr["Id"].ToString()),
                                LastName = dr["LastName"].ToString(),
                                FirstName = dr["FirstName"].ToString(),
                                DOB = (dr["PatientDOB"] == DBNull.Value ? (DateTime?)null : (DateTime.Parse(dr["PatientDOB"].ToString()))),
                                Phone = dr["Phone1"].ToString()
                            };

                            searchResults.Add(workbookspt);
                        }
                    }
                }
            }
            return searchResults;
        }

        public async Task<PatientDetails> UpdatePatientDetails(int userId, PatientDetails patientDetail)
        {

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spUpdatePatient", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@Id", SqlDbType.Int).Value = patientDetail.Id;
                    sqlCommand.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = patientDetail.FirstName;
                    sqlCommand.Parameters.Add("@LastName", SqlDbType.VarChar).Value = patientDetail.LastName;
                    sqlCommand.Parameters.Add("@PatientDOB", SqlDbType.Date).Value = patientDetail.PatientDOB;
                    sqlCommand.Parameters.Add("@WatchFlag", SqlDbType.Bit).Value = patientDetail.IsWatchList;
                    sqlCommand.Parameters.Add("@ActiveStatus", SqlDbType.Bit).Value = patientDetail.ActiveStatus;
                    sqlCommand.Parameters.Add("@GenderID", SqlDbType.Int).Value = patientDetail.GenderId;
                    sqlCommand.Parameters.Add("@PCPId", SqlDbType.Int).Value = patientDetail.PCPId;
                    sqlCommand.Parameters.Add("@InsuranceId", SqlDbType.Int).Value = patientDetail.InsuranceId;
                    sqlCommand.Parameters.Add("@ConditionIDs", SqlDbType.VarChar).Value = string.Join(",", (from c in patientDetail.Conditions select c.ID).ToArray());
                    sqlCommand.Parameters.Add("@ProvPMCAScore", SqlDbType.Int).Value = patientDetail.ProviderPMCAScore;
                    sqlCommand.Parameters.Add("@ProviderNotes", SqlDbType.VarChar).Value = patientDetail.ProviderNotes;
                    sqlCommand.Parameters.Add("@Phone1", SqlDbType.VarChar).Value = patientDetail.Phone1;
                    sqlCommand.Parameters.Add("@Email", SqlDbType.VarChar).Value = patientDetail.Email;
                    sqlCommand.Parameters.Add("@AddressLine1", SqlDbType.VarChar).Value = patientDetail.AddressLine1;
                    sqlCommand.Parameters.Add("@City", SqlDbType.VarChar).Value = patientDetail.City;
                    sqlCommand.Parameters.Add("@State", SqlDbType.VarChar).Value = patientDetail.State;
                    sqlCommand.Parameters.Add("@StateID", SqlDbType.Int).Value = patientDetail.StateId;
                    sqlCommand.Parameters.Add("@Zip", SqlDbType.VarChar).Value = patientDetail.Zip;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    sqlCommand.ExecuteNonQuery();

                    return await GetPatientDetails(userId, patientDetail.Id, true); //only active patient can be updated

                }
            }
        }
        public async Task<int> AddPatient(int userId, Patient patient)
        {

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spAddPatient", sqlConnection))
                {

                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = patient.FirstName;
                    sqlCommand.Parameters.Add("@LastName", SqlDbType.VarChar).Value = patient.LastName;
                    sqlCommand.Parameters.Add("@DOB", SqlDbType.Date).Value = patient.DOB;
                    sqlCommand.Parameters.Add("@GenderId", SqlDbType.Int).Value = patient.GenderId;
                    sqlCommand.Parameters.Add("@PCPId", SqlDbType.Int).Value = patient.PCP_StaffID;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    return ((int)sqlCommand.ExecuteScalar());
                }
            }
        }

        public async Task<int> AcceptPotentialPatient(int userId, int potentialPatientId, int PotentialProcessType)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spAcceptPotentialPatient", sqlConnection))
                {

                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@PotentialPatientID", SqlDbType.Int).Value = potentialPatientId;
                    sqlCommand.Parameters.Add("@PotentialProcessStatus", SqlDbType.Int).Value = PotentialProcessType;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    return ((int)sqlCommand.ExecuteScalar());
                }
            }
        }
        public async Task<bool> IsExistingPatient(int userId, Patient patient)
        {

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spCheckExistingPatient", sqlConnection))
                {

                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = patient.FirstName;
                    sqlCommand.Parameters.Add("@LastName", SqlDbType.VarChar).Value = patient.LastName;
                    sqlCommand.Parameters.Add("@DOB", SqlDbType.Date).Value = patient.DOB;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    return ((bool)sqlCommand.ExecuteScalar());
                }
            }
        }
        public async Task<bool> UpdatePatientWatchlist(int userId, int patientId)
        {

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spUpdatePatientWatch", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@PatientID", SqlDbType.Int).Value = patientId;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure                    
                    return ((bool)sqlCommand.ExecuteScalar());

                }
            }
        }

        public async Task<List<PatientCondition>> GetPatientConditionsAll()
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetConditionList", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    List<PatientCondition> returnObject = null;

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {

                        DataSet ds = new DataSet();
                        da.Fill(ds);

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (returnObject == null)
                            {
                                returnObject = new List<PatientCondition>();
                            }
                            int.TryParse(ds.Tables[0].Rows[i]["ID"].ToString(), out int intId);

                            PatientCondition PtCondition = new PatientCondition
                            {
                                ID = intId,
                                Name = ds.Tables[0].Rows[i]["Condition"].ToString(),
                                Description = ds.Tables[0].Rows[i]["Desc"].ToString()
                            };

                            returnObject.Add(PtCondition);
                        }

                        return returnObject;
                    }

                } 
            }
        }

        public async Task<List<PatientInsurance>> GetPatientInsuranceAll(int userId)
        {
            DataTable dataTable = new DataTable();
            List<PatientInsurance> insurances;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetInsuranceList", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        insurances = (from DataRow dr in dataTable.Rows
                                 select new PatientInsurance()
                                 {
                                     Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                     Name = dr["InsName"].ToString()
                                 }
                        ).ToList();
                    }
                }
            }
            return insurances;
        }

        public async Task<PatientDetails> GetPatientDetails(int userId, int patientId, bool potentialPatient)
        {
            DataTable dataTable = new DataTable();
            PatientDetails details = null;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPatientSummary", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = patientId;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@PotentiallyActive", SqlDbType.Int).Value = potentialPatient;
                    await sqlConnection.OpenAsync();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);

                        List<PatientCondition> patientConditions = await GetPatientConditionsAll();

                        foreach (DataRow dr in dataTable.Rows)
                        {
                            details = new PatientDetails()
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
                                InsuranceId = (dr["InsId"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["InsId"].ToString())),
                                InsuranceName = dr["InsName"].ToString(),
                                AddressLine1 = dr["AddressLine1"].ToString(),
                                AddressLine2 = dr["AddressLine2"].ToString(),
                                City = dr["City"].ToString(),
                                StateId = (dr["StateID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StateID"].ToString())),
                                State = dr["State"].ToString(),
                                Zip = dr["Zip"].ToString(),
                                Conditions = new List<PatientCondition>(),
                                PMCAScore = (dr["PMCAScore"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PMCAScore"].ToString())),
                                ProviderPMCAScore = (dr["ProviderPMCAScore"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ProviderPMCAScore"].ToString())),
                                ProviderNotes = dr["ProviderNotes"].ToString(),
                                ActiveStatus = bool.Parse(dr["ActiveStatus"].ToString()),
                                PotentiallyActiveStatus = bool.Parse(dr["PotentiallyActive"].ToString()),
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
                            };

                            if (!string.IsNullOrWhiteSpace(dr["ConditionIDs"].ToString()))
                            {
                                foreach (int conditionId in dr["ConditionIDs"].ToString().Split(',').Select(p => int.Parse(p)))
                                {
                                    if (patientConditions.Any(p => p.ID == conditionId))
                                        details.Conditions.Add(patientConditions.First(p => p.ID == conditionId));
                                    else
                                        _logger.LogError("An unmapped patient condition id was returned by the database ");
                                }
                            }
                        }



                    }
                    return details;
                }
            }
        }


        public async Task<List<Gender>> ListGender()
        {
            DataTable dataTable = new DataTable();
            List<Gender> genders;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetGenderList", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        genders = (from DataRow dr in dataTable.Rows
                                      select new Gender()
                                      {
                                          Id = (dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"].ToString())),
                                          ShortName = dr["Gender"].ToString(),
                                          Name = dr["GenderDesc"].ToString()
                                      }
                        ).ToList();
                    }
                }
            }
            return genders;
        }
        public async Task<List<PMCA>> ListPMCA()
        {
            DataTable dataTable = new DataTable();
            List<PMCA> pmcas;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPMCAList", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        pmcas = (from DataRow dr in dataTable.Rows
                                      select new PMCA()
                                      {
                                          Id = (dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"].ToString())),
                                          Score = dr["PMCAScore"].ToString(),
                                          Description = dr["Description"].ToString()
                                      }
                        ).ToList();
                    }
                }
            }
            return pmcas;
        }
        public async Task<List<State>> ListState()
        {
            DataTable dataTable = new DataTable();
            List<State> states;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetStateList", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        states = (from DataRow dr in dataTable.Rows
                                      select new State()
                                      {
                                          Id = (dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"].ToString())),
                                          Name = dr["Name"].ToString(),
                                          ShortName = dr["ShortName"].ToString()
                                      }
                        ).ToList();
                    }
                }
            }
            return states;
        }

        public bool IsPatientInSamePractice(int userId, int patientId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spCheckPermissions", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@PatientID", SqlDbType.Int).Value = patientId;
                    sqlConnection.Open();

                    return (bool)sqlCommand.ExecuteScalar();
                }
            }
        }

        public async Task<PatientDetails> GetPotentialPatientDetails(int userId, int patientId, bool potentialStatus)
        {
            DataTable dataTable = new DataTable();
            PatientDetails details = null;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPotentialPatient", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = patientId;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@PotentiallyActive", SqlDbType.Int).Value = potentialStatus;
                    await sqlConnection.OpenAsync();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);

                        List<PatientCondition> patientConditions = await GetPatientConditionsAll();

                        foreach (DataRow dr in dataTable.Rows)
                        {
                            details = new PatientDetails()
                            {
                                Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                PatientMRNId = dr["PAT_MRN_ID"].ToString(),                               
                                PracticeId = (dr["PracticeID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PracticeID"].ToString())),
                                FirstName = dr["FirstName"].ToString(),                             
                                LastName = dr["LastName"].ToString(),
                                PatientDOB = (dr["PatientDOB"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["PatientDOB"].ToString())),
                                PCPId = (dr["PCP_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PCP_ID"].ToString())),
                                PCPFirstName = dr["PCP_FirstName"].ToString(),
                                PCPLastName = dr["PCP_LastName"].ToString(),                               
                                ActiveStatus = bool.Parse(dr["ActiveStatus"].ToString()),
                                PotentiallyActiveStatus = bool.Parse(dr["PotentiallyActive"].ToString()),
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
                            };

                            if (!string.IsNullOrWhiteSpace(dr["ConditionIDs"].ToString()))
                            {
                                foreach (int conditionId in dr["ConditionIDs"].ToString().Split(',').Select(p => int.Parse(p)))
                                {
                                    if (patientConditions.Any(p => p.ID == conditionId))
                                        details.Conditions.Add(patientConditions.First(p => p.ID == conditionId));
                                    else
                                        _logger.LogError("An unmapped patient condition id was returned by the database ");
                                }
                            }
                        }



                    }
                    return details;
                }
            }
        }

    }
}
    
