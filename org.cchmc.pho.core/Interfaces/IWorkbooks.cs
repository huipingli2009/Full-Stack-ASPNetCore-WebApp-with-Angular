using org.cchmc.pho.core.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IWorkbooks
    {
        Task<List<WorkbooksDepressionPatient>> GetDepressionPatientList(int userId, int formResponseId);
        Task<WorkbooksDepressionConfirmation> GetDepressionConfirmation(int userId, int formResponseId);
        Task<bool> UpdateDepressionConfirmation(int userId, WorkbooksDepressionConfirmation confirmation);
        Task<bool> UpdateQIQuestion(int userId, int formResponseId, Question question);
        Task<List<WorkbooksAsthmaPatient>> GetAsthmaPatientList(int userId, int formResponseId);
        Task<WorkbooksPractice> GetPracticeWorkbooks(int userId, int formResponseId);
        Task<List<WorkbooksProvider>> GetPracticeWorkbooksProviders(int userId, int formResponseId);
        Task<List<WorkbooksLookup>> GetWorkbooksLookups(int formId, int userId, string nameSearch);
        Task<bool> AddPatientToDepressionWorkbooks(int userId, int formResponseId, int patientID, int providerstaffID, DateTime? dos, int phq9score, bool action);
        Task<bool> RemovePatientFromWorkbooks(int userId, int formResponseId, int patientID);
        Task<bool> AddProviderToWorkbooks(int userId, int formResponseId, int providerId);
        Task<bool> RemoveProviderFromWorkbooks(int userId, int formResponseId, int providerId);
        Task<int> UpdateWorkbooksProviders(int userId, int formResponseId, int providerstaffID, int phqs, int total);
        Task<WorkbooksPatientFollowup> GetWorkbooksPatientPHQ9FollowUp(int userId, int formResponseId, int patientID);       
        Task<int> UpdateWorkbooksPatientFollowup(int userId, int formResponseId, int patientId, bool actionPlanGiven, bool managedByExternalProvider, DateTime? dateOfLastCommunicationByExternalProvider, bool followupPhoneCallOneToTwoWeeks, DateTime? dateOfFollowupCall, bool oneMonthFollowupVisit, DateTime? dateOfOneMonthVisit, string oneMonthFolllowupPHQ9Score, string PHQ9FollowUpNotes);
        Task<List<WorkbooksForms>> GetWorkbooksForms(int userid);
        Task<List<AsthmaTreatmentPlan>> GetAsthmaTreatmentPlan();
        Task<AsthmaWorkbooksPractice> GetAsthmaPracticeWorkbooks(int userId, int formResponseId);
        Task<bool> AddPatientToAsthmaWorkbooks(int userId, int formResponseId, int patientID, int providerstaffID, DateTime? dos, int asthmascore, bool assessmentCompleted, int treatmentplanId, bool actionPlanGiven);
        Task<QIWorkbookPractice> GetPracticeQIWorkbooks(int userId, int formResponseId);
        Task<QIWorkbookQuestions> GetQIWorkbookQuestions(int userId, int formResponseId);
    }
}