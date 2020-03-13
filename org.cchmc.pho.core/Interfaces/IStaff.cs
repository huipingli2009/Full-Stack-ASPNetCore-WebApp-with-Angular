using org.cchmc.pho.core.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IStaff
    {
        Task<List<Staff>> ListStaff(int userId, string topfilter, string tagfilter, string namesearch);
        Task<StaffDetail> GetStaffDetails(int userId, int staffId);
        Task<StaffDetail> GetStaffDetailsById(int staffId);
        Task<StaffDetail> UpdateStaffDetails(int userId, StaffDetail staffDetail);
        Task<List<Position>> ListPositions();
        Task<List<Credential>> ListCredentials();
        Task<List<Responsibility>> ListResponsibilities();
        Task<List<Provider>> ListProviders(int userId);
        bool IsStaffInSamePractice(int userId, int staffId);
        Task<bool> SignLegalDisclaimer(int userId);
        Task<List<Practice>> GetPracticeList(int userId);
        Task<bool> SwitchPractice(int userId, int practiceID);
    }
}
