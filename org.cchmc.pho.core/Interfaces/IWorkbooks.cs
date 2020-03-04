using org.cchmc.pho.core.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IWorkbooks
    {       
        Task<List<WorkbooksPatient>> ListPatients(int userId, int formResponseId);
        Task<WorkbooksPractice> GetPracticeWorkbooks(int userId, int formResponseId);
        Task<List<WorkbooksProvider>> GetPracticeWorkbooksProviders(int userId, int formResponseId);
        Task<List<WorkbooksLookup>> GetWorkbooksLookups(int userId, string nameSearch);
        Task UpdateWorkbooksPatient (int userId, int formResponseId, int patientID, int providerstaffID, DateTime dos, int phq9score, bool action);
    }
}