using org.cchmc.pho.core.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IStaff
    {
        Task<List<Staff>> ListStaff(int userId);
        Task<StaffDetails> GetStaffDetails(int userId, int staffId);
        Task<StaffDetails> GetStaffDetailsById(int staffId);
        Task<StaffDetails> UpdateStaffDetails(int userId, StaffDetails staffDetail);
        Task<bool> RemoveStaff(int userId, int staffId, DateTime endDate, bool? deletedFlag);
        Task<List<Position>> ListPositions();
        Task<List<Credential>> ListCredentials();
        Task<List<Responsibility>> ListResponsibilities();
        Task<List<Provider>> ListProviders(int userId);
        Task<List<Location>> ListLocations(int userId);
        bool IsStaffInSamePractice(int userId, int staffId);
        Task<bool> SignLegalDisclaimer(int userId);
        Task<SelectPractice> GetPracticeList(int userId);
        Task<bool> SwitchPractice(int userId, int practiceID);
        //Task<bool> AddNewStaff(int userId, int staffPositionId, string firstName, string lastName, string email, string phone, int locationId, DateTime startDate);
        Task<StaffDetails> AddNewStaff(int userId, StaffDetails staffDetails);        
        Task<PracticeCoach> GetPracticeCoach(int userId);
    }
}
