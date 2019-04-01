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
        public void UpdateDictionaryRecords(PHO_WebApp.Models.DataDictionary dd)
        {
            SqlCommand com = new SqlCommand("spUpdateDataDictionaryRecords", con);
            com.CommandType = CommandType.StoredProcedure;

            //check if PHIFlag is null
            //IsNullOrEmpty(dd.PHIFlag) ?= null ? PHIFlag : DBNull.Value;

            //com.Parameters.AddWithValue("@DatabaseName", dd.DatabaseName);
            //com.Parameters.AddWithValue("@SchemaName", dd.SchemaName);
            //com.Parameters.AddWithValue("@ObjectName", dd.ObjectName);
            //com.Parameters.AddWithValue("@ColumnName", dd.ColumnName);
            //com.Parameters.AddWithValue("@SQLColumnDesc", dd.SQLColumnDesc);
            //com.Parameters.AddWithValue("@IsNullable", dd.IsNullable);
            //com.Parameters.AddWithValue("@DataType", dd.DataType);
            com.Parameters.AddWithValue("@DataDictionaryId", dd.skDataDictionary);

          
            if ((dd.PHIFlag == null)||(dd.PHIFlag == "-- please select --"))
            {
                com.Parameters.AddWithValue("@PHIFlag", DBNull.Value);
            }
            else
            {
                com.Parameters.AddWithValue("@PHIFlag", dd.PHIFlag);
            }            

            if (dd.BusinessDefinition == null)
            {
                com.Parameters.AddWithValue("@BusinessDefinition", DBNull.Value);
            }
            else
            {
                com.Parameters.AddWithValue("@BusinessDefinition", dd.BusinessDefinition);
            }            

            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }
        public DataSet RefreshDataDictionary()     //call stored procedure to update metadata table 
        {
            SqlCommand com = new SqlCommand("spUpdateDataDictionary", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);
            return ds;
        }
    }
}