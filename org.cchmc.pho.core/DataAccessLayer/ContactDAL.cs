using Microsoft.Extensions.Options;
using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.DataAccessLayer
{
    public class ContactDAL : IContact
    {
        private readonly ConnectionStrings _connectionStrings;

        public ContactDAL(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }

        public async Task<List<Contact>> GetContacts(int userId, bool? qpl, string specialty, string membership, string board, string namesearch)
        {           
            List<Contact> contactList = new List<Contact>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("[spGetContactList]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@QPL", SqlDbType.Bit).Value = qpl;
                    sqlCommand.Parameters.Add("@Specialty", SqlDbType.VarChar, 50).Value = specialty;
                    sqlCommand.Parameters.Add("@Membership", SqlDbType.VarChar, 50).Value = membership;
                    sqlCommand.Parameters.Add("@Board", SqlDbType.VarChar, 50).Value = board;
                    sqlCommand.Parameters.Add("@NameSearch", SqlDbType.VarChar, 100).Value = namesearch;

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        DataTable dataTable = new DataTable();
                        da.Fill(dataTable);

                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var myContact = new Contact()
                            {
                                PracticeId = Convert.ToInt32(dr["PracticeID"]),
                                PracticeName = dr["Name"].ToString(),
                                PracticeType = dr["PracticeType"].ToString(),
                                EMR = dr["EMR"].ToString(),
                                Phone = dr["Phone"].ToString(),
                                Fax = dr["Fax"].ToString(),
                                WebsiteURL = dr["WebsiteURL"].ToString()
                            };

                            contactList.Add(myContact);
                        }
                    }

                }
            }
            return contactList;
        }

        public async Task<ContactPracticeDetails> GetContactPracticeDetails(int userId, int practiceId)
        {          
            ContactPracticeDetails contactPracticeDetail = null;

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                SqlCommand sqlCommand = new SqlCommand("spGetContactPracticeDet", sqlConnection);

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                sqlCommand.Parameters.Add("@PracticeID", SqlDbType.Int).Value = practiceId;

                await sqlConnection.OpenAsync();

                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    DataTable dataTable = new DataTable();
                    da.Fill(dataTable);

                    foreach (DataRow dr in dataTable.Rows)
                    {
                        contactPracticeDetail = new ContactPracticeDetails()
                        {
                            PracticeId = dr["PracticeID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PracticeId"].ToString()),
                            PracticeName = dr["PracticeName"].ToString(),
                            MemberSince = dr["MemberSince"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["MemberSince"].ToString()),
                            PracticeManager = dr["PracticeManager"].ToString(),
                            PMEmail = dr["PMEmail"].ToString(),
                            PIC = dr["PIC"].ToString(),
                            PICEmail = dr["PICEmail"].ToString(),
                            ContactPracticeLocations = await GetContactPracticeLocations(userId, practiceId)
                        };
                    }
                }
            }
            return contactPracticeDetail;
        }
        public async Task<List<ContactPracticeLocation>> GetContactPracticeLocations(int userId, int practiceId)
        {           
            List<ContactPracticeLocation> locations = new List<ContactPracticeLocation>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                SqlCommand sqlCommand = new SqlCommand("spGetContactPracticeLocations", sqlConnection);

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                sqlCommand.Parameters.Add("@PracticeID", SqlDbType.Int).Value = practiceId;

                await sqlConnection.OpenAsync();

                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    DataTable dataTable = new DataTable();
                    da.Fill(dataTable);

                    foreach (DataRow dr in dataTable.Rows)
                    {
                        ContactPracticeLocation location = new ContactPracticeLocation()
                        {
                            LocationId = dr["LocationID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["LocationID"].ToString()),
                            PracticeId = dr["PracticeID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PracticeID"].ToString()),
                            LocationName = dr["LocationName"].ToString(),
                            OfficePhone = dr["OfficePhone"].ToString(),
                            Fax = dr["Fax"].ToString(),
                            County = dr["County"].ToString(),
                            Address = dr["Address"].ToString(),
                            City = dr["City"].ToString(),
                            State = dr["State"].ToString(),
                            Zip = dr["Zip"].ToString()
                        };

                        locations.Add(location);
                    }
                }
            }
            return locations;
        }
        public async Task<List<ContactPracticeStaff>> GetContactPracticeStaffList(int userId, int practiceId)
        {
            List<ContactPracticeStaff> practiceStaffList = new List<ContactPracticeStaff>();          

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetContactStaffList", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@PracticeID", SqlDbType.Int).Value = practiceId;

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        DataTable dataTable = new DataTable();
                        da.Fill(dataTable);

                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var staff = new ContactPracticeStaff()
                            {
                                StaffId = (dr["StaffId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StaffId"].ToString())),
                                StaffName = dr["StaffName"].ToString()
                            };

                            practiceStaffList.Add(staff);
                        }
                    }
                }
            }
            return practiceStaffList;
        }
        public async Task<ContactPracticeStaffDetails> GetContactStaffDetails(int userId, int staffId)
        {           
            ContactPracticeStaffDetails staffDetails = null;

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                SqlCommand sqlCommand = new SqlCommand("spGetContactStaffDetail", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                sqlCommand.Parameters.Add("@StaffID", SqlDbType.Int).Value = staffId;
                await sqlConnection.OpenAsync();

                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    DataTable dataTable = new DataTable();
                    da.Fill(dataTable);

                    foreach (DataRow dr in dataTable.Rows)
                    {
                        staffDetails = new ContactPracticeStaffDetails()
                        {
                            Id = Convert.ToInt32(dr["StaffID"].ToString()),
                            StaffName = dr["StaffName"].ToString(),
                            Email = dr["EmailAddress"].ToString(),
                            Phone = dr["Phone"].ToString(),
                            Position = dr["Position"].ToString(),
                            NPI = (dr["NPI"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["NPI"].ToString())),
                            Locations = dr["Locations"].ToString(),
                            Specialty = dr["Specialty"].ToString(),
                            CommSpecialist = (dr["CommSpecialist"] != DBNull.Value && Convert.ToBoolean(dr["CommSpecialist"])),
                            OVPCAPhysician = (dr["OVPCAPhysician"] != DBNull.Value && Convert.ToBoolean(dr["OVPCAPhysician"])),
                            OVPCAMidLevel = (dr["OVPCAMidLevel"] != DBNull.Value && Convert.ToBoolean(dr["OVPCAMidLevel"])),
                            Responsibilities = dr["Responsibilities"].ToString(),
                            BoardMembership = dr["BoardMembership"].ToString(),
                            NotesAboutProvider = dr["NotesAboutProvider"].ToString()
                        };
                    }
                }
            }
            return staffDetails;
        }
        public async Task<List<PHOMembership>> GetContactPracticePHOMembership()
        {
            var phoMembership = new List<PHOMembership>();

            using (SqlConnection sqlConn = new SqlConnection(_connectionStrings.PHODB))
            {
                SqlCommand sqlCommand = new SqlCommand("spGetPHOMembership", sqlConn);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                await sqlConn.OpenAsync();

                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        var membership = new PHOMembership()
                        {
                            Id = Convert.ToInt32(dr["Id"].ToString()),
                            Membership = dr["Membership"].ToString()
                        };
                        phoMembership.Add(membership);
                    }
                }
            }
            return phoMembership;
        }

        public async Task<List<Specialty>> GetContactPracticeSpecialties()
        {
            var practiceSpecialties = new List<Specialty>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                SqlCommand sqlCommand = new SqlCommand("spGetSpecialtyList", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                await sqlConnection.OpenAsync();

                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        var specialty = new Specialty()
                        {
                            Id = Convert.ToInt32(dr["Id"].ToString()),
                            SpecialtyName = dr["Specialty"].ToString()
                        };
                        practiceSpecialties.Add(specialty);
                    }
                }
            }
            return practiceSpecialties;
        }

        public async Task<List<BoardMembership>> GetContactPracticeBoardMembership()
        {
            var boardMemberships = new List<BoardMembership>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                SqlCommand sqlCommand = new SqlCommand("spGetBoards", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                await sqlConnection.OpenAsync();

                using(SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    foreach(DataRow dr in dt.Rows)
                    {
                        var boardmembership = new BoardMembership()
                        {
                            Id = Convert.ToInt32(dr["Id"].ToString()),
                            BoardName = dr["BoardName"].ToString(),
                            Description = dr["Description"].ToString(),
                            Hyperlink = dr["Hyperlink"].ToString()
                        };
                        boardMemberships.Add(boardmembership);
                    }
                }

            }
            return boardMemberships;
        }
        public async Task<List<Staff>> GetContactEmailList(int userId, bool? managers, bool? admins, bool? all)
        {
            var staffEmailList = new List<Staff>();           

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                SqlCommand sqlCommand = new SqlCommand("spGetContactEmailList", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                sqlCommand.Parameters.Add("@Managers", SqlDbType.Bit).Value = managers;
                sqlCommand.Parameters.Add("@Admins", SqlDbType.Bit).Value = admins;
                sqlCommand.Parameters.Add("@All", SqlDbType.Bit).Value = all;
               
                await sqlConnection.OpenAsync();

                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        var staff = new Staff()
                        {
                            Id = Convert.ToInt32(dr["StaffID"].ToString()),
                            FirstName = dr["FirstName"].ToString(),
                            LastName = dr["LastName"].ToString(),
                            Email = dr["EmailAddress"].ToString(),
                            MyPractice = new Practice()
                            {
                                Id = Convert.ToInt32(dr["PracticeId"].ToString()),
                                Name = dr["PracticeName"].ToString() 
                            },
                            Position = new Position()
                            {
                                Id = Convert.ToInt32(dr["StaffPositionId"].ToString()),
                                Name = dr["StaffPosition"].ToString()
                            },
                            EmailFilters = dr["EmailFilters"].ToString()                           
                        };
                        staffEmailList.Add(staff);
                    }
                }

            }
            return staffEmailList;
        }
    }   
}