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

        public List<Cohort> getAllActiveCohortRecords()
        {
            SqlCommand com = new SqlCommand("spGetCohortRecords", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            //Cohorts = ds.Tables[0].
            da.Fill(ds);

            List<Cohort> Cohorts = new List<Cohort>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Cohort c = CreateCohortModel(ds.Tables[0].Rows[i]);
                Cohorts.Add(c);
            }

            return Cohorts;
        }

        public Cohort GetCohort(int id)
        {
            Cohort returnObject = null;

            SqlCommand com = new SqlCommand("spGetCohortRecordById", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add(new SqlParameter("@Id", id));

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            //Cohorts = ds.Tables[0].
            da.Fill(ds);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                returnObject = CreateCohortModel(ds.Tables[0].Rows[0]);
            }

            return returnObject;
        }

        public List<CohortStatus> GetCohortStatuses()
        {
            List<CohortStatus> returnObject = null;

            SqlCommand com = new SqlCommand("spGetCohortStatuses", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            //Cohorts = ds.Tables[0].
            da.Fill(ds);
            
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (returnObject == null)
                {
                    returnObject = new List<CohortStatus>();
                }
                CohortStatus c = CreateCohortStatusModel(ds.Tables[0].Rows[i]);
                returnObject.Add(c);
            }

            return returnObject;
        }
        
        public Cohort CreateCohortModel()
        {
            Cohort returnObject = new Cohort();

            returnObject.AllStatuses = this.GetCohortStatuses();

            return returnObject;
        }

        public Cohort CreateCohortModel(object Id,
            object Name,
            object ShortName,
            object Description,
            object Details,
            object Calculations,
            object Limitations,
            object Exceptions,
            object DataSources,
            object Lookback,
            object StatusName,
            object SQL,
            object Status,
            object ModifiedDate, 
            object Owner)
        {
            Cohort c = this.CreateCohortModel();
            if (Id != null)
            {
                c.id = SharedLogic.ParseNumeric(Id.ToString());
            }
            if (Name != null)
            {
                c.Name = Name.ToString();
            }
            if (ShortName != null)
            {
                c.ShortName = ShortName.ToString();
            }
            if (Description != null)
            {
                c.Description = Description.ToString();
            }
            if (Details != null)
            {
                c.Details = Details.ToString();
            }
            if (Calculations != null)
            {
                c.Calculations = Calculations.ToString();
            }
            if (Limitations != null)
            {
                c.Limitations = Limitations.ToString();
            }
            if (Exceptions != null)
            {
                c.Exceptions = Exceptions.ToString();
            }
            if (DataSources != null)
            {
                c.DataSources = DataSources.ToString();
            }
            if (Lookback != null)
            {
                c.Lookback = Lookback.ToString();
            }
            if (SQL != null)
            {
                c.SQL = SQL.ToString();
            }
            if (Status != null)
            {
                c.Status = SharedLogic.ParseNumeric(Status.ToString());
            }
            if (ModifiedDate != null)
            {
                c.ModifiedDate = SharedLogic.ParseDateTimeNullable(ModifiedDate.ToString());
            }
            if (Owner != null)
            {
                c.Owner = Owner.ToString();
            }
            
            return c;
        }

        public Cohort CreateCohortModel(DataRow dr)
        {
            Cohort c = CreateCohortModel((string)dr["Id"], dr["Name"], dr["ShortName"], dr["Description"], dr["Details"], dr["Calculations"], dr["Limitations"], dr["Exceptions"], dr["DataSources"], dr["Lookback"], dr["StatusName"], dr["SQL"], dr["Status"], dr["ModifiedDate"], dr["Owner"]);
            //if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            //{
            //    c.id = int.Parse(dr["Id"].ToString());
            //}
            //c.Name = dr["Name"].ToString();
            //c.ShortName = dr["ShortName"].ToString();
            //c.Description = dr["Description"].ToString();
            //c.Details = dr["Details"].ToString();
            //c.Calculations = dr["Calculations"].ToString();
            //c.Limitations = dr["Limitations"].ToString();
            //c.Exceptions = dr["Exceptions"].ToString();
            //c.DataSources = dr["DataSources"].ToString();
            //c.Lookback = dr["Lookback"].ToString();
            //c.StatusName = dr["StatusName"].ToString();
            //c.SQL = dr["SQL"].ToString();
            //if (!string.IsNullOrWhiteSpace(dr["Status"].ToString()))
            //{
            //    c.Status = int.Parse(dr["Status"].ToString());
            //}
            //if (dr["ModifiedDate"].ToString() != "")
            //{
            //    c.ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"].ToString());
            //}
            //c.Owner = dr["Owner"].ToString();

            return c;
        }

        public CohortStatus CreateCohortStatusModel(DataRow dr)
        {
            CohortStatus c = new CohortStatus();
            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                c.Id = int.Parse(dr["Id"].ToString());
            }
            c.Name = dr["Name"].ToString();

            return c;
        }

        public int UpdateCohort(Cohort model)
        {
            int returnValue = 0;

            SqlCommand com = new SqlCommand("spUpdateCohortRecord", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add("@CohortId", SqlDbType.VarChar);
            com.Parameters.Add("@Name", SqlDbType.VarChar);
            com.Parameters.Add("@ShortName", SqlDbType.VarChar);
            com.Parameters.Add("@Description", SqlDbType.VarChar);
            com.Parameters.Add("@Details", SqlDbType.VarChar);
            com.Parameters.Add("@Calculations", SqlDbType.VarChar);
            com.Parameters.Add("@Limitations", SqlDbType.VarChar);
            com.Parameters.Add("@Exceptions", SqlDbType.VarChar);
            com.Parameters.Add("@DataSources", SqlDbType.VarChar);
            com.Parameters.Add("@Lookback", SqlDbType.VarChar);
            com.Parameters.Add("@Owner", SqlDbType.VarChar);
            com.Parameters.Add("@Status", SqlDbType.Int);

            if (model.id > 0)
            {
                com.Parameters["@CohortId"].Value = model.id;
            }
            else
            {
                com.Parameters["@CohortId"].Value = DBNull.Value;
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

            if (!string.IsNullOrWhiteSpace(model.Limitations))
            {
                com.Parameters["@Limitations"].Value = model.Limitations;
            }
            else
            {
                com.Parameters["@Limitations"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Calculations))
            {
                com.Parameters["@Calculations"].Value = model.Calculations;
            }
            else
            {
                com.Parameters["@Calculations"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Exceptions))
            {
                com.Parameters["@Exceptions"].Value = model.Exceptions;
            }
            else
            {
                com.Parameters["@Exceptions"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.DataSources))
            {
                com.Parameters["@DataSources"].Value = model.DataSources;
            }
            else
            {
                com.Parameters["@DataSources"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Lookback))
            {
                com.Parameters["@Lookback"].Value = model.Lookback;
            }
            else
            {
                com.Parameters["@Lookback"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Owner))
            {
                com.Parameters["@Owner"].Value = model.Owner;
            }
            else
            {
                com.Parameters["@Owner"].Value = DBNull.Value;
            }

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

            return returnValue;
        }
        public int InsertCohort(Cohort model)
        {
            int returnValue = 0;

            SqlCommand com = new SqlCommand("spInsertCohortRecord", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add("@Name", SqlDbType.VarChar);
            com.Parameters.Add("@ShortName", SqlDbType.VarChar);
            com.Parameters.Add("@Description", SqlDbType.VarChar);
            com.Parameters.Add("@Details", SqlDbType.VarChar);
            com.Parameters.Add("@Calculations", SqlDbType.VarChar);
            com.Parameters.Add("@Limitations", SqlDbType.VarChar);
            com.Parameters.Add("@Exceptions", SqlDbType.VarChar);
            com.Parameters.Add("@DataSources", SqlDbType.VarChar);
            com.Parameters.Add("@Lookback", SqlDbType.VarChar);
            com.Parameters.Add("@Owner", SqlDbType.VarChar);
            com.Parameters.Add("@Status", SqlDbType.Int);
            
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

            if (!string.IsNullOrWhiteSpace(model.Limitations))
            {
                com.Parameters["@Limitations"].Value = model.Limitations;
            }
            else
            {
                com.Parameters["@Limitations"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Calculations))
            {
                com.Parameters["@Calculations"].Value = model.Calculations;
            }
            else
            {
                com.Parameters["@Calculations"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Exceptions))
            {
                com.Parameters["@Exceptions"].Value = model.Exceptions;
            }
            else
            {
                com.Parameters["@Exceptions"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.DataSources))
            {
                com.Parameters["@DataSources"].Value = model.DataSources;
            }
            else
            {
                com.Parameters["@DataSources"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Lookback))
            {
                com.Parameters["@Lookback"].Value = model.Lookback;
            }
            else
            {
                com.Parameters["@Lookback"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Owner))
            {
                com.Parameters["@Owner"].Value = model.Owner;
            }
            else
            {
                com.Parameters["@Owner"].Value = DBNull.Value;
            }

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

            return returnValue;
        }


    }
}