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
        //private IConfiguration _config;
        private readonly ConnectionStrings _connectionStrings;

        public ContactDAL(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }

        public async Task<List<Contact>> GetContacts(int userId, bool? qpl, string specialty, string membership, string board, string namesearch)
        {
            DataTable dataTable = new DataTable();
            List<Contact> contactList = new List<Contact>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("[spGetContactList]", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@QPL", SqlDbType.Bit).Value = qpl;
                    sqlCommand.Parameters.Add("@Specialty", SqlDbType.VarChar, 50).Value = specialty;
                    sqlCommand.Parameters.Add("@Membership", SqlDbType.VarChar, 50).Value = membership;
                    sqlCommand.Parameters.Add("@Board", SqlDbType.VarChar, 50).Value = board;
                    sqlCommand.Parameters.Add("@NameSearch", SqlDbType.VarChar, 100).Value = namesearch;

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
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
    }
}