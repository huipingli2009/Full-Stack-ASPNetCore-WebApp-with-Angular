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

        public async Task<WorkbooksDepressionConfirmation> GetDepressionConfirmation(int userId, int formResponseId) 
        {
            DataTable dataTable = new DataTable();
            WorkbooksDepressionConfirmation confirmation = new WorkbooksDepressionConfirmation();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPHQ9Workbooks_Confirmations", sqlConnection))
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
                            var workbookspt = new WorkbooksDepressionConfirmation()
                            {
                                FormResponseId = int.Parse(dr["FormResponseId"].ToString()),
                                AllProvidersConfirmed = bool.Parse(dr["AllProvidersConfirmed"].ToString()),
                                NoPatientsConfirmed = bool.Parse(dr["NoPatientsConfirmed"].ToString())
                            };

                            confirmation = workbookspt;
                        }
                    }
                }
            }
            return confirmation;
        }
        public async Task<bool> UpdateDepressionConfirmation(int userId, WorkbooksDepressionConfirmation confirmation)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spPostPHQ9Workbook_Confirmations", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = confirmation.FormResponseId;
                    sqlCommand.Parameters.Add("@AllProvidersConfirmed", SqlDbType.Bit).Value = confirmation.AllProvidersConfirmed;
                    sqlCommand.Parameters.Add("@NoPatientsConfirmed", SqlDbType.Bit).Value = confirmation.NoPatientsConfirmed;

                    await sqlConnection.OpenAsync();

                    //return rows of data affected
                    return (bool)sqlCommand.ExecuteScalar();
                }
            }
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
                                Asthma_Score = (dr["AsthmaScore"] == DBNull.Value ? 0 : Convert.ToInt32(dr["AsthmaScore"].ToString())),
                                ActionPlanGiven = (dr["ActionPlanGiven"] == DBNull.Value ? Convert.ToBoolean(0) : Convert.ToBoolean(dr["ActionPlanGiven"].ToString())),
                                AssessmentCompleted = (dr["AssessmentCompleted"] == DBNull.Value ? Convert.ToBoolean(0) : Convert.ToBoolean(dr["AssessmentCompleted"].ToString())),
                            };

                            if (dr["Treatment"] != DBNull.Value && int.TryParse(dr["Treatment"].ToString(), out int intTreatment))
                            {
                                AsthmaTreatmentPlan treatment = new AsthmaTreatmentPlan();
                                treatment.TreatmentId = intTreatment;
                                treatment.TreatmentLabel = dr["TreatmentLabel"].ToString();
                                workbookspt.Treatment = treatment;
                            }

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
                                FormResponseId = (dr["FormResponseID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["FormResponseID"].ToString()))
                            };

                            if (DateTime.TryParse(dr["ReportingPeriod"].ToString(), out DateTime reportingDate))
                            {
                                workbookslookup.ReportingPeriod = reportingDate.ToShortDateString();
                            }
                            else
                            {
                                workbookslookup.ReportingPeriod = dr["ReportingPeriod"].ToString();
                            }

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

        public async Task<bool> AddPatientToAsthmaWorkbooks(int userId, int formResponseId, int patientID, int providerstaffID, DateTime? dos, int asthmascore, bool assessmentCompleted, int treatment, bool actionPlanGiven)
        {            
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spPostAsthmaWorkbook_Patient", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;
                    sqlCommand.Parameters.Add("@PatientID", SqlDbType.Int).Value = patientID;
                    sqlCommand.Parameters.Add("@ProviderStaffID", SqlDbType.Int).Value = providerstaffID;
                    sqlCommand.Parameters.Add("@AsthmaScore", SqlDbType.Int).Value = asthmascore;
                    sqlCommand.Parameters.Add("@DateOfService", SqlDbType.DateTime).Value = dos;

                    if (treatment > 0)
                    {
                        sqlCommand.Parameters.Add("@TreamentID", SqlDbType.Int).Value = treatment;
                    }
                    else
                    {
                        sqlCommand.Parameters.Add("@TreamentID", SqlDbType.Int).Value = DBNull.Value;

                    }
                    sqlCommand.Parameters.Add("@ActionPlan", SqlDbType.Int).Value = actionPlanGiven;
                    sqlCommand.Parameters.Add("@AssessmentCompleted", SqlDbType.Int).Value = assessmentCompleted;

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
                using (SqlCommand sqlCommand = new SqlCommand("spRemoveWorkbook_Patient", sqlConnection))
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
                                                        OneMonthFolllowupPHQ9Score = dr["1_Month_follow-up_PHQ-9_Score"].ToString(),
                                                        PHQ9FollowUpNotes = dr["PHQ-9_Follow_Up_Notes"].ToString()
                                                    }).SingleOrDefault();
                    }                  
                }
            }
            return workbookspatientfollowup;
        }
      
        public async Task<int> UpdateWorkbooksPatientFollowup(int userId, int formResponseId, int patientId, bool actionPlanGiven, bool managedByExternalProvider, DateTime? dateOfLastCommunicationByExternalProvider, bool followupPhoneCallOneToTwoWeeks, DateTime? dateOfFollowupCall, bool oneMonthFollowupVisit, DateTime? dateOfOneMonthVisit, string oneMonthFolllowupPHQ9Score, string pHQ9FollowUpNotes)
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
                    sqlCommand.Parameters.Add("@1_Month_followup_PHQ9_Score", SqlDbType.VarChar, 20).Value = oneMonthFolllowupPHQ9Score;
                    sqlCommand.Parameters.Add("@PHQ9_Follow_Up_Notes", SqlDbType.VarChar, 500).Value = pHQ9FollowUpNotes;

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

        public async Task<List<AsthmaTreatmentPlan>> GetAsthmaTreatmentPlan()      
        {
            DataTable dataTable = new DataTable();
            List<AsthmaTreatmentPlan> asthmatreatmentplan;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetAsthmaWorkbooks_TreatmentLookup", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    await sqlConnection.OpenAsync();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        asthmatreatmentplan = (from DataRow dr in dataTable.Rows
                                 select new AsthmaTreatmentPlan()
                                 {
                                     TreatmentId = (dr["TreatmentID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TreatmentID"].ToString())),

                                     TreatmentLabel = dr["TreatmentLabel"].ToString()
                                 }
                        ).ToList();
                    }
                }
            }
            return asthmatreatmentplan;
        }

        public async Task<AsthmaWorkbooksPractice> GetAsthmaPracticeWorkbooks(int userId, int formResponseId)
        {   

            DataTable dataTable = new DataTable();
            AsthmaWorkbooksPractice asthmaworkbookpractice;

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetAsthmaWorkbooks_Practice", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@FormResponseId", SqlDbType.Int).Value = formResponseId;

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);

                        asthmaworkbookpractice = (from DataRow dr in dataTable.Rows
                                            select new AsthmaWorkbooksPractice()
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
            return asthmaworkbookpractice;
        }
    }
}