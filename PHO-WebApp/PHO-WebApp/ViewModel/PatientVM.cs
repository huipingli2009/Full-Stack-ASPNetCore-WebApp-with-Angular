using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PHO_WebApp.Models;
using PHO_WebApp.DataAccessLayer;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.ComponentModel.DataAnnotations;

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

        public bool IsAddPatientIAreaVisible { get; set; }
        
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
        
        private void deletePatient()
        {
            PatientVM ptVM = new PatientVM();
            Patient pt = new Patient();

            pt.Id = Convert.ToInt32(EventArgument);

            ptVM.DeletePatient(pt);

            GetPatients();

            ListMode();
        }
        public void DeletePatient(Patient pt)
        {
            SqlCommand com = new SqlCommand("spDeletePatient", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterPatientId = new SqlParameter("@id", SqlDbType.Int);
            parameterPatientId.Value = temp;
            com.Parameters.Add(parameterPatientId);

            con.Open();
            com.ExecuteNonQuery();
            con.Close();
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
                    pt.Insert(ptEntity, UserLogin.PracticeId);
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

        private void AddPatient()
        {
            PatientVM pt = new PatientVM();

            //added here for testing purpose
            IsValid = true;

            if (IsValid)
            {
                pt.Insert(ptEntity, UserLogin.PracticeId);
               /* Insert(vmPA)*/;
            }
            else

            {
                if (Mode == "AddStaff")
                {
                    AddMode();
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
            IsAddPatientIAreaVisible = false;

            Mode = "PatientList";
        }
        private void SelectMode()
        {
            IsPatientListAreaVisible = false;
            IsPatientSummaryAreaVisible = false;
            IsPatientSummaryEditAreaVisible = true;
            IsAddPatientIAreaVisible = false;

            Mode = "SelectPatient";
        }
        private void EditMode()
        {
            IsPatientListAreaVisible = false;
            IsPatientSummaryAreaVisible = false;
            IsPatientSummaryEditAreaVisible = true;
            IsAddPatientIAreaVisible = false;

            Mode = "EdittPatient";
        }
        private void Add()
        {
            IsValid = true;

            //initialize
            ptEntity = new Patient();
            ptEntity.LastName = string.Empty;
            ptEntity.FirstName = string.Empty;
            ptEntity.DOB = null;

            ptEntity.AddressLine1 = "";
            ptEntity.AddressLine2 = "";
            ptEntity.City = "";
            ptEntity.State = "";
            ptEntity.Zip = "";         
          

            //Put ViewModel mode to AddMode
            AddMode();
        }

        private void AddMode()
        {
            IsPatientListAreaVisible = false;
            IsPatientSummaryAreaVisible = false;
            IsPatientSummaryEditAreaVisible = false;
            IsAddPatientIAreaVisible = true;

            Mode = "AddPatient";
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
                case "deletepatient":
                    IsValid = true;
                    deletePatient();
                    GetPatients();
                    break;
                case "savepatient":
                    IsValid = true;                  
                    SavePatient();
                    GetPatients();
                    break;
                case "addpatient":
                    IsValid = true;
                    Add();
                   // AddPatient();
                   // GetPatients();
                    break;

                default:
                    break;
            }
        }
        public void Insert(Patient model, int practiceId)
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
           //com.Parameters.Add("@CreatedById", SqlDbType.Int);
            //com.Parameters.Add("@CreatedONDate", SqlDbType.DateTime);

            //replace this part with coding when we have Create Staff part done
            //Create Staff is the prior step before creating user login per system design
            com.Parameters["@PracticeId"].Value = practiceId;                         

           
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
                com.Parameters["@DOB"].Value = model.DOB;
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
                com.Parameters["@AddressLine2"].Value = model.AddressLine1;
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
            //    com.Parameters["@CreatedById"].Value = UserLogin.LoginId;
            //}
            //else
            //{
            //    com.Parameters["@CreatedById"].Value = DBNull.Value;
            //}

            //com.Parameters["@CreatedONDate"].Value = model.CreatedOnDate;

            con.Open();
            com.ExecuteNonQuery();
            con.Close();
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
            com.Parameters.Add("@GenderID", SqlDbType.Int);
            com.Parameters.Add("@Email", SqlDbType.NVarChar);
            com.Parameters.Add("@Phone1", SqlDbType.NVarChar);
            com.Parameters.Add("@Phone2", SqlDbType.NVarChar);
            com.Parameters.Add("@AddressLine1", SqlDbType.NVarChar);
            com.Parameters.Add("@AddressLine2", SqlDbType.NVarChar);
            com.Parameters.Add("@City", SqlDbType.NVarChar);
            com.Parameters.Add("@State", SqlDbType.NVarChar);
            com.Parameters.Add("@Zip", SqlDbType.NVarChar);
            com.Parameters.Add("@InsuranceId", SqlDbType.Int);
            com.Parameters.Add("@PCPId", SqlDbType.Int);
            com.Parameters.Add("@Conditions", SqlDbType.NVarChar);
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

            if(model.GenderId.HasValue)
            {
                com.Parameters["@GenderID"].Value = model.GenderId;
            }
            else
            {
                com.Parameters["@GenderID"].Value = DBNull.Value;
            }
            if (model.InsuranceId.HasValue)
            {
                com.Parameters["@InsuranceId"].Value = model.InsuranceId;
            }
            else
            {
                com.Parameters["@InsuranceId"].Value = DBNull.Value;
            }
            com.Parameters["@PCPId"].Value = model.StaffId;

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

            if (!String.IsNullOrWhiteSpace(model.PatientConditionsSelected.ToString()))
            {
                //com.Parameters["@Zip"].Value = model.Zip;
            }
            else
            {
                //com.Parameters["@Zip"].Value = DBNull.Value;
            }

            //Model.ptEntity.PatientConditionsSelected

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