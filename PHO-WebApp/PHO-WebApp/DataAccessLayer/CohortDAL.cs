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
    public class CohortDAL
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        //public DataSet getAllActiveCohortRecords()
        //{
        //    SqlCommand com = new SqlCommand("spGetCohortRecords",con);
        //    com.CommandType = CommandType.StoredProcedure;

        //    SqlDataAdapter da = new SqlDataAdapter(com);
        //    DataSet ds = new DataSet();
        //    //List<Cohort> Cohorts = new List<Cohort>();
        //    //Cohorts = ds.Tables[0].
        //    da.Fill(ds);
        //    return ds;
        //}

        public List<Cohort> getAllActiveCohortRecords()
        {
            SqlCommand com = new SqlCommand("spGetCohortRecords", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();            
            //Cohorts = ds.Tables[0].
            da.Fill(ds);
            
            List<Cohort> Cohorts = new List<Cohort>();

            for (int i= 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Cohort c = new Cohort();
                c.Name = ds.Tables[0].Rows[i]["Name"].ToString();
                c.ShortName = ds.Tables[0].Rows[i]["ShortName"].ToString();
                c.Description = ds.Tables[0].Rows[i]["Description"].ToString();

                if (ds.Tables[0].Rows[i]["ModifiedDate"].ToString() != "" )
                {
                    c.ModifiedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["ModifiedDate"].ToString());
                }
               
                c.Owner = ds.Tables[0].Rows[i]["Owner"].ToString();
                Cohorts.Add(c);
            }
            return Cohorts;
        }

    }
}