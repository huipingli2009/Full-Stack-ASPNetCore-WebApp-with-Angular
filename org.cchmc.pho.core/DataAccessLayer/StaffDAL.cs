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
            List<Position> positions;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPositionList", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        positions = (from DataRow dr in dataTable.Rows
                                       select new Position()
                                       {
                                           Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                           Name = dr["Position"].ToString()
                                       }
                        ).ToList();
                    }
                    return positions;
                }
            }
        }

        public async Task<List<Credential>> ListCredentials()
        {
            DataTable dataTable = new DataTable();
            List<Credential> credentials;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetCredentialList", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        credentials = (from DataRow dr in dataTable.Rows
                            select new Credential()
                            {
                                Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                Name = dr["CredentialName"].ToString(),
                                Description = dr["CredDesc"].ToString()
                            }
                        ).ToList();
                    }
                    return credentials;
                }
            }
        }

        public async Task<List<Responsibility>> ListResponsibilities()
        {
            DataTable dataTable = new DataTable();
            List<Responsibility> responsibilities = new List<Responsibility>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetResponsibilityList", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        responsibilities = (from DataRow dr in dataTable.Rows
                                     select new Responsibility()
                                     {
                                         Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                         Name = dr["Responsibility"].ToString(),
                                         Type = dr["ResponsibiltyType"].ToString()
                                     }
                        ).ToList();
                    }
                    return responsibilities;
                }
            }
        }
    }
}
