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
        public DataSet GetDataDictionaryRecordsWithSearchCriteria(string DatabaseName, string SchemaName, string ObjectName, string ObjectType, string ColumnName, string SQLColumnDescription, string IsNullable, string DataType, bool? PHIFlag, string BusinessDefiniton)
        {
            SqlCommand com = new SqlCommand("spGetDataDictionaryRecordsWithSearchCriteria", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add("@DatabaseName", SqlDbType.NVarChar);
            com.Parameters.Add("@SchemaName", SqlDbType.NVarChar);
            com.Parameters.Add("@ObjectName", SqlDbType.NVarChar);
            com.Parameters.Add("@ObjectType", SqlDbType.NVarChar);
            com.Parameters.Add("@ColumnName", SqlDbType.NVarChar);
            com.Parameters.Add("@SQLColumnDescription", SqlDbType.NVarChar);
            com.Parameters.Add("@IsNullable", SqlDbType.NVarChar);
            com.Parameters.Add("@DataType", SqlDbType.NVarChar);
            com.Parameters.Add("@PHIFlag", SqlDbType.NVarChar);
            com.Parameters.Add("@BusinessDefinition", SqlDbType.NVarChar);

            //Assign parameters
            if (!string.IsNullOrWhiteSpace(DatabaseName))
            {
                com.Parameters["@DatabaseName"].Value = DatabaseName;
            }
            if (!string.IsNullOrWhiteSpace(SchemaName))
            {
                com.Parameters["@SchemaName"].Value = SchemaName;
            }
            if (!string.IsNullOrWhiteSpace(ObjectName))
            {
                com.Parameters["@ObjectName"].Value = ObjectName;
            }
            if (!string.IsNullOrWhiteSpace(ObjectType))
            {
                com.Parameters["@ObjectType"].Value = ObjectType;
            }
            if (!string.IsNullOrWhiteSpace(ColumnName))
            {
                com.Parameters["@ColumnName"].Value = ColumnName;
            }
            if (!string.IsNullOrWhiteSpace(SQLColumnDescription))
            {
                com.Parameters["@SQLColumnDescription"].Value = SQLColumnDescription;
            }
            if (!string.IsNullOrWhiteSpace(IsNullable))
            {
                com.Parameters["@IsNullable"].Value = IsNullable;
            }
            if (!string.IsNullOrWhiteSpace(DataType))
            {
                com.Parameters["@DataType"].Value = DataType;
            }
            if (PHIFlag.HasValue)
            {
                com.Parameters["@PHIFlag"].Value = PHIFlag.Value;
            }
            if (!string.IsNullOrWhiteSpace(BusinessDefiniton))
            {
                com.Parameters["@BusinessDefinition"].Value = BusinessDefiniton;
            }


            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);
            return ds;
        }
    }
}