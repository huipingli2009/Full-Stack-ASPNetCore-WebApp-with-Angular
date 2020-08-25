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
    public class WorkbooksDal : IWorkbooks
    {
        private readonly ConnectionStrings _connectionStrings;

        public WorkbooksDal(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }
        public async Task<List<WorkbooksDepressionPatient>> GetDepressionPatientList(int userId, int formResponseId)
        {
            DataTable dataTable = new DataTable();
            List<WorkbooksDepressionPatient> workbookspatients = new List<WorkbooksDepressionPatient>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPHQ9Workbooks_Patients", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);

                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var workbookspt = new WorkbooksDepressionPatient()
                            {
                                FormResponseId = int.Parse(dr["FormResponseId"].ToString()),
                                PatientId = int.Parse(dr["PatientId"].ToString()),
                                Patient = dr["Patient"].ToString(),
                                DOB = (dr["DOB"] == DBNull.Value ? (DateTime?)null : (DateTime.Parse(dr["DOB"].ToString()))),
                                Phone = dr["Phone"].ToString(),
                                Provider = dr["Provider"].ToString(),
                                DateOfService = (dr["DateOfService"] == DBNull.Value ? (DateTime?)null : (DateTime.Parse(dr["DateOfService"].ToString()))),
                                PHQ9_Score = dr["PHQ9_Score"].ToString(),
                                ActionFollowUp = (dr["ActionFollowUp"] != DBNull.Value && Convert.ToBoolean(dr["ActionFollowUp"])),
                                Improvement = dr["Improvement"].ToString(),
                                FollowUpResponse = bool.Parse(dr["FollowUpResponse"].ToString())
                            };

                            workbookspatients.Add(workbookspt);
                        }
                    }
                }
            }
            return workbookspatients;
        }

        public async Task<List<WorkbooksAsthmaPatient>> GetAsthmaPatientList(int userId, int formResponseId)
        {        

            DataTable dataTable = new DataTable();
            List<WorkbooksAsthmaPatient> workbooksasthmapatients = new List<WorkbooksAsthmaPatient>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetAsthmaWorkbooks_Patients", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);

                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var workbookspt = new WorkbooksAsthmaPatient()
                            {
                                FormResponseId = int.Parse(dr["FormResponseId"].ToString()),
                                PatientId = int.Parse(dr["PatientId"].ToString()),
                                Patient = dr["Patient"].ToString(),
                                DOB = (dr["DOB"] == DBNull.Value ? (DateTime?)null : (DateTime.Parse(dr["DOB"].ToString()))),
                                Phone = dr["Phone"].ToString(),
                                Provider = dr["Provider"].ToString(),
                                DateOfService = (dr["DateOfService"] == DBNull.Value ? (DateTime?)null : (DateTime.Parse(dr["DateOfService"].ToString()))),
                                Asthma_Score = dr["AsthmaScore"].ToString(),
                                ActionPlanGiven = dr["ActionPlanGiven"] != DBNull.Value && Convert.ToBoolean(dr["ActionPlanGiven"]),                               
                                Treatment = dr["Treatment"].ToString(),                               
                                AssessmentCompleted = (dr["AssessmentCompleted"] != DBNull.Value && Convert.ToBoolean(dr["AssessmentCompleted"]))
                            };

                            workbooksasthmapatients.Add(workbookspt);
                        }
                    }
                }
            }
            return workbooksasthmapatients;
        }

        public async Task<WorkbooksPractice> GetPracticeWorkbooks(int userId, int formResponseId)
        {
            DataTable dataTable = new DataTable();
            WorkbooksPractice workbookpractice;

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPHQ9Workbooks_Practice", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);

                        workbookpractice = (from DataRow dr in dataTable.Rows
                                            select new WorkbooksPractice()
                                            {
                                                FormResponseId = int.Parse(dr["FormResponseId"].ToString()),
                                                Header = dr["Header"].ToString(),
                                                Line1 = dr["Line1"].ToString(),
                                                Line2 = dr["Line2"].ToString(),
                                                Line3 = dr["Line3"].ToString(),
                                                JobAidURL = dr["JobAidURL"].ToString()
                                            }
                                        ).SingleOrDefault();
                    }
                }
            }
            return workbookpractice;
        }

        public async Task<List<WorkbooksProvider>> GetPracticeWorkbooksProviders(int userId, int formResponseId)
        {
            DataTable dataTable = new DataTable();
            List<WorkbooksProvider> workbooksproviders = new List<WorkbooksProvider>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPHQ9Workbooks_Providers", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);

                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var workbooksprovider = new WorkbooksProvider()
                            {
                                FormResponseID = int.Parse(dr["FormResponseID"].ToString()),
                                StaffID = int.Parse(dr["StaffID"].ToString()),
                                Provider = dr["Provider"].ToString(),
                                PHQS = int.Parse(dr["PHQS"].ToString()),
                                TOTAL = int.Parse(dr["Total"].ToString())
                            };
                            workbooksproviders.Add(workbooksprovider);
                        }
                    }
                }
                return workbooksproviders;
            }
        }

        public async Task<List<WorkbooksLookup>> GetWorkbooksLookups(int formId, int userId, string nameSearch)
        {
            DataTable dataTable = new DataTable();
            List<WorkbooksLookup> workbookslookups = new List<WorkbooksLookup>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetWorkbookPeriods", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add("@FormId", SqlDbType.Int).Value = formId;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@NameSearch", SqlDbType.VarChar).Value = nameSearch;

                    await sqlConnection.OpenAsync();
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var workbookslookup = new WorkbooksLookup()
                            {
                                FormId = (dr["FormID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["FormID"].ToString())),
                                QuestionId = (dr["QuestionID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["QuestionID"].ToString())),
                                PracticeId = (dr["PracticeID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PracticeID"].ToString())),
                                FormResponseId = (dr["FormResponseID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["FormResponseID"].ToString())),
                                ReportingPeriod = dr["ReportingPeriod"].ToString()
                            };

                            workbookslookups.Add(workbookslookup);
                        }
                    }
                }
            }
            return workbookslookups;
        }

        public async Task<bool> AddPatientToDepressionWorkbooks(int userId, int formResponseId, int patientID, int providerstaffID, DateTime? dos, int phq9score, bool action)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spPostPHQ9Workbook_Patient", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;
                    sqlCommand.Parameters.Add("@PatientID", SqlDbType.Int).Value = patientID;
                    sqlCommand.Parameters.Add("@ProviderStaffID", SqlDbType.Int).Value = providerstaffID;
                    sqlCommand.Parameters.Add("@PHQ9Score", SqlDbType.Int).Value = phq9score;
                    sqlCommand.Parameters.Add("@DateOfService", SqlDbType.DateTime).Value = dos;
                    sqlCommand.Parameters.Add("@Action", SqlDbType.Int).Value = action;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    return (bool)sqlCommand.ExecuteScalar();
                }
            }
        }

        public async Task<bool> RemovePatientFromWorkbooks(int userId, int formResponseId, int patientID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spRemovePHQ9Workbook_Patient", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;
                    sqlCommand.Parameters.Add("@PatientID", SqlDbType.Int).Value = patientID;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    return (bool)sqlCommand.ExecuteScalar();
                }
            }
        }

        public async Task<bool> AddProviderToWorkbooks(int userId, int formResponseId, int providerId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spAddPHQ9Provider", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;
                    sqlCommand.Parameters.Add("@ProviderStaffId", SqlDbType.Int).Value = providerId;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    return (bool)sqlCommand.ExecuteScalar();
                }
            }
        }

        public async Task<bool> RemoveProviderFromWorkbooks(int userId, int formResponseId, int providerId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spRemovePHQ9Provider", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;
                    sqlCommand.Parameters.Add("@ProviderStaffId", SqlDbType.Int).Value = providerId;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    return (bool)sqlCommand.ExecuteScalar();
                }
            }
        }

        public async Task<int> UpdateWorkbooksProviders(int userId, int formResponseId, int providerstaffID, int phqs, int total)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spUpdatePHQ9Workbooks_Providers", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;
                    sqlCommand.Parameters.Add("@ProviderStaffID", SqlDbType.Int).Value = providerstaffID;
                    sqlCommand.Parameters.Add("@PHQS", SqlDbType.Int).Value = phqs;
                    sqlCommand.Parameters.Add("@Total", SqlDbType.Int).Value = total;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    return sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public async Task<WorkbooksPatientFollowup> GetWorkbooksPatientPHQ9FollowUp(int userId, int formResponseId, int patientID)
        {
            DataTable dataTable = new DataTable();            
            WorkbooksPatientFollowup workbookspatientfollowup;

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPHQ9Workbooks_PatientFollowUp", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;
                    sqlCommand.Parameters.Add("@PatientID", SqlDbType.Int).Value = patientID;

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        workbookspatientfollowup = (from DataRow dr in dataTable.Rows
                                                    select new WorkbooksPatientFollowup()
                                                    {
                                                        FormResponseId = int.Parse(dr["FormResponseId"].ToString()),
                                                        PatientId = int.Parse(dr["PatientId"].ToString()),
                                                        ActionPlanGiven = bool.Parse(dr["Action_Plan_Given"].ToString()),
                                                        ManagedByExternalProvider = bool.Parse(dr["Managed_by_External_Provider"].ToString()),
                                                        DateOfLastCommunicationByExternalProvider = (dr["Date_of_last_communication_by_external_provider"] == DBNull.Value ? (DateTime?)null: (DateTime.Parse(dr["Date_of_last_communication_by_external_provider"].ToString()))),
                                                        FollowupPhoneCallOneToTwoWeeks = bool.Parse(dr["Follow-up_phone_call_1-2_weeks"].ToString()),
                                                        DateOfFollowupCall = (dr["Date_of_follow_up_call_(optional)"] == DBNull.Value ? (DateTime?)null : (DateTime.Parse(dr["Date_of_follow_up_call_(optional)"].ToString()))),
                                                        OneMonthFollowupVisit = bool.Parse(dr["1_Month_follow_up_visit"].ToString()),
                                                        DateOfOneMonthVisit = (dr["Date_of_1_month_visit_(optional)"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["Date_of_1_month_visit_(optional)"].ToString())),
                                                        OneMonthFolllowupPHQ9Score = int.Parse(dr["1_Month_follow-up_PHQ-9_Score"].ToString())
                                                    }).SingleOrDefault();
                    }                  
                }
            }
            return workbookspatientfollowup;
        }
      
        public async Task<int> UpdateWorkbooksPatientFollowup(int userId, int formResponseId, int patientId, bool actionPlanGiven, bool managedByExternalProvider, DateTime? dateOfLastCommunicationByExternalProvider, bool followupPhoneCallOneToTwoWeeks, DateTime? dateOfFollowupCall, bool oneMonthFollowupVisit, DateTime? dateOfOneMonthVisit, int oneMonthFolllowupPHQ9Score)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spUpdatePHQ9Workbook_PatientFollowUp", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;
                    sqlCommand.Parameters.Add("@PatientID", SqlDbType.Int).Value = patientId;
                    sqlCommand.Parameters.Add("@Action_Plan_Given", SqlDbType.Bit).Value = actionPlanGiven;
                    sqlCommand.Parameters.Add("@Managed_by_External_Provider", SqlDbType.Bit).Value = managedByExternalProvider;
                    sqlCommand.Parameters.Add("@Date_of_last_communication_by_external_provider", SqlDbType.DateTime).Value = dateOfLastCommunicationByExternalProvider;
                    sqlCommand.Parameters.Add("@Followup_phone_call_1_2_weeks", SqlDbType.Bit).Value = followupPhoneCallOneToTwoWeeks;
                    sqlCommand.Parameters.Add("@Date_of_follow_up_call", SqlDbType.DateTime).Value = dateOfFollowupCall;
                    sqlCommand.Parameters.Add("@1_Month_follow_up_visit", SqlDbType.Bit).Value = oneMonthFollowupVisit;
                    sqlCommand.Parameters.Add("@Date_of_1_month_visit", SqlDbType.DateTime).Value = dateOfOneMonthVisit;
                    sqlCommand.Parameters.Add("@1_Month_followup_PHQ9_Score", SqlDbType.Int).Value = oneMonthFolllowupPHQ9Score;

                    await sqlConnection.OpenAsync();

                    //return rows of data affected
                    return sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public async Task<List<WorkbooksForms>> GetWorkbooksForms(int userId)
        {
            DataTable dataTable = new DataTable();
            List<WorkbooksForms> workbooksForms;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetWorkbookForms", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@userId", SqlDbType.Int).Value = userId;

                    await sqlConnection.OpenAsync();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        workbooksForms = (from DataRow dr in dataTable.Rows
                                   select new WorkbooksForms()
                                   {
                                       Id = Convert.ToInt32(dr["FormId"]),
                                       Label = dr["Survey_Title"].ToString()
                                   }
                            ).ToList();
                    }
                    return workbooksForms;
                }
            }
        }
    }
}