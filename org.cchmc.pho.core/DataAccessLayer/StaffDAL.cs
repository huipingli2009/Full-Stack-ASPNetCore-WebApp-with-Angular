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
        private readonly ILogger<StaffDAL> _logger;
        public StaffDAL(IOptions<ConnectionStrings> options, ILogger<StaffDAL> logger)
        {
            _connectionStrings = options.Value;
            _logger = logger;            
        }

        public async Task<List<Staff>> ListStaff(int userId)
        {
            DataTable dataTable = new DataTable();
            List<Staff> staff = new List<Staff>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetStaff", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        foreach(DataRow dr in dataTable.Rows)
                        {
                            var record = new Staff()
                            {
                                Id = (dr["StaffID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StaffID"].ToString())),
                                FirstName = dr["FirstName"].ToString(),
                                LastName = dr["LastName"].ToString(),
                                Email = dr["EmailAddress"].ToString(),
                                Phone = dr["Phone"].ToString(),
                                IsRegistry = (dr["RegistryYN"] != DBNull.Value && Convert.ToBoolean(dr["RegistryYN"])),
                                Responsibilities = dr["Responsibilities"].ToString(),
                                LegalDisclaimerSigned = dr["LegalDisclaimerSigned"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["LegalDisclaimerSigned"].ToString()),
                                

                            };
                            if (dr["CredentialId"] != DBNull.Value && int.TryParse(dr["CredentialId"].ToString(), out int intCredential))
                            {
                                Credential cred = new Credential();
                                cred.Id = intCredential;
                                cred.Name = dr["CredentialName"].ToString();
                                record.Credentials = cred;
                            }
                            if (dr["PositionId"] != DBNull.Value && int.TryParse(dr["PositionId"].ToString(), out int intPosition))
                            {
                                Position pos = new Position();
                                pos.Id = intPosition;
                                pos.Name = dr["Position"].ToString();
                                pos.PositionType= dr["PositionType"].ToString();
                                record.Position = pos;
                            }
                            staff.Add(record);
                        }
                    }
                    return staff;
                }
            }
        }

        //NOTE: This method exists for Chris's code to doublecheck something in the UserController. It simply doesn't require a userId, that's the only difference. It is not consumed by the StaffController.
        public async Task<StaffDetail> GetStaffDetailsById(int staffId)
        {
            DataTable dataTable = new DataTable();
            StaffDetail staffDetail = null;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetStaffDetailUsingStaffID", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@StaffID", SqlDbType.Int).Value = staffId;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        //List<Location> locations = await ListLocations(userId);

                        foreach (DataRow dr in dataTable.Rows)
                        {
                            staffDetail = new StaffDetail()
                            {
                                Id = (dr["StaffID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StaffID"].ToString())),
                                FirstName = dr["FirstName"].ToString(),
                                LastName = dr["LastName"].ToString(),
                                Email = dr["EmailAddress"].ToString(),
                                Phone = dr["Phone"].ToString(),
                                StartDate = (dr["StartDate"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["StartDate"].ToString())),
                                EndDate = (dr["EndDate"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["EndDate"].ToString())),
                                DeletedFlag = (dr["DeletedFlag"] != DBNull.Value && Convert.ToBoolean(dr["DeletedFlag"])),
                                PositionId = (dr["StaffPositionId"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["StaffPositionId"].ToString())),
                                CredentialId = (dr["CredentialId"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["CredentialId"].ToString())),
                                NPI = (dr["NPI"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["NPI"].ToString())),
                                IsLeadPhysician = (dr["LeadPhysician"] != DBNull.Value && Convert.ToBoolean(dr["LeadPhysician"])),
                                IsQITeam = (dr["QITeam"] != DBNull.Value && Convert.ToBoolean(dr["QITeam"])),
                                IsPracticeManager = (dr["PracticeManager"] != DBNull.Value && Convert.ToBoolean(dr["PracticeManager"])),
                                IsInterventionContact = (dr["InterventionContact"] != DBNull.Value && Convert.ToBoolean(dr["InterventionContact"])),
                                IsQPLLeader = (dr["QPLLeader"] != DBNull.Value && Convert.ToBoolean(dr["QPLLeader"])),
                                IsPHOBoard = (dr["PHOBoard"] != DBNull.Value && Convert.ToBoolean(dr["PHOBoard"])),
                                IsOVPCABoard = (dr["OVPCABoard"] != DBNull.Value && Convert.ToBoolean(dr["OVPCABoard"])),
                                IsRVPIBoard = (dr["RVPIBoard"] != DBNull.Value && Convert.ToBoolean(dr["RVPIBoard"])),
                                Locations = new List<Location>()
                            };

                        }
                        return staffDetail;
                    }
                }
            }
        }

        public async Task<StaffDetail> GetStaffDetails(int userId, int staffId)
        {
            DataTable dataTable = new DataTable();
            StaffDetail staffDetail = null;
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
                        List<Location> locations = await ListLocations(userId);

                        foreach (DataRow dr in dataTable.Rows)
                        {
                            staffDetail = new StaffDetail()
                            {
                                Id = (dr["StaffID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StaffID"].ToString())),
                                FirstName = dr["FirstName"].ToString(),
                                LastName = dr["LastName"].ToString(),
                                Email = dr["EmailAddress"].ToString(),
                                Phone = dr["Phone"].ToString(),
                                StartDate = (dr["StartDate"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["StartDate"].ToString())),
                                EndDate = (dr["EndDate"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["EndDate"].ToString())),
                                DeletedFlag = (dr["DeletedFlag"] != DBNull.Value && Convert.ToBoolean(dr["DeletedFlag"])),
                                PositionId = (dr["StaffPositionId"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["StaffPositionId"].ToString())),
                                CredentialId = (dr["CredentialId"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["CredentialId"].ToString())),
                                NPI = (dr["NPI"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["NPI"].ToString())),
                                IsLeadPhysician = (dr["LeadPhysician"] != DBNull.Value && Convert.ToBoolean(dr["LeadPhysician"])),
                                IsQITeam = (dr["QITeam"] != DBNull.Value && Convert.ToBoolean(dr["QITeam"])),
                                IsPracticeManager = (dr["PracticeManager"] != DBNull.Value && Convert.ToBoolean(dr["PracticeManager"])),
                                IsInterventionContact = (dr["InterventionContact"] != DBNull.Value && Convert.ToBoolean(dr["InterventionContact"])),
                                IsQPLLeader = (dr["QPLLeader"] != DBNull.Value && Convert.ToBoolean(dr["QPLLeader"])),
                                IsPHOBoard = (dr["PHOBoard"] != DBNull.Value && Convert.ToBoolean(dr["PHOBoard"])),
                                IsOVPCABoard = (dr["OVPCABoard"] != DBNull.Value && Convert.ToBoolean(dr["OVPCABoard"])),
                                IsRVPIBoard = (dr["RVPIBoard"] != DBNull.Value && Convert.ToBoolean(dr["RVPIBoard"])),
                                Locations = new List<Location>()
                            };

                            if (!string.IsNullOrWhiteSpace(dr["LocationID"].ToString()))
                            {
                                foreach (int locationId in dr["LocationID"].ToString().Split(',').Select(p => int.Parse(p)))
                                {
                                    if (locations.Any(l => l.Id == locationId))
                                        staffDetail.Locations.Add(locations.First(p => p.Id == locationId));
                                    else
                                        _logger.LogError("An unmapped location id was returned by the database ");
                                }
                            }

                        }
                        return staffDetail;
                    }
                }
            }
        }
        public async Task<StaffDetail> UpdateStaffDetails(int userId, StaffDetail staffDetail)
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
                    sqlCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = staffDetail.EndDate;
                    sqlCommand.Parameters.Add("@DeletedFlag", SqlDbType.Bit).Value = staffDetail.DeletedFlag;
                    sqlCommand.Parameters.Add("@StaffPositionId", SqlDbType.Int).Value = (!staffDetail.PositionId.HasValue ? (int?)null : staffDetail.PositionId.Value);
                    sqlCommand.Parameters.Add("@CredentialId", SqlDbType.Int).Value = (!staffDetail.CredentialId.HasValue ? (int?)null : staffDetail.CredentialId.Value);
                    sqlCommand.Parameters.Add("@NPI", SqlDbType.Int).Value = (!staffDetail.NPI.HasValue ? (int?)null : staffDetail.NPI.Value);
                    sqlCommand.Parameters.Add("@LeadPhysician", SqlDbType.Bit).Value = staffDetail.IsLeadPhysician;
                    sqlCommand.Parameters.Add("@QITeam", SqlDbType.Bit).Value = staffDetail.IsQITeam;
                    sqlCommand.Parameters.Add("@PracticeManager", SqlDbType.Bit).Value = staffDetail.IsPracticeManager;
                    sqlCommand.Parameters.Add("@InterventionContact", SqlDbType.Bit).Value = staffDetail.IsInterventionContact;
                    sqlCommand.Parameters.Add("@QPLLeader", SqlDbType.Bit).Value = staffDetail.IsQPLLeader;
                    sqlCommand.Parameters.Add("@PHOBoard", SqlDbType.Bit).Value = staffDetail.IsPHOBoard;
                    sqlCommand.Parameters.Add("@OVPCABoard", SqlDbType.Bit).Value = staffDetail.IsOVPCABoard;
                    sqlCommand.Parameters.Add("@RVPIBoard", SqlDbType.Bit).Value = staffDetail.IsRVPIBoard;
                    sqlCommand.Parameters.Add("@LocationIds", SqlDbType.VarChar).Value = string.Join(",", (from l in staffDetail.Locations select l.Id).ToArray());

                    sqlConnection.Open();

                    //Execute Stored Procedure
                    sqlCommand.ExecuteNonQuery();

                    return await GetStaffDetails(userId, staffDetail.Id);
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
            List<Responsibility> responsibilities;
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

        public async Task<List<Provider>> ListProviders(int userId)
        {
            DataTable dataTable = new DataTable();
            List<Provider> providers;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetProviderList", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        providers = (from DataRow dr in dataTable.Rows
                                      select new Provider()
                                      {
                                          Id = (dr["StaffId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StaffId"].ToString())),
                                          Name = dr["StaffName"].ToString()
                                      }
                        ).ToList();
                    }
                }
            }
            return providers;
        }

        public async Task<List<Location>> ListLocations(int userId)
        {
            DataTable dataTable = new DataTable();
            List<Location> locations = new List<Location>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetLocationList", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var record = new Location()
                            {
                                Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                Name = dr["LocationName"].ToString()
                            };
                            locations.Add(record);
                        }
                    }
                    return locations;
                }
            }
        }
        public async Task<bool> RemoveStaff(int userId, int staffId, DateTime endDate, bool? deletedFlag)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spRemoveStaff", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@StaffID", SqlDbType.Int).Value = staffId;
                    sqlCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = endDate;
                    sqlCommand.Parameters.Add("@DeletedFlag", SqlDbType.Bit).Value = deletedFlag;
                    sqlConnection.Open();

                    return (bool)sqlCommand.ExecuteScalar();
                }
            }
        }

        public bool IsStaffInSamePractice(int userId, int staffId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spCheckPermissions", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@StaffID", SqlDbType.Int).Value = staffId;
                    sqlConnection.Open();

                    return (bool)sqlCommand.ExecuteScalar();
                }
            }
        }
        public async Task<bool> SignLegalDisclaimer(int userId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spUpdateUserLegalDisclaimer", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    await sqlConnection.OpenAsync();

                    return (sqlCommand.ExecuteNonQuery() > 0);
                }
            }
        }

        public async Task<bool> SwitchPractice(int userId, int practiceID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spUpdateDefPracticeID", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@PracticeID", SqlDbType.Int).Value = practiceID;
                    await sqlConnection.OpenAsync();

                    return (sqlCommand.ExecuteNonQuery() > 0);
                }
            }
        }

        public async Task<SelectPractice> GetPracticeList(int userId)
        {
            DataSet practicesDS = new DataSet();
            SelectPractice selectpractice = new SelectPractice()
            {
                PracticeList = new List<Practice>()
            };

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPracticeList", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    await sqlConnection.OpenAsync();
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(practicesDS);

                        foreach (DataRow dr in practicesDS.Tables[0].Rows)
                        {
                            var practice = new Practice()
                            {
                                Id = (dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"].ToString())),
                                Name = dr["PracticeName"].ToString()
                            };
                            selectpractice.PracticeList.Add(practice);
                        }

                        selectpractice.CurrentPracticeId = 51;
                        if (practicesDS.Tables.Count > 1 && practicesDS.Tables[1].Rows.Count > 0 && practicesDS.Tables[1].Rows[0].ItemArray.Length > 0)
                        {
                            if(int.TryParse(practicesDS.Tables[1].Rows[0].ItemArray[0].ToString(), out int result))
                                selectpractice.CurrentPracticeId = result;
                        }                                          
                    }
                }
            }
            return selectpractice;
        }
    }   
}
