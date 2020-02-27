using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Data;
using System.Linq;


namespace org.cchmc.pho.core.DataAccessLayer
{
    public class StaffDAL: IStaff
    {
        private readonly ConnectionStrings _connectionStrings;
        public StaffDAL(IOptions<ConnectionStrings> options, ILogger<StaffDAL> logger)
        {
            _connectionStrings = options.Value;
        }

        public async Task<List<Staff>> ListStaff(int userId, string topfilter, string tagfilter, string namesearch)
        {
            DataTable dataTable = new DataTable();
            List<Staff> staff = new List<Staff>();
            return staff;
        }

        public async Task<List<Position>> ListPositions()
        {
            DataTable dataTable = new DataTable();
            List<Position> positions = new List<Position>();
            return positions;
        }

        public async Task<List<Credential>> ListCredentials()
        {
            DataTable dataTable = new DataTable();
            List<Credential> credentials = new List<Credential>();
            return credentials;
        }

        public async Task<List<Responsibility>> ListResponsibilities()
        {
            DataTable dataTable = new DataTable();
            List<Responsibility> responsibilities = new List<Responsibility>();
            return responsibilities;
        }
    }
}
