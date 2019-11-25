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
    public class Patients
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public List<Models.Patient> GetPatients(int practiceId)
        {
            List<Models.Patient> ptList = new List<Models.Patient>();

            SqlCommand com = new SqlCommand("spGetPracticePatients", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.AddWithValue("@practiceId", practiceId);

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Models.Patient pt = new Models.Patient();
                pt = CreatePatientModel(ds.Tables[0].Rows[i]);

                ptList.Add(pt);
            }
            return ptList;
        }

        public Patient GetPatient(int PtId)
        {
            Patient practicePatient = new Patient();

            SqlCommand com = new SqlCommand("spGetPatientSummary", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterPatientId = new SqlParameter("@id", SqlDbType.Int);
            parameterPatientId.Value = PtId;
            com.Parameters.Add(parameterPatientId);

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                practicePatient = CreatePatientModel(ds.Tables[0].Rows[i]);
            }

            return practicePatient;
        }

        public void AddPatient(Patient pt)
        {
            SqlCommand com = new SqlCommand("proc_AddPatient", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@FirstName", pt.FirstName);
            com.Parameters.AddWithValue("@LastName", pt.LastName);
            com.Parameters.AddWithValue("@DOB", pt.DOB);
            com.Parameters.AddWithValue("@AddressLine1", pt.AddressLine1);
            com.Parameters.AddWithValue("@City", pt.City);
            //com.Parameters.AddWithValue("@StateId", pt.State_Id);
            com.Parameters.AddWithValue("@Zip", pt.Zip);
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }

        public void UpPatient(Patient pt)
        {
            SqlCommand com = new SqlCommand("proc_UpdatePatient", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@id", pt.patientId);
            com.Parameters.AddWithValue("@FirstName", pt.FirstName);
            com.Parameters.AddWithValue("@LastName", pt.LastName);
            com.Parameters.AddWithValue("@DOB", pt.DOB);
            com.Parameters.AddWithValue("@AddressLine1", pt.AddressLine1);
            com.Parameters.AddWithValue("@City", pt.City);
            //com.Parameters.AddWithValue("@StateId", pt.State_Id);
            com.Parameters.AddWithValue("@Zip", pt.Zip);
            con.Open();
            com.ExecuteNonQuery();
            con.Close();

        }

        public void DeletePatient(Patient pt)
        {
            SqlCommand com = new SqlCommand("proc_DeletePatient", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@id", pt.patientId);

            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }

        public Patient CreatePatientModel(DataRow dr)
        {
            Patient p = new Patient();

            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                p.patientId = SharedLogic.ParseNumeric(dr["Id"].ToString());
            }

            //if (dr["StaffTypeId"] != null && !string.IsNullOrWhiteSpace(dr["StaffTypeId"].ToString()))
            //{
            //    p.StaffTypeId = SharedLogic.ParseNumeric(dr["StaffTypeId"].ToString());
            //}

            p.FirstName = dr["FirstName"].ToString();
            p.LastName = dr["LastName"].ToString();

            //p.EmailAddress = dr["Email"].ToString();

            p.AddressLine1 = dr["AddressLine1"].ToString();
            p.AddressLine2 = dr["AddressLine2"].ToString();
            p.City = dr["City"].ToString();
            p.State = dr["State"].ToString();
            p.Condition = dr["Condition"].ToString();

            ////s.StateId = SharedLogic.ParseNumeric(dr["StateId"].ToString());

            //p.Phone = dr["Phone"].ToString();    

            return p;
        }
    }
}