using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PHO_WebApp.Models;
using PHO_WebApp.DataAccessLayer;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace PHO_WebApp.ViewModel
{
    public class PatientVM
    {
        private SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public List<Patient> PatientList { get; set; }
        public UserDetails UserLogin { get; set; }     

        public Patient ptEntity { get; set; }
        static int temp;

        public string EventCommand { get; set; }
        public string EventArgument { get; set; }
        public string Mode { get; set; }
        public bool IsValid { get; set; }

        public bool IsPatientListAreaVisible { get; set; }
       
        public bool IsPatientSummaryAreaVisible { get; set; }

        public bool IsPatientSummaryEditAreaVisible { get; set; }
        public PatientVM()
        {
            Init();
            PatientList = new List<Patient>();
            ptEntity = new Patient();
        }
        private void Init()
        {
            EventCommand = "PatientList";
            EventArgument = string.Empty;
            ListMode();
        }      

        private void GetPatients()
        {           
            Patients pts = new Patients();           
            PatientList = pts.GetPatients(UserLogin.PracticeId);
        }

        public Patient GetPatient(int PtId)
        {
            Patients pt = new Patients(); //calls Patient DAL

            temp = PtId;
            ptEntity = pt.GetPatient(PtId);
            return ptEntity;
        }
        private void SelectPatient()
        {
            PatientVM pt = new PatientVM();

            ptEntity = pt.GetPatient(Convert.ToInt32(EventArgument));

            SelectMode();
        }
        private void SavePatient()
        {
            PatientVM pt = new PatientVM();

            //added here for testing purpose
            IsValid = true;

            if (IsValid)
            {
                if (Mode == "AddPatient")
                {
                    //pt.Insert(ptEntity, UserLogin.PracticeId);
                    //Insert(vmPA);
                }
                else   //update Patient
                {                   
                    pt.UpdatePatient(ptEntity);                   
                }
            }
            else

            {
                if (Mode == "AddStaff")
                {
                    //AddMode();
                }
                else   //update staff
                {
                    EditMode();
                }
            }
        }
        //private void EditPatient()
        //{
        //    PatientVM pt = new PatientVM();

        //    ptEntity = pt.GetPatient(Convert.ToInt32(EventArgument));

        //    EditMode();
        //}
        private void ListMode()
        {
            IsValid = true;
            IsPatientListAreaVisible = true;
            IsPatientSummaryAreaVisible = false;
            IsPatientSummaryEditAreaVisible = false;          

            Mode = "PatientList";
        }
        private void SelectMode()
        {
            IsPatientListAreaVisible = false;
            IsPatientSummaryAreaVisible = false;
            IsPatientSummaryEditAreaVisible = true;
            Mode = "SelectPatient";
        }
        private void EditMode()
        {
            IsPatientListAreaVisible = false;
            IsPatientSummaryAreaVisible = false;
            IsPatientSummaryEditAreaVisible = true;
            Mode = "EdittPatient";
        }
        public void HandleRequest()
        {
            switch (EventCommand.ToLower())
            {
                case "patientlist":
                    ListMode();
                    GetPatients();
                    break;
                case "selectpatient":
                    IsValid = true;                   
                    SelectPatient();
                    break;
               
                case "savepatient":
                    IsValid = true;                  
                    SavePatient();
                    GetPatients();
                    break;

                default:
                    break;
            }
        }
        public void UpdatePatient(Patient model)
        {
            SqlCommand com = new SqlCommand("spUpdatePatientSummary", con);
            com.CommandType = CommandType.StoredProcedure;

            //com.Parameters.Add("@PracticeId", SqlDbType.Int);

            com.Parameters.Add("@Id", SqlDbType.Int);            
            com.Parameters.Add("@FirstName", SqlDbType.NVarChar);
            com.Parameters.Add("@LastName", SqlDbType.NVarChar);
            com.Parameters.Add("@PatientDOB", SqlDbType.DateTime);
            com.Parameters.Add("@Gender", SqlDbType.NVarChar);
            com.Parameters.Add("@Email", SqlDbType.NVarChar);
            com.Parameters.Add("@Phone1", SqlDbType.NVarChar);
            com.Parameters.Add("@Phone2", SqlDbType.NVarChar);
            com.Parameters.Add("@AddressLine1", SqlDbType.NVarChar);
            com.Parameters.Add("@AddressLine2", SqlDbType.NVarChar);
            com.Parameters.Add("@City", SqlDbType.NVarChar);
            com.Parameters.Add("@State", SqlDbType.NVarChar);
            com.Parameters.Add("@Zip", SqlDbType.NVarChar);
            //com.Parameters.Add("@ModifiedById", SqlDbType.Int);
            //com.Parameters.Add("@CreatedONDate", SqlDbType.DateTime);                    

            com.Parameters["@Id"].Value = temp; //model.Id; //hard coding for now
            
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

            //Convert.ToDateTime(dr["EffectiveDate"].ToString());
            if (!String.IsNullOrWhiteSpace(Convert.ToDateTime(model.DOB).ToString()))
            {
                com.Parameters["@PatientDOB"].Value = model.DOB;
            }
            else
            {
                com.Parameters["@PatientDOB"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.Gender))
            {
                com.Parameters["@Gender"].Value = model.Gender;
            }
            else
            {
                com.Parameters["@Gender"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                com.Parameters["@Email"].Value = model.Email;
            }
            else
            {
                com.Parameters["@Email"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.Phone1))
            {
                com.Parameters["@Phone1"].Value = model.Phone1;
            }
            else
            {
                com.Parameters["@Phone1"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.Phone2))
            {
                com.Parameters["@Phone2"].Value = model.Phone2;
            }
            else
            {
                com.Parameters["@Phone2"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.AddressLine1))
            {
                com.Parameters["@AddressLine1"].Value = model.AddressLine1;
            }
            else
            {
                com.Parameters["@AddressLine1"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.AddressLine2))
            {
                com.Parameters["@AddressLine2"].Value = model.AddressLine2;
            }
            else
            {
                com.Parameters["@AddressLine2"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.City))
            {
                com.Parameters["@City"].Value = model.City;
            }
            else
            {
                com.Parameters["@City"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.State))
            {
                com.Parameters["@State"].Value = model.State;
            }
            else
            {
                com.Parameters["@State"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.Zip))
            {
                com.Parameters["@Zip"].Value = model.Zip;
            }
            else
            {
                com.Parameters["@Zip"].Value = DBNull.Value;
            }        

            //if (UserLogin != null && UserLogin.LoginId > 0)
            //{
            //    com.Parameters["@ModifiedById"].Value = UserLogin.LoginId;
            //}
            //else
            //{
            //    com.Parameters["@ModifiedById"].Value = DBNull.Value;
            //}
           
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }
    }
}