using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using PHO_WebApp.Models;
using System.Security.Cryptography;

namespace PHO_WebApp.DataAccessLayer
{
    public class StaffDAL
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public List<Staff> getPracticeStaffs(int practiceId)
        {
            List<Staff> practiceStaffList = new List<Staff>();

            SqlCommand com = new SqlCommand("spGetPracticeStaff", con);
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
    }
}