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
                                     IsRegistry = (dr["RegistryYN"] != DBNull.Value && Convert.ToBoolean(dr["RegistryYN"])),
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
                                     NPI = (dr["NPI"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NPI"].ToString())),
                                     IsLeadPhysician = (dr["LeadPhysician"] != DBNull.Value && Convert.ToBoolean(dr["LeadPhysician"])),
                                     IsQITeam = (dr["QITeam"] != DBNull.Value && Convert.ToBoolean(dr["QITeam"])),
                                     IsPracticeManager = (dr["PracticeManager"] != DBNull.Value && Convert.ToBoolean(dr["PracticeManager"])),
                                     IsInterventionContact = (dr["InterventionContact"] != DBNull.Value && Convert.ToBoolean(dr["InterventionContact"])),
                                     IsQPLLeader = (dr["QPLLeader"] != DBNull.Value && Convert.ToBoolean(dr["QPLLeader"])),
                                     IsPHOBoard = (dr["PHOBoard"] != DBNull.Value && Convert.ToBoolean(dr["PHOBoard"])),
                                     IsOVPCABoard = (dr["OVPCABoard"] != DBNull.Value && Convert.ToBoolean(dr["OVPCABoard"])),
                                     IsRVPIBoard = (dr["RVPIBoard"] != DBNull.Value && Convert.ToBoolean(dr["RVPIBoard"])),
                                 }
                        ).SingleOrDefault();
                    }
                    return staffDetail;
                }
            }
        }

        public async Task UpdateStaffDetails(int userId, StaffDetail staffDetail)
        {

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spUpdateStaff", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@StaffID", SqlDbType.Int).Value = staffDetail.Id;
                    sqlCommand.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = staffDetail.FirstName;
                    sqlCommand.Parameters.Add("@LastName", SqlDbType.VarChar).Value = staffDetail.LastName;
                    sqlCommand.Parameters.Add("@EmailAddress", SqlDbType.VarChar).Value = staffDetail.Email;
                    sqlCommand.Parameters.Add("@Phone", SqlDbType.VarChar).Value = staffDetail.Phone;
                    sqlCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = staffDetail.StartDate;
                    sqlCommand.Parameters.Add("@StaffPositionId", SqlDbType.Int).Value = staffDetail.PositionId;
                    sqlCommand.Parameters.Add("@CredentialId", SqlDbType.Int).Value = staffDetail.CredentialId;
                    sqlCommand.Parameters.Add("@NPI", SqlDbType.Int).Value = staffDetail.NPI;
                    sqlCommand.Parameters.Add("@LeadPhysician", SqlDbType.Bit).Value = staffDetail.IsLeadPhysician;
                    sqlCommand.Parameters.Add("@QITeam", SqlDbType.Bit).Value = staffDetail.IsQITeam;
                    sqlCommand.Parameters.Add("@PracticeManager", SqlDbType.Bit).Value = staffDetail.IsPracticeManager;
                    sqlCommand.Parameters.Add("@InterventionContact", SqlDbType.Bit).Value = staffDetail.IsInterventionContact;
                    sqlCommand.Parameters.Add("@QPLLeader", SqlDbType.Bit).Value = staffDetail.IsQPLLeader;
                    sqlCommand.Parameters.Add("@PHOBoard", SqlDbType.Bit).Value = staffDetail.IsPHOBoard;
                    sqlCommand.Parameters.Add("@OVPCABoard", SqlDbType.Bit).Value = staffDetail.IsOVPCABoard;
                    sqlCommand.Parameters.Add("@RVPIBoard", SqlDbType.Bit).Value = staffDetail.IsRVPIBoard;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    sqlCommand.ExecuteNonQuery();

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
