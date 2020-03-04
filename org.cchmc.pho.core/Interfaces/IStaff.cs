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
        Task UpdateStaffDetails(int userId, StaffDetail staffDetail);
        Task<List<Position>> ListPositions();
        Task<List<Credential>> ListCredentials();
        Task<List<Responsibility>> ListResponsibilities();
    }
}
