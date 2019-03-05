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
              
        public DataSet GetPatients()
        {
            SqlCommand com = new SqlCommand("proc_PatientPopulation", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);
            return ds;
        }

        public DataSet GetPatientInfo(int id)
        {
            SqlCommand com = new SqlCommand("proc_GetPatientInfo", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.AddWithValue("@id", id);

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        public void AddPatient(Patient pt)
        {
            SqlCommand com = new SqlCommand("proc_AddPatient", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@FirstName", pt.FirstName);
            com.Parameters.AddWithValue("@LastName", pt.LastName);
            com.Parameters.AddWithValue("@DOB", pt.PersonDOB);
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
            com.Parameters.AddWithValue("@id", pt.Id);
            com.Parameters.AddWithValue("@FirstName", pt.FirstName);
            com.Parameters.AddWithValue("@LastName", pt.LastName);
            com.Parameters.AddWithValue("@DOB", pt.PersonDOB);
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
            com.Parameters.AddWithValue("@id", pt.Id);
           
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }
    }
}