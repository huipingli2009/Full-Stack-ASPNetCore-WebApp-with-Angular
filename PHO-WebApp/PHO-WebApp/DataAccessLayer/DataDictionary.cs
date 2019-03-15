using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PHO_WebApp.Models;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace PHO_WebApp.DataAccessLayer
{
    public class DataDictionary
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con_DW"].ConnectionString);
        public DataSet GetDataDictionaryRecords()
        {
            SqlCommand com = new SqlCommand("spGetDataDictionaryRecords", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);
            return ds;
        }
    }
}