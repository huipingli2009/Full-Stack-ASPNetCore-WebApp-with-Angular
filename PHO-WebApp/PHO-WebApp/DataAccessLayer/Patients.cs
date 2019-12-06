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
              
        public List<Patient> GetPatients(int practiceId)
        {
            List<Patient> ptList = new List<Patient>();
            
            //SqlCommand com = new SqlCommand("spGetPracticePatients", con);
            SqlCommand com = new SqlCommand("spGetPracticePatients", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.AddWithValue("@practiceId", practiceId);

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);

            for (int i=0; i < ds.Tables[0].Rows.Count; i++)
            {
                Patient pt = new Patient();
                pt = CreatePatientsModel(ds.Tables[0].Rows[i]);

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

        public List<PracticeInsurance> GetInsuranceStatuses(int practiceId)
        {
            List<PracticeInsurance> returnObject = null;

            SqlCommand com = new SqlCommand("spGetPracticeInsurance", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterPracticeId = new SqlParameter("@PracticeId", SqlDbType.Int);
            parameterPracticeId.Value = practiceId;
            com.Parameters.Add(parameterPracticeId);

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            //Cohorts = ds.Tables[0].
            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (returnObject == null)
                {
                    returnObject = new List<PracticeInsurance>();
                }
                PracticeInsurance c = CreateInsuranceStatusModel(ds.Tables[0].Rows[i]);
                returnObject.Add(c);
            }

            return returnObject;
        }

        public PracticeInsurance CreateInsuranceStatusModel(DataRow dr)
        {
            PracticeInsurance c = new PracticeInsurance();
            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                c.Id = int.Parse(dr["Id"].ToString());
            }

            if (dr["InsName"] != null && !string.IsNullOrWhiteSpace(dr["InsName"].ToString()))
            {
                c.InsuranceName = dr["InsName"].ToString();
            }

            return c;
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
            com.Parameters.AddWithValue("@id", pt.Id);
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
            com.Parameters.AddWithValue("@id", pt.Id);
           
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }

        public Patient CreatePatientsModel(DataRow dr)
        {
            Patient p = new Patient();

            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                p.Id = SharedLogic.ParseNumeric(dr["Id"].ToString());
            }           

            p.FirstName = dr["FirstName"].ToString();
            p.LastName = dr["LastName"].ToString();
            p.DOB = Convert.ToDateTime(dr["PatientDOB"].ToString());

            //p.Email = dr["Email"].ToString();
            // p.ActiveStatus = dr["ActiveStatus"].ToString();

            p.AddressLine1 = dr["AddressLine1"].ToString();
            //p.AddressLine2 = dr["AddressLine2"].ToString();
            p.City = dr["City"].ToString();
            p.State = dr["State"].ToString();
            p.Zip = dr["Zip"].ToString();
            //p.Phone1 = dr["Phone1"].ToString();
            //p.Phone2 = dr["Phone2"].ToString();
            p.Condition = dr["Condition"].ToString();
            //p.Gender = dr["Gender"].ToString();
            //p.PMCAScore = dr["PMCAScore"].ToString();
            //p.ProviderPMCAScore = dr["ProviderPMCAScore"].ToString();
            //p.ProviderPMCANotes = dr["ProviderPMCANotes"].ToString();
            //p.PMCA_ProvFirst = dr["PMCA_ProvFirst"].ToString();
            //p.PMCA_ProvLast = dr["PMCA_ProvLast"].ToString();
            //p.PCPFirstName = dr["PCP_FirstName"].ToString();
            //p.PCPLastName = dr["PCP_LastName"].ToString();

            ////s.StateId = SharedLogic.ParseNumeric(dr["StateId"].ToString());

            //p.Phone = dr["Phone"].ToString();    

            return p;
        }

        public Patient CreatePatientModel(DataRow dr)
        {
            Patient p = new Patient();            

            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                p.Id = SharedLogic.ParseNumeric(dr["Id"].ToString());
            }          

            p.FirstName = dr["FirstName"].ToString();
            p.LastName = dr["LastName"].ToString();
            p.DOB = Convert.ToDateTime(dr["PatientDOB"].ToString());

            p.Email = dr["Email"].ToString();
            p.Insurance = dr["InsName"].ToString();

            if (dr["InsId"] != null && !string.IsNullOrWhiteSpace(dr["InsId"].ToString()))
            {
                p.InsuranceId = Int32.Parse(dr["InsId"].ToString());
            }
            
            //p.Insurance = dr["I"]
           // p.ActiveStatus = dr["ActiveStatus"].ToString();

            p.AddressLine1 = dr["AddressLine1"].ToString();
            p.AddressLine2 = dr["AddressLine2"].ToString();
            p.City = dr["City"].ToString();
            p.State = dr["State"].ToString();
            p.Zip = dr["Zip"].ToString();
            p.Phone1 = dr["Phone1"].ToString();
            p.Phone2 = dr["Phone2"].ToString();
            p.Condition = dr["Condition"].ToString();
            p.Gender = dr["Gender"].ToString();
            p.PMCAScore = dr["PMCAScore"].ToString();
            p.ProviderPMCAScore = dr["ProviderPMCAScore"].ToString();
            p.ProviderPMCANotes = dr["ProviderPMCANotes"].ToString();
            p.PMCA_ProvFirst = dr["PMCA_ProvFirst"].ToString();
            p.PMCA_ProvLast = dr["PMCA_ProvLast"].ToString();
            p.PCPFirstName = dr["PCP_FirstName"].ToString();
            p.PCPLastName = dr["PCP_LastName"].ToString();
            
            if (dr["PCP_ID"] != null && !string.IsNullOrWhiteSpace(dr["PCP_ID"].ToString()))
            {
                p.StaffId = Int32.Parse(dr["PCP_ID"].ToString());
            }

            //use SPA id = 37 for now
            //p.Payors = this.GetInsuranceStatuses(practiceId);
            p.Payors = this.GetInsuranceStatuses(70);

            p.PatientProviders = this.GetPatientProviders(349071);



            ////s.StateId = SharedLogic.ParseNumeric(dr["StateId"].ToString());

            //p.Phone = dr["Phone"].ToString();    

            return p;
        }

        //work for insurance
        public List<PracticeInsurance> GetPracticeInsuranceList()
        {
            List<PracticeInsurance> returnObject = null;

            SqlCommand com = new SqlCommand("spGetPracticeInsurance", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            //Cohorts = ds.Tables[0].
            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (returnObject == null)
                {
                    returnObject = new List<PracticeInsurance>();
                }
                PracticeInsurance PracIns = CreatePracticeInsuranceModel(ds.Tables[0].Rows[i]);
                returnObject.Add(PracIns);
            }

            return returnObject;
        }

        public PracticeInsurance CreatePracticeInsuranceModel(DataRow dr)
        {
            PracticeInsurance c = new PracticeInsurance();
            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                c.Id = int.Parse(dr["Id"].ToString());
            }
            c.InsuranceName = dr["InsName"].ToString();

            if (dr["Medicaid"].ToString() == "1")
            {
                c.Medicaid = true;
            }
            else
            {
                c.Medicaid = false;
            }
           
            c.PayorType = dr["PayorType"].ToString();

            return c;
        }

        public PatientProvider CreatePatientProviderModel(DataRow dr)
        {
            PatientProvider c = new PatientProvider();
            if (dr["StaffId"] != null && !string.IsNullOrWhiteSpace(dr["StaffId"].ToString()))
            {
                c.StaffId = int.Parse(dr["StaffId"].ToString());
            }
            //c.Staff_FirstName = dr["Name"].ToString();           
            c.Staff_Name= dr["Name"].ToString();

            return c;
        }

        public List<PatientProvider> GetPatientProviders(int PatientId)
        {
            List<PatientProvider> returnObject = null;

            SqlCommand com = new SqlCommand("spGetPatientProvider", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterPatientId = new SqlParameter("@PatientId", SqlDbType.Int);
            parameterPatientId.Value = PatientId;
            com.Parameters.Add(parameterPatientId);

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (returnObject == null)
                {
                    returnObject = new List<PatientProvider>();
                }
                PatientProvider PatProvider = CreatePatientProviderModel(ds.Tables[0].Rows[i]);
                returnObject.Add(PatProvider);
            }

            return returnObject;
        }
    }
}