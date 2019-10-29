using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using PHO_WebApp.Models;
using System.Security.Cryptography;
using PHO_WebApp.ViewModel;

namespace PHO_WebApp.DataAccessLayer
{
    public class StaffDAL
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public List<Staff> getPracticeStaffs(int practiceId)
        {
            List<Staff> practiceStaffList = new List<Staff>();

            SqlCommand com = new SqlCommand("spGetPracticeStaffs", con);
            com.CommandType = CommandType.StoredProcedure;


            SqlParameter parameterPracticeId = new SqlParameter("@PracticeId", SqlDbType.Int);
            parameterPracticeId.Value = practiceId;
            com.Parameters.Add(parameterPracticeId);

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Staff staff = CreateStaffModel(ds.Tables[0].Rows[i]);
                practiceStaffList.Add(staff);
            }

            return practiceStaffList;
        }

        public List<Staff> getPracticeProviders(int practiceId)
        {
            List<Staff> practiceStaffList = getPracticeStaffs(practiceId);

            practiceStaffList.Where(c => c.DeletedFlag == false)
                            .Where(c => c.StaffTypeId == (int)StaffTypeEnum.Provider)
                            .Distinct().ToList();

            return practiceStaffList;
        }

        public Staff getPracticeStaff(int stfId)
        {
            Staff practiceStaff = new Staff();

            SqlCommand com = new SqlCommand("spGetPracticeStaff", con);
            com.CommandType = CommandType.StoredProcedure;

            //Session["UserDetails"]
            int practiceId = 7;

            //need later
            string tx = HttpContext.Current.Session["UserDetails"].ToString();

            SqlParameter parameterStaffId = new SqlParameter("@StaffId", SqlDbType.Int);
            parameterStaffId.Value = stfId;
            com.Parameters.Add(parameterStaffId);

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                practiceStaff = CreateStaffModel(ds.Tables[0].Rows[i]);
                //practiceStaff.Add(staff);
            }

            return practiceStaff;
        }
        public Staff CreateStaffModel(DataRow dr)
        {
            Staff s = new Staff();

            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                s.Id = int.Parse(dr["Id"].ToString());
            }

            if (dr["StaffTypeId"] != null && !string.IsNullOrWhiteSpace(dr["StaffTypeId"].ToString()))
            {
                s.StaffTypeId = int.Parse(dr["StaffTypeId"].ToString());
            }

            s.FirstName = dr["FirstName"].ToString();
            s.LastName = dr["LastName"].ToString();
            
            s.EmailAddress = dr["EmailAddress"].ToString();

            s.AddressLine1 = dr["AddressLine1"].ToString();
            s.AddressLine2 = dr["AddressLine2"].ToString();
            s.City = dr["City"].ToString();
            s.State = dr["State"].ToString();

            s.RegistryAccess = dr["UserName"].ToString();        

            s.LeadPhysician = dr["Lead_Physician"].ToString();
            s.QI_Team = dr["QI_Team"].ToString();
            s.PracticeManager = dr["PracticeManager"].ToString();
            s.InterventionContact = dr["Intervention_Contact"].ToString();
            //s.StateId = int.Parse(dr["StateId"].ToString());

            s.Phone = dr["Phone"].ToString();

            s.StaffPosition = dr["PositionDesc"].ToString();
            s.CredName = dr["CredName"].ToString();
           
            if(!string.IsNullOrEmpty(dr["NPI"].ToString()))
            { 
                s.NPI = int.Parse(dr["NPI"].ToString());
            }
            else
            {
                s.NPI = null;
            }

            //s.PracticelocationId = int.Parse(dr["PracticelocationId"].ToString());
            //s.ActiveFlag = dr["ActiveFlag"].ToString();
            //s.CreatedOnDate = DateTime.Parse(dr["CreatedOnDate"].ToString());           

            //if (!string.IsNullOrWhiteSpace(dr["Status"].ToString()))
            //{
            //    c.Status = int.Parse(dr["Status"].ToString());
            //}
            //if (!string.IsNullOrWhiteSpace(dr["StatusDesc"].ToString()))
            //{
            //    c.StatusDesc = dr["StatusDesc"].ToString();
            //}

            //if (dr["EffectiveDate"].ToString() != "")
            //{
            //    c.EffectiveDate = Convert.ToDateTime(dr["EffectiveDate"].ToString());
            //}
            //else
            //{
            //    c.EffectiveDate = null;
            //}


            //if (dr["ModifiedDate"].ToString() != "")
            //{
            //    c.ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"].ToString());
            //}

            //if (dr["LastMeasureDate"].ToString() != "")
            //{
            //    c.LastMeasureDate = Convert.ToDateTime(dr["LastMeasureDate"].ToString());
            //}

            //c.MeasureValue = string.Format("{0:#.##}", Double.Parse(dr["MeasureValue"].ToString()));
            //c.LastNetworkValue = string.Format("{0:#.##}", Double.Parse(dr["LastNetworkValue"].ToString()));
            //c.Owner = dr["Owner"].ToString();

            //c.AllInitiativeStatuses = this.GetInitiativeStatuses();

            return s;
        }

        public void AddStaff(PracticeAdmin model)
        {
            SqlCommand com = new SqlCommand("spAddNewStaff", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add("@PracticeId", SqlDbType.Int);
            com.Parameters.Add("@StaffTypeId", SqlDbType.Int);
            com.Parameters.Add("@FirstName", SqlDbType.NVarChar);
            com.Parameters.Add("@LastName", SqlDbType.NVarChar);
            com.Parameters.Add("@EmailAddress", SqlDbType.NVarChar);
            com.Parameters.Add("@Phone", SqlDbType.NVarChar);
            com.Parameters.Add("@NPI ", SqlDbType.Int);
            com.Parameters.Add("@CreatedById", SqlDbType.Int);

            //replace this part with coding when we have Create Staff part done
            //Create Staff is the prior step before creating user login per system design
            com.Parameters["@PracticeId"].Value = 7;   //practice staff type id = 4           

            //hard code for practice staff. Will be replaced with Staff Type look ups
            com.Parameters["@StaffTypeId"].Value = 5;

            if (!String.IsNullOrWhiteSpace(model.Entity.FirstName))
            {
                com.Parameters["@FirstName"].Value = model.Entity.FirstName;
            }
            else
            {
                com.Parameters["@FirstName"].Value = DBNull.Value;
            }           

            if (!String.IsNullOrWhiteSpace(model.Entity.LastName))
            {
                com.Parameters["@LastName"].Value = model.Entity.LastName;
            }
            else
            {
                com.Parameters["@LastName"].Value = DBNull.Value;
            }
           
            if (!String.IsNullOrWhiteSpace(model.Entity.EmailAddress))
            {
                com.Parameters["@EmailAddress"].Value = model.Entity.EmailAddress;
            }
            else
            {
                com.Parameters["@EmailAddress"].Value = DBNull.Value;
            }
           
            if (!String.IsNullOrWhiteSpace(model.Entity.Phone))
            {
                com.Parameters["@Phone"].Value = model.Entity.Phone;
            }
            else
            {
                com.Parameters["@Phone"].Value = DBNull.Value;
            }

            com.Parameters["@NPI"].Value = model.Entity.NPI;

            com.Parameters["@CreatedById"].Value = model.Entity.NPI;

            if (!String.IsNullOrWhiteSpace(HttpContext.Current.Session["UserId"].ToString()))
            {
                com.Parameters["@CreatedById"].Value = int.Parse(HttpContext.Current.Session["UserId"].ToString());
            }
            else
            {
                com.Parameters["@CreatedById"].Value = DBNull.Value;
            }

            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }       
    }
}