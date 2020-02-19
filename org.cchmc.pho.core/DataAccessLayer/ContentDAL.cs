﻿using org.cchmc.pho.core.DataModels;
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
    public class ContentDAL:IContent
    {
        //private IConfiguration _config;
        private readonly ConnectionStrings _connectionStrings;
        public ContentDAL(IOptions<ConnectionStrings> options, ILogger<AlertDAL> logger)
        {
            _connectionStrings = options.Value;
        }

        public async Task<List<Content>> ListActiveContents()
        {
            DataTable dataTable = new DataTable();
            List<Content> contents = new List<Content>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetDashboardContent", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    await sqlConnection.OpenAsync();

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        await Task.Run(() => da.Fill(dataTable));


                        contents = (from DataRow dr in dataTable.Rows
                                  select new Content()
                                  {
                                      Header = dr["Header"].ToString(),
                                      PlacementOrder = Convert.ToInt32(dr["PlacementOrder"]),
                                      Body = dr["Body"].ToString(),
                                      Hyperlink = dr["Hyperlink"].ToString(),
                                      ImageHyperlink = dr["ImageHyperlink"].ToString(),
                                      ContentPlacement = dr["ContentPlacement"].ToString()
                                  }

                          ).ToList();
                    }
                   
                }

                return contents;
            }
        }
    }
}
