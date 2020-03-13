using org.cchmc.pho.core.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IWorkbooks
    {       
        Task<List<WorkbooksPatient>> ListPatients(int userId, int formResponseId);
        Task<List<WorkbookPatientNameSearch>> SearchPatients(int userId, string search);
        Task<WorkbooksPractice> GetPracticeWorkbooks(int userId, int formResponseId);
        Task<List<WorkbooksProvider>> GetPracticeWorkbooksProviders(int userId, int formResponseId);
        Task<List<WorkbooksLookup>> GetWorkbooksLookups(int userId, string nameSearch);
        Task<int> AddPatientToWorkbooks (int userId, int formResponseId, int patientID, int providerstaffID, DateTime? dos, int phq9score, bool action);
        Task<int> UpdateWorkbooksProviders(int userId, int formResponseId, int providerstaffID, int phqs, int total);
        Task<WorkbooksPatientFollowup> GetWorkbooksPatientPHQ9FollowUp(int userId, int formResponseId, int patientID);       
        Task<int> UpdateWorkbooksPatientFollowup(int userId, int formResponseId, int patientId, bool actionPlanGiven, bool managedByExternalProvider, DateTime? dateOfLastCommunicationByExternalProvider, bool followupPhoneCallOneToTwoWeeks, DateTime? dateOfFollowupCall, bool oneMonthFollowupVisit, DateTime? dateOfOneMonthVisit, int oneMonthFolllowupPHQ9Score);
    }
}