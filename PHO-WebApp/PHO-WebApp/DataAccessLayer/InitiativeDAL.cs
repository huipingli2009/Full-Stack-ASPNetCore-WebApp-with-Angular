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

            //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //{
            //    Initiative c = new Initiative();

            //    c.id = int.Parse(ds.Tables[0].Rows[i]["Id"].ToString());
            //    c.Name = ds.Tables[0].Rows[i]["Name"].ToString();
            //    c.ShortName = ds.Tables[0].Rows[i]["ShortName"].ToString();
            //    c.Description = ds.Tables[0].Rows[i]["Description"].ToString();
            //    //c.Status = int.Parse(ds.Tables[0].Rows[i]["Status"].ToString());c.

            //    if (ds.Tables[0].Rows[i]["ModifiedDate"].ToString() != "")
            //    {
            //        c.ModifiedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["ModifiedDate"].ToString());
            //    }

            //    c.Owner = ds.Tables[0].Rows[i]["Owner"].ToString();
            //    c.StatusDesc = ds.Tables[0].Rows[i]["Status"].ToString();
            //    InitiativeRecords.Add(c);
            //}

            //List<Initiative> Initiatives = new List<Initiative>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Initiative c = CreateInitiativeModel(ds.Tables[0].Rows[i]);
                InitiativeRecords.Add(c);
            }

            return InitiativeRecords;
        }

        public Initiative getInitiativeRecordById(int id)
        {
            Initiative I = new Initiative();

            SqlCommand com = new SqlCommand("spGetInitiativeRecordById", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add(new SqlParameter("@id", id));

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();            
            da.Fill(ds);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                I = CreateInitiativeModel(ds.Tables[0].Rows[0]);
            }

            return I;
        }
        //public Initiative GetInitiative(int id)
        //{
        //    Initiative returnObject = null;

        //    SqlCommand com = new SqlCommand("spGetInitiativeRecordById", con);
        //    com.CommandType = CommandType.StoredProcedure;
        //    com.Parameters.Add(new SqlParameter("@id", id));

        //    SqlDataAdapter da = new SqlDataAdapter(com);
        //    DataSet ds = new DataSet();
        //    //Cohorts = ds.Tables[0].
        //    da.Fill(ds);

        //    if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //    {
        //        returnObject = CreateInitiativeModel(ds.Tables[0].Rows[0]);
        //    }

        //    return returnObject;
        //}

        public Initiative CreateInitiativeModel()
        {
            Initiative returnObject = new Initiative();

            returnObject.AllInitiativeStatuses = this.GetInitiativeStatuses();

            return returnObject;
        }       

        public List<InitiativeStatus> GetInitiativeStatuses()
        {
            List<InitiativeStatus> returnObject = null;

            SqlCommand com = new SqlCommand("spGetInitiativeStatus", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            //Cohorts = ds.Tables[0].
            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (returnObject == null)
                {
                    returnObject = new List<InitiativeStatus>();
                }
                InitiativeStatus c = CreateInitiativeStatusModel(ds.Tables[0].Rows[i]);
                returnObject.Add(c);
            }

            return returnObject;
        }

        public InitiativeStatus CreateInitiativeStatusModel(DataRow dr)
        {
            InitiativeStatus c = new InitiativeStatus();
            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                c.id = int.Parse(dr["Id"].ToString());
            }
            c.Name = dr["Name"].ToString();

            return c;
        }

        public Initiative CreateInitiativeModel(DataRow dr)
        {
            Initiative c = new Initiative();
            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                c.id = int.Parse(dr["Id"].ToString());
            }
            c.Name = dr["Name"].ToString();
            c.ShortName = dr["ShortName"].ToString();
            c.Description = dr["Description"].ToString();
            c.Details = dr["Details"].ToString();
            c.PrimaryAim = dr["PrimaryAim"].ToString();
            c.SecondaryAim = dr["SecondaryAim"].ToString();
            c.EvidenceGuidelines = dr["EvidenceGuidelines"].ToString();
            c.StatusDesc = dr["StatusDesc"].ToString();

            if (!string.IsNullOrWhiteSpace(dr["Status"].ToString()))
            {
                c.Status = int.Parse(dr["Status"].ToString());               
            }
           
            if (dr["ModifiedDate"].ToString() != "")
            {
                c.ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"].ToString());
            }
          
            c.AllInitiativeStatuses = this.GetInitiativeStatuses();
            c.Owner = dr["Owner"].ToString();

            return c;
        }

        public int UpdateInitiative(Initiative model)
        {
            int returnValue = 0;

            SqlCommand com = new SqlCommand("spUodateInitiativeRecord", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add("@Id", SqlDbType.Int);
            com.Parameters.Add("@Name", SqlDbType.VarChar);
            com.Parameters.Add("@ShortName", SqlDbType.VarChar);
            com.Parameters.Add("@Description", SqlDbType.VarChar);
            com.Parameters.Add("@Details", SqlDbType.VarChar);
            com.Parameters.Add("@PrimaryAim", SqlDbType.VarChar);
            com.Parameters.Add("@SecondaryAim", SqlDbType.VarChar);
            com.Parameters.Add("@EvidenceGuidelines", SqlDbType.VarChar);            
            com.Parameters.Add("@Owner", SqlDbType.VarChar);
            com.Parameters.Add("@Status", SqlDbType.Int);

            if (model.id > 0)
            {
                com.Parameters["@Id"].Value = model.id;
            }
            else
            {
                com.Parameters["@Id"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                com.Parameters["@Name"].Value = model.Name;
            }
            else
            {
                com.Parameters["@Name"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.ShortName))
            {
                com.Parameters["@ShortName"].Value = model.ShortName;
            }
            else
            {
                com.Parameters["@ShortName"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                com.Parameters["@Description"].Value = model.Description;
            }
            else
            {
                com.Parameters["@Description"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Details))
            {
                com.Parameters["@Details"].Value = model.Details;
            }
            else
            {
                com.Parameters["@Details"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.PrimaryAim))
            {
                com.Parameters["@PrimaryAim"].Value = model.PrimaryAim;
            }
            else
            {
                com.Parameters["@PrimaryAim"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.SecondaryAim))
            {
                com.Parameters["@SecondaryAim"].Value = model.SecondaryAim;
            }
            else
            {
                com.Parameters["@SecondaryAim"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.@EvidenceGuidelines))
            {
                com.Parameters["@EvidenceGuidelines"].Value = model.@EvidenceGuidelines;
            }
            else
            {
                com.Parameters["@EvidenceGuidelines"].Value = DBNull.Value;
            }         

            if (!string.IsNullOrWhiteSpace(model.Owner))
            {
                com.Parameters["@Owner"].Value = model.Owner;
            }
            else
            {
                com.Parameters["@Owner"].Value = DBNull.Value;
            }

            if (model.Status> 0)
            {
                com.Parameters["@Status"].Value = model.Status;
            }
            else
            {
                com.Parameters["@Status"].Value = DBNull.Value;
            }

            con.Open();
            com.ExecuteNonQuery();
            con.Close();

            return returnValue;
        }
        public void AddInitiativeModel(Initiative model)
        {
            SqlCommand com = new SqlCommand("spInsertInitiativeRecord", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add("@Name", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.Name))
            {
                com.Parameters["@Name"].Value = model.Name;
            }
            else
            {
                com.Parameters["@Name"].Value = DBNull.Value;
            }

            com.Parameters.Add("@ShortName", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.ShortName))
            {
                com.Parameters["@ShortName"].Value = model.ShortName;
            }
            else
            {
                com.Parameters["@ShortName"].Value = DBNull.Value;
            }

            com.Parameters.Add("@Description", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.ShortName))
            {
                com.Parameters["@Description"].Value = model.Description;
            }
            else
            {
                com.Parameters["@Description"].Value = DBNull.Value;
            }
            com.Parameters.Add("@Details", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.Details))
            {
                com.Parameters["@Details"].Value = model.Details;
            }
            else
            {
                com.Parameters["@Details"].Value = DBNull.Value;
            }
            com.Parameters.Add("@PrimaryAim", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.PrimaryAim))
            {
                com.Parameters["@PrimaryAim"].Value = model.PrimaryAim;
            }
            else
            {
                com.Parameters["@PrimaryAim"].Value = DBNull.Value;
            }
            com.Parameters.Add("@SecondaryAim", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.SecondaryAim))
            {
                com.Parameters["@SecondaryAim"].Value = model.SecondaryAim;
            }
            else
            {
                com.Parameters["@SecondaryAim"].Value = DBNull.Value;
            }
            com.Parameters.Add("@EvidenceGuidelines", SqlDbType.VarChar);
            if(!String.IsNullOrWhiteSpace(model.EvidenceGuidelines))
            {
                com.Parameters["@EvidenceGuidelines"].Value = model.EvidenceGuidelines;
            }
            else
            {
                com.Parameters["@EvidenceGuidelines"].Value = DBNull.Value;
            }

            com.Parameters.Add("@Owner", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.Owner))
            {
                com.Parameters["@Owner"].Value = model.Owner;
            }
            else
            {
                com.Parameters["@Owner"].Value = DBNull.Value;
            }

            com.Parameters.Add("@Status", SqlDbType.Int);
            if (model.Status > 0)
            {
                com.Parameters["@Status"].Value = model.Status;
            }
            else
            {
                com.Parameters["@Status"].Value = DBNull.Value;
            }

            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }
    }    
}