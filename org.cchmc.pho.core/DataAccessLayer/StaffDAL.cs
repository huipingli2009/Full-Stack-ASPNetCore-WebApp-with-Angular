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
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetStaff", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@TopFilter", SqlDbType.VarChar).Value = topfilter;
                    sqlCommand.Parameters.Add("@TagFilter", SqlDbType.VarChar).Value = tagfilter;
                    sqlCommand.Parameters.Add("@NameSearch", SqlDbType.VarChar).Value = namesearch;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        staff = (from DataRow dr in dataTable.Rows
                                 select new Staff()
                                 {
                                     Id = (dr["StaffID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StaffID"].ToString())),
                                     FirstName = dr["FirstName"].ToString(),
                                     LastName = dr["LastName"].ToString(),
                                     Email = dr["EmailAddress"].ToString(),
                                     Phone = dr["Phone"].ToString(),
                                     PositionId = (dr["PositionId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PositionId"].ToString())),
                                     CredentialId = (dr["CredentialId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CredentialId"].ToString())),
                                     Registry = (dr["RegistryYN"] == DBNull.Value ? false : Convert.ToBoolean(dr["RegistryYN"].ToString())),
                                     Responsibilities = dr["Responsibilities"].ToString()
                                 }
                        ).ToList();
                    }
                    return staff;
                }
            }
        }
        public async Task<StaffDetail> GetStaffDetails(int userId, int staffId)
        {
            DataTable dataTable = new DataTable();
            StaffDetail staffDetail;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetStaffDetail", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@StaffID", SqlDbType.Int).Value = staffId;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        staffDetail = (from DataRow dr in dataTable.Rows
                                 select new StaffDetail()
                                 {
                                     Id = (dr["StaffID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StaffID"].ToString())),
                                     FirstName = dr["FirstName"].ToString(),
                                     LastName = dr["LastName"].ToString(),
                                     Email = dr["EmailAddress"].ToString(),
                                     Phone = dr["Phone"].ToString(),
                                     StartDate = (dr["StartDate"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["StartDate"].ToString())),
                                     PositionId = (dr["StaffPositionId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StaffPositionId"].ToString())),
                                     CredentialId = (dr["CredentialId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CredentialId"].ToString())),
                                     LeadPhysician = (dr["LeadPhysician"] == DBNull.Value ? false : Convert.ToBoolean(dr["LeadPhysician"].ToString())),
                                     QITeam = (dr["QITeam"] == DBNull.Value ? false : Convert.ToBoolean(dr["QITeam"].ToString())),
                                     PracticeManager = (dr["PracticeManager"] == DBNull.Value ? false : Convert.ToBoolean(dr["PracticeManager"].ToString())),
                                     InterventionContact = (dr["InterventionContact"] == DBNull.Value ? false : Convert.ToBoolean(dr["InterventionContact"].ToString())),
                                     QPLLeader = (dr["QPLLeader"] == DBNull.Value ? false : Convert.ToBoolean(dr["QPLLeader"].ToString())),
                                     PHOBoard = (dr["PHOBoard"] == DBNull.Value ? false : Convert.ToBoolean(dr["PHOBoard"].ToString())),
                                     OVPCABoard = (dr["OVPCABoard"] == DBNull.Value ? false : Convert.ToBoolean(dr["OVPCABoard"].ToString())),
                                     RVPIBoard = (dr["RVPIBoard"] == DBNull.Value ? false : Convert.ToBoolean(dr["RVPIBoard"].ToString())),
                                 }
                        ).SingleOrDefault();
                    }
                    return staffDetail;
                }
            }
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
