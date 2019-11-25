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
    public class MeasureDAL
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public List<Measure> getAllMeasures(UserDetails ud)
        {          
            //SqlCommand com = new SqlCommand("spGetAllQIMeasures", con);
            SqlCommand com = new SqlCommand("spQI_Summary", con);
            com.CommandType = CommandType.StoredProcedure;

            //newly added
            com.Parameters.Add(new SqlParameter("@userID", ud.StaffId));

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            da.Fill(ds);           

            List<Measure> MeasureRecords = new List<Measure>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Measure c = CreateMeasureModel(ds.Tables[0].Rows[i]);
                MeasureRecords.Add(c);
            }

            return MeasureRecords;
        }
        public Measure CreateMeasureModel(DataRow dr)
        {
            Measure c = new Measure();

            if (dr["MeasureId"] != null && !string.IsNullOrWhiteSpace(dr["MeasureId"].ToString()))
            {
                c.MeasureId = int.Parse(dr["MeasureId"].ToString());
            }
            c.MeasureName = dr["MeasureName"].ToString();
            c.MeasureDesc = dr["MeasureDesc"].ToString();

            //c.MeasureFrequency = dr["MeasureFrequency"].ToString();
            c.Frequency = dr["Frequency"].ToString();
            //c.StatusDesc = dr["StatusDesc"].ToString();
            c.StatusDesc = dr["StatusDesc"].ToString();
            c.Numerator = dr["Numerator"].ToString();
            c.Denominator = dr["Denominator"].ToString();
            //c.SQL = dr["SQL"].ToString();
            c.SQL = "";
            c.Factor = dr["Factor"].ToString();


            //if (!string.IsNullOrWhiteSpace(dr["Status"].ToString()))
            //{
            //    c.Status = int.Parse(dr["Status"].ToString());
            //}
            if (!string.IsNullOrWhiteSpace(dr["StatusDesc"].ToString()))
            {
                c.StatusDesc = dr["StatusDesc"].ToString();
            }

            if (dr["EffectiveDate"].ToString() != "")
            {
                c.EffectiveDate = Convert.ToDateTime(dr["EffectiveDate"].ToString());
            }
            else
            {
                c.EffectiveDate = null;
            }


            if (dr["ModifiedDate"].ToString() != "")
            {
                c.ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"].ToString());
            }

            if (dr["LastMeasureDate"].ToString() != "")
            {
                c.LastMeasureDate = Convert.ToDateTime(dr["LastMeasureDate"].ToString());
            }


            if (dr["MeasureValue"] != null && !string.IsNullOrWhiteSpace(dr["MeasureValue"].ToString()))
            {
                c.MeasureValue = string.Format("{0:#.##}", Double.Parse(dr["MeasureValue"].ToString()));
            }

            if (dr["LastNetworkValue"] != null && !string.IsNullOrWhiteSpace(dr["LastNetworkValue"].ToString()))
            {
                c.LastNetworkValue = string.Format("{0:#.##}", Double.Parse(dr["LastNetworkValue"].ToString()));
            }

            c.Owner = dr["Owner"].ToString();

            c.AllInitiativeStatuses = this.GetInitiativeStatuses();

            return c;
        }
        public Measure CreateMeasureModel(object Id,
            object Name,
            object Description,
            object Frequency,
            object Status,
            object StatusDescription,
            object Numerator,
            object Denominator,
            object NumeratorFactValue,
            object DenominatorFactValue,
            object Factor,
            object EffectiveDate,
            object ModifiedDate,
            object SQL,
            object LastMeasureDate,
            object LastMeasureValue,
            object LastNetworkValue,
            object Owner)
        {
            Measure c = CreateMeasureModel();
            if (Id != null)
            {
                c.MeasureId = SharedLogic.ParseNumeric(Id.ToString());
            }
            if (Name != null)
            {
                c.MeasureName = Name.ToString();
            }
            if (Description != null)
            {
                c.MeasureDesc = Description.ToString();
            }
            if (Frequency != null)
            {
                c.Frequency = Frequency.ToString();
            }
            if (StatusDescription != null)
            {
                c.StatusDesc = StatusDescription.ToString();
            }
            if (Numerator != null)
            {
                c.Numerator = Numerator.ToString();
            }
            if (Denominator != null)
            {
                c.Denominator = Denominator.ToString();
            }
            if (NumeratorFactValue != null)
            {
                c.NumeratorFactValue = NumeratorFactValue.ToString();
            }
            if (DenominatorFactValue != null)
            {
                c.DenominatorFactValue = DenominatorFactValue.ToString();
            }
            if (Factor != null)
            {
                c.Factor = Factor.ToString();
            }
            if (LastMeasureDate != null)
            {
                c.LastMeasureDate = SharedLogic.ParseDateTimeNullable(LastMeasureDate.ToString());
            }
            if (LastMeasureValue != null)
            {
                c.MeasureValue = Decimal.Round(SharedLogic.ParseDecimal(LastMeasureValue.ToString()), 3).ToString();
            }
            if (LastNetworkValue != null)
            {
                c.LastNetworkValue = Decimal.Round(SharedLogic.ParseDecimal(LastNetworkValue.ToString()), 3).ToString();
            }
            if (SQL != null)
            {
                c.SQL = SQL.ToString();
            }
            if (Status != null)
            {
                c.Status = SharedLogic.ParseNumeric(Status.ToString());
            }
            if (EffectiveDate != null)
            {
                c.EffectiveDate = SharedLogic.ParseDateTimeNullable(EffectiveDate.ToString());
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


        public Chart CreateChartModel(object Id,
            object Type,
            object Description,
            object URL,
            object NetworkURL)
        {
            Chart c = new Chart();
            if (Id != null)
            {
                c.Id = SharedLogic.ParseNumeric(Id.ToString());
            }
            if (Type != null)
            {
                c.Type = Type.ToString();
            }
            if (Description != null)
            {
                c.Description = Description.ToString();
            }
            if (URL != null)
            {
                c.URL = URL.ToString();
            }
            if (NetworkURL != null)
            {
                c.NetworkURL = NetworkURL.ToString();
            }
            return c;
        }

        public Measure CreateMeasureModelById(DataRow dr)
        {
            return CreateMeasureModel(dr["MeasureId"]
                , dr["MeasureName"]
                , dr["MeasureDesc"]
                , dr["Frequency"]
                , dr["Status"]
                , dr["StatusDesc"]
                , dr["Numerator"]
                , dr["Denominator"]
                , null
                , null
                , dr["Factor"]
                , dr["EffectiveDate"]
                , dr["ModifiedDate"]
                , dr["SQL"]
                , dr["LastMeasureDate"]
                , dr["MeasureValue"]
                , dr["LastNetworkValue"]
                , dr["Owner"]);
                       
        }
        public Measure getQIMeasureById(int id)
        {
            Measure m = new Measure();

            SqlCommand com = new SqlCommand("spGetQIMeasureById", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add(new SqlParameter("@MeasureId", id));

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);

            if(ds.Tables[0].Rows.Count > 0)
            {
                m = CreateMeasureModelById(ds.Tables[0].Rows[0]);
            }

            return m;
        }

        public Measure CreateMeasureModel()
        {
            Measure m = new Measure();
            m.AllInitiativeStatuses = this.GetInitiativeStatuses();
            return m;
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

        public int UpdateQIMeasure(Measure model)
        {
            int returnValue = 0;

            SqlCommand com = new SqlCommand("spUpdateMeasureRecord", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add("@MeasureId", SqlDbType.Int);
            com.Parameters.Add("@MeasureName", SqlDbType.VarChar);
            com.Parameters.Add("@MeasureDesc", SqlDbType.VarChar);
            com.Parameters.Add("@Frequency", SqlDbType.VarChar);
            com.Parameters.Add("@Numerator", SqlDbType.VarChar);
            com.Parameters.Add("@Denominator", SqlDbType.VarChar);
            com.Parameters.Add("@Factor", SqlDbType.VarChar);
            com.Parameters.Add("@EffectiveDate", SqlDbType.DateTime);
            com.Parameters.Add("@Owner", SqlDbType.VarChar);
            com.Parameters.Add("@SQL", SqlDbType.VarChar);
            com.Parameters.Add("@Status", SqlDbType.Int);

            if (model.MeasureId > 0)
            {
                com.Parameters["@MeasureId"].Value = model.MeasureId;
            }
            else
            {
                com.Parameters["@MeasureId"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.MeasureName))
            {
                com.Parameters["@MeasureName"].Value = model.MeasureName;
            }
            else
            {
                com.Parameters["@MeasureName"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.MeasureDesc))
            {
                com.Parameters["@MeasureDesc"].Value = model.MeasureDesc;
            }
            else
            {
                com.Parameters["@MeasureDesc"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Frequency))
            {
                com.Parameters["@Frequency"].Value = model.Frequency;
            }
            else
            {
                com.Parameters["@Frequency"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Numerator))
            {
                com.Parameters["@Numerator"].Value = model.Numerator;
            }
            else
            {
                com.Parameters["@Numerator"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Denominator))
            {
                com.Parameters["@Denominator"].Value = model.Denominator;
            }
            else
            {
                com.Parameters["@Denominator"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Factor))
            {
                com.Parameters["@Factor"].Value = model.Factor;
            }
            else
            {
                com.Parameters["@Factor"].Value = DBNull.Value;
            }

            if ((model.EffectiveDate) != null)
            {
                com.Parameters["@EffectiveDate"].Value = model.EffectiveDate;
            }
            else
            {
                com.Parameters["@EffectiveDate"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Owner))
            {
                com.Parameters["@Owner"].Value = model.Owner;
            }
            else
            {
                com.Parameters["@Owner"].Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.SQL))
            {
                com.Parameters["@SQL"].Value = model.SQL;
            }
            else
            {
                com.Parameters["@SQL"].Value = DBNull.Value;
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

        public void AddQIMeasureModel(Measure model)
        {
            SqlCommand com = new SqlCommand("spInsertQIMeasureRecord", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add("@MeasureName", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.MeasureName))
            {
                com.Parameters["@MeasureName"].Value = model.MeasureName;
            }
            else
            {
                com.Parameters["@MeasureName"].Value = DBNull.Value;
            }

            com.Parameters.Add("@MeasureDesc", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.MeasureDesc))
            {
                com.Parameters["@MeasureDesc"].Value = model.MeasureDesc;
            }
            else
            {
                com.Parameters["@MeasureDesc"].Value = DBNull.Value;
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

            com.Parameters.Add("@Frequency", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.Frequency))
            {
                com.Parameters["@Frequency"].Value = model.Frequency;
            }
            else
            {
                com.Parameters["@Frequency"].Value = DBNull.Value;
            }

            com.Parameters.Add("@Numerator", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.Numerator))
            {
                com.Parameters["@Numerator"].Value = model.Numerator;
            }
            else
            {
                com.Parameters["@Numerator"].Value = DBNull.Value;
            }

            com.Parameters.Add("@Denominator", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.Denominator))
            {
                com.Parameters["@Denominator"].Value = model.Denominator;
            }
            else
            {
                com.Parameters["@Denominator"].Value = DBNull.Value;
            }

            com.Parameters.Add("@Factor", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.Factor))
            {
                com.Parameters["@Factor"].Value = model.Factor;
            }
            else
            {
                com.Parameters["@Factor"].Value = DBNull.Value;
            }

            com.Parameters.Add("@EffectiveDate", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.EffectiveDate.ToString()))
            {
                com.Parameters["@EffectiveDate"].Value = model.EffectiveDate;
            }
            else
            {
                com.Parameters["@EffectiveDate"].Value = DBNull.Value;
            }

            com.Parameters.Add("@SQL", SqlDbType.VarChar);
            if (!String.IsNullOrWhiteSpace(model.SQL))
            {
                com.Parameters["@SQL"].Value = model.SQL;
            }
            else
            {
                com.Parameters["@SQL"].Value = DBNull.Value;
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