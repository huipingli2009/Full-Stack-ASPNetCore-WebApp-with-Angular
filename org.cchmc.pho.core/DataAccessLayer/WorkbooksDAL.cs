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
        public async Task<List<WorkbooksPatient>> ListPatients(int userId, int formResponseId)
        {
            DataTable dataTable = new DataTable();
            List<WorkbooksPatient> workbookspatients = new List<WorkbooksPatient>();

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
                            var workbookspt = new WorkbooksPatient()
                            {
                                FormResponseId = int.Parse(dr["FormResponseId"].ToString()),
                                PatientId = int.Parse(dr["PatientId"].ToString()),
                                Patient = dr["Patient"].ToString(),
                                DOB = (dr["DOB"] == DBNull.Value ? (DateTime?)null : (DateTime.Parse(dr["DOB"].ToString()))),
                                Phone = dr["Phone"].ToString(),
                                Provider = dr["Provider"].ToString(),
                                DateOfService = (dr["DateOfService"] == DBNull.Value ? (DateTime?)null : (DateTime.Parse(dr["DateOfService"].ToString()))),
                                PHQ9_Score = dr["PHQ9_Score"].ToString(),
                                ActionFollowUp = dr["ActionFollowUp"].ToString()
                            };

                            workbookspatients.Add(workbookspt);
                        }
                    }
                }
            }
            return workbookspatients;
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

        public async Task<List<WorkbooksLookup>> GetWorkbooksLookups(int userId, string nameSearch)
        {
            DataTable dataTable = new DataTable();
            List<WorkbooksLookup> workbookslookups = new List<WorkbooksLookup>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPHQ9Workbooks", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@NameSearch", SqlDbType.Int).Value = nameSearch;

                    await sqlConnection.OpenAsync();
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var workbookslookup = new WorkbooksLookup()
                            {
                                FormResponseID = int.Parse(dr["FormResponseID"].ToString()),
                                ReportingMonth = (dr["ReportingMonth"] == DBNull.Value ? (DateTime?)null : (DateTime.Parse(dr["ReportingMonth"].ToString())))
                            };

                            workbookslookups.Add(workbookslookup);
                        }
                    }
                }
            }
            return workbookslookups;
        }

        public async Task UpdateWorkbooksPatient(int userId, int formResponseId, int patientID, int providerstaffID, DateTime? dos, int phq9score, bool action)
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
                    sqlCommand.ExecuteNonQuery();                    
                }
            }
        }

        public async Task UpdateWorkbooksProviders(int userId, int formResponseId, int providerstaffID, int phqs, int total)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spUpdatePHQ9Workbook_Providers", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;
                    sqlCommand.Parameters.Add("@ProviderStaffID", SqlDbType.Int).Value = providerstaffID;
                    sqlCommand.Parameters.Add("@PHQ9Score", SqlDbType.Int).Value = phqs;
                    sqlCommand.Parameters.Add("@Total", SqlDbType.Int).Value = total;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    sqlCommand.ExecuteNonQuery();
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
                                                        OneMonthFolllowupPHQ9Score = int.Parse(dr["1_Month_follow-up_PHQ-9_Score"].ToString()),
                                                        Improvement = bool.Parse(dr["Improvement"].ToString())
                                                    }).SingleOrDefault();
                    }                  
                }
            }
            return workbookspatientfollowup;
        }
    }
}