using Microsoft.Extensions.Logging;
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
    public class WorkbooksDAL : IWorkbooks
    {
        private readonly ConnectionStrings _connectionStrings;
        private readonly ILogger<WorkbooksDAL> _logger;

        public WorkbooksDAL(IOptions<ConnectionStrings> options, ILogger<WorkbooksDAL> logger)
        {
            _connectionStrings = options.Value;
            _logger = logger;
        }
        public async Task<List<WorkbooksPatient>> ListPatients(int userId, DateTime reportingDate, string NameSearch)
        {
            DataTable dataTable = new DataTable();
            List<WorkbooksPatient> workbookspatients = new List<WorkbooksPatient>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetPHQ9Workbooks_Patients", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@ReportingDate", SqlDbType.DateTime).Value = reportingDate;
                    sqlCommand.Parameters.Add("@NameSearch", SqlDbType.Int).Value = NameSearch;

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);

                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var workbookspt = new WorkbooksPatient()
                            {
                                FormResponseId = int.Parse(dr["FormResponseId"].ToString()),
                                PatientId = int.Parse(dr["PatientId"].ToString()),
                                Patient = dr["Patient"].ToString(),
                                DOB = (dr["DOB"] == DBNull.Value? (DateTime?)null: (DateTime.Parse(dr["DOB"].ToString()))),
                                Phone = dr["Phone"].ToString(),
                                Provider = dr["Provider"].ToString(),
                                DateOfService = (dr["DateOfService"] == DBNull.Value? (DateTime?)null:(DateTime.Parse(dr["DateOfService"].ToString()))),
                                PHQ9_Score = dr["PHQ9_Score"].ToString(),
                                ActionFollowUp = dr["ActionFollowUp"].ToString()
                            };

                            workbookspatients.Add(workbookspt);
                        }
                    }
                }               
            }
            return workbookspatients;
        }
    }
}
