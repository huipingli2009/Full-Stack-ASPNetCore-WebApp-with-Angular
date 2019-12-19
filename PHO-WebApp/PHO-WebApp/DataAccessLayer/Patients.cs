using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PHO_WebApp.Models;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using PHO_WebApp.ViewModel;


namespace PHO_WebApp.DataAccessLayer
{   
    public class Patients
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        static int temp = 0;

        public List<Patient> GetPatients(int practiceId)
        {
            List<Patient> ptList = new List<Patient>();
            temp = practiceId;
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

        public void AddPatient(Patient model, UserDetails userDetails)
        {
            SqlCommand com = new SqlCommand("spAddPatient", con);
            com.CommandType = CommandType.StoredProcedure;  

            com.Parameters.Add("@PracticeId", SqlDbType.Int);
            com.Parameters.Add("@FirstName", SqlDbType.NVarChar);
            com.Parameters.Add("@LastName", SqlDbType.NVarChar);
            com.Parameters.Add("@DOB", SqlDbType.NVarChar);
            com.Parameters.Add("@AddressLine1", SqlDbType.NVarChar);
            com.Parameters.Add("@AddressLine2", SqlDbType.NVarChar);
            com.Parameters.Add("@City", SqlDbType.NVarChar);
            com.Parameters.Add("@State", SqlDbType.NVarChar);
            com.Parameters.Add("@Zip", SqlDbType.NVarChar);
           
            com.Parameters.Add("@CreatedById", SqlDbType.Int);
           
            com.Parameters["@PracticeId"].Value = userDetails.PracticeId;   //practice staff type id = 4           

            if (!String.IsNullOrWhiteSpace(model.FirstName))
            {
                com.Parameters["@FirstName"].Value = model.FirstName;
            }
            else
            {
                com.Parameters["@FirstName"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.LastName))
            {
                com.Parameters["@LastName"].Value = model.LastName;
            }
            else
            {
                com.Parameters["@LastName"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.DOB.ToString()))
            {
                com.Parameters["@DOB"].Value = model.DOB;
            }
            else
            {
                com.Parameters["@DOB"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.AddressLine1))
            {
                com.Parameters["@AddressLine1"].Value = model.AddressLine1;
            }
            else
            {
                com.Parameters["@AddressLine1"].Value = DBNull.Value;
            }

            com.Parameters["@AddressLine2"].Value = model.AddressLine2;

            com.Parameters["@City"].Value = model.City;
            com.Parameters["@State"].Value = model.State;
            com.Parameters["@Zip"].Value = model.Zip;

            if (userDetails != null && userDetails.LoginId > 0)
            {
                com.Parameters["@CreatedById"].Value = userDetails.LoginId;
            }
            else
            {
                com.Parameters["@CreatedById"].Value = DBNull.Value;
            }

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

            p.AddressLine1 = dr["AddressLine1"].ToString();
            p.AddressLine2 = dr["AddressLine2"].ToString();
            p.City = dr["City"].ToString();
            p.State = dr["State"].ToString();
            p.Zip = dr["Zip"].ToString();
            p.Phone1 = dr["Phone1"].ToString();
            p.Phone2 = dr["Phone2"].ToString();
            p.Condition = dr["Condition"].ToString();

            
            //read thru all the condion ids and save in an array
            var query = from val in (dr["ConditionIDs"].ToString()).Split(',')
                        select int.Parse(val);

            p.PatientConditions = this.GetPatientConditionsAll();
           
            foreach (int num in query)
            {
                foreach(Conditions pc in p.PatientConditions)
                {
                    if (pc.ID == num)
                    {
                        p.PatientConditionsSelected.Add(pc);
                    }
                }
            }
            //p.ConditionId = int.Parse(dr["ConditionIDs"].ToString());
            p.Gender = dr["Gender"].ToString();
            p.GenderId = int.Parse(dr["GenderID"].ToString());
            p.PMCAScore = dr["PMCAScore"].ToString();
            //p.ProviderPMCAScoreId = int.Parse(dr["ProviderPMCAScoreId"].ToString());
            p.ProviderPMCAScoreId = int.Parse(dr["ProviderPMCAScoreId"].ToString());
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
            p.Payors = this.GetInsuranceStatuses(temp);

            //p.PatientProviders = this.GetPatientProviders(70);

            p.PatientProviders = this.GetPracticeProviders(temp);
            p.PatientGenderList = this.GetPatientGenderList();
            p.PatientConditions = this.GetPatientConditionList();
            p.PMCAScore_Provider = this.GetPMCAScoreList();

            return p;
        }

        //work for insurance
        public List<PracticeInsurance> GetPracticeInsuranceList()
        {
            List<PracticeInsurance> returnObject = null;

            SqlCommand com = new SqlCommand("spGetPracticeInsurance", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.AddWithValue("@PracticeId", temp);

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
           
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
            c.Staff_Name= dr["StaffName"].ToString();

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
        public List<PatientProvider> GetPracticeProviders(int PracticeId)
        {
            PracticeId = temp;
            List<PatientProvider> returnObject = null;

            SqlCommand com = new SqlCommand("spGetPracticeProviders", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterPracticeId = new SqlParameter("@PracticeId", SqlDbType.Int);
            parameterPracticeId.Value = PracticeId;
            com.Parameters.Add(parameterPracticeId);

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

        public List<PatientGender> GetPatientGenderList()
        {
            List<PatientGender> returnObject = null;

            SqlCommand com = new SqlCommand("spGetPatientGender", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();            
            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (returnObject == null)
                {
                    returnObject = new List<PatientGender>();
                }
                PatientGender PtGender = CreatePatientGenderModel(ds.Tables[0].Rows[i]);
                returnObject.Add(PtGender);
            }

            return returnObject;
        }

        public PatientGender CreatePatientGenderModel(DataRow dr)
        {
            PatientGender c = new PatientGender();
            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                c.Id = int.Parse(dr["Id"].ToString());
            }
            c.Gender = dr["Gender"].ToString();          

            return c;
        }
        public List<Conditions> GetPatientConditionList()
        {
            List<Conditions> returnObject = null;

            SqlCommand com = new SqlCommand("spGetAllConditions", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (returnObject == null)
                {
                    returnObject = new List<Conditions>();
                }
                Conditions PtCondition = CreatePatientConditionModel(ds.Tables[0].Rows[i]);
                returnObject.Add(PtCondition);
            }

            return returnObject;
        }

        public List<Conditions> GetPatientConditionsAll()
        {
            List<Conditions> returnObject = null;

            SqlCommand com = new SqlCommand("spGetAllConditions", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (returnObject == null)
                {
                    returnObject = new List<Conditions>();
                }
                Conditions PtCondition = CreatePatientConditionModel(ds.Tables[0].Rows[i]);
                returnObject.Add(PtCondition);
            }

            return returnObject;
        }
        public Conditions CreatePatientConditionModel(DataRow dr)
        {
            Conditions c = new Conditions();
            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                c.ID = int.Parse(dr["Id"].ToString());
            }
            c.Condition = dr["Condition"].ToString();          

            return c;
        }
        public List<PMCAScoreFromProvider> GetPMCAScoreList()
        {
            List<PMCAScoreFromProvider> returnObject = null;

            SqlCommand com = new SqlCommand("spGetPMCAScore", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (returnObject == null)
                {
                    returnObject = new List<PMCAScoreFromProvider>();
                }
                PMCAScoreFromProvider PtPMCAScore = CreatePMCAScoreModel(ds.Tables[0].Rows[i]);
                returnObject.Add(PtPMCAScore);
            }

            return returnObject;
        }
        public PMCAScoreFromProvider CreatePMCAScoreModel(DataRow dr)
        {
            PMCAScoreFromProvider c = new PMCAScoreFromProvider();
            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                c.Id = int.Parse(dr["Id"].ToString());
            }
            c.PMCAScoreId = int.Parse(dr["PMCAScore"].ToString());

            return c;
        }
    }
}