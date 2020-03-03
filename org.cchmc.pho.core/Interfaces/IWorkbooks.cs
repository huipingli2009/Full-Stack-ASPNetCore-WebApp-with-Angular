using org.cchmc.pho.core.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IWorkbooks
    {       
        Task<List<WorkbooksPatient>> ListPatients(int userId, int formResponseId, string nameSearch);
        Task<WorkbooksPractice> GetPracticeWorkbooks(int userId, int formResponseId);
        Task<List<WorkbooksProvider>> GetPracticeWorkbooksProviders(int userId, int formResponseId);
    }
}