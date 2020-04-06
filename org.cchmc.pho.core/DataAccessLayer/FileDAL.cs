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
    public class FileDAL
    {
        private readonly ConnectionStrings _connectionStrings;
        public FileDAL(IOptions<ConnectionStrings> options, ILogger<FileDAL> logger)
        {
            _connectionStrings = options.Value;
        }


        public async Task<List<File>> ListFiles(int userId, int resourceTypeId, int initiativeId, string tag, bool watch, string name)
        {
            DataTable dataTable = new DataTable();
            List<File> file = new List<File>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetFiles", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@ResourceTypeId", SqlDbType.Int).Value = resourceTypeId;
                    sqlCommand.Parameters.Add("@InitiativeId", SqlDbType.Int).Value = initiativeId;
                    sqlCommand.Parameters.Add("@Tag", SqlDbType.VarChar).Value = tag;
                    sqlCommand.Parameters.Add("@Watch", SqlDbType.Bit).Value = watch;
                    sqlCommand.Parameters.Add("@NameSearch", SqlDbType.VarChar).Value = name;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var record = new File()
                            {
                                Id = (dr["StaffID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StaffID"].ToString()))


                            };
                            //if (dr["CredentialId"] != DBNull.Value && int.TryParse(dr["CredentialId"].ToString(), out int intCredential))
                            //{
                            //    Credential cred = new Credential();
                            //    cred.Id = intCredential;
                            //    cred.Name = dr["CredentialName"].ToString();
                            //    record.Credentials = cred;
                            //}
                            //if (dr["PositionId"] != DBNull.Value && int.TryParse(dr["PositionId"].ToString(), out int intPosition))
                            //{
                            //    Position pos = new Position();
                            //    pos.Id = intPosition;
                            //    pos.Name = dr["Position"].ToString();
                            //    pos.PositionType = dr["PositionType"].ToString();
                            //    record.Position = pos;
                            //}
                            file.Add(record);
                        }
                    }
                    return file;
                }
            }
        }


    }
}
