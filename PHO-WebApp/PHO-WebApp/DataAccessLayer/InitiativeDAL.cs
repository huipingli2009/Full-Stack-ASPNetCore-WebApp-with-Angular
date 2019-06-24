using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using PHO_WebApp.Models;

namespace PHO_WebApp.DataAccessLayer
{
    public class InitiativeDAL
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public List<Initiative> getAllInitiatives()
        {
            SqlCommand com = new SqlCommand("spGetAllInitiatives", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            //Cohorts = ds.Tables[0].
            da.Fill(ds);

            List<Initiative> InitiativeRecords = new List<Initiative>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Initiative c = new Initiative();
                c.Name = ds.Tables[0].Rows[i]["Name"].ToString();
                c.ShortName = ds.Tables[0].Rows[i]["ShortName"].ToString();
                c.Description = ds.Tables[0].Rows[i]["Description"].ToString();

                if (ds.Tables[0].Rows[i]["ModifiedDate"].ToString() != "")
                {
                    c.ModifiedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["ModifiedDate"].ToString());
                }

                c.Owner = ds.Tables[0].Rows[i]["Owner"].ToString();
                c.StatusDesc = ds.Tables[0].Rows[i]["Status"].ToString();
                InitiativeRecords.Add(c);
            }

            return InitiativeRecords;
        }

    }
}