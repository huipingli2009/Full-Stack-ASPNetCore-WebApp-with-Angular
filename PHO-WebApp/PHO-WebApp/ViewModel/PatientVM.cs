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
            ptEntity = pt.GetPatient(PtId);
            return ptEntity;
        }
        private void SelectPatient()
        {
            PatientVM pt = new PatientVM();

            ptEntity = pt.GetPatient(Convert.ToInt32(EventArgument));

            SelectMode();
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
            //IsPatientSummaryAreaVisible = true;
            //IsPatientSummaryEditAreaVisible = false;
            IsPatientSummaryEditAreaVisible = true;

            Mode = "SelectPatient";
        }
        public void HandleRequest()
        {
            //EventCommand = "PatienTlist";

            //IsPatientListAreaVisible = true;

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
                case "editpatient":
                    IsValid = true;
                    IsPatientSummaryEditAreaVisible = true;
                    IsPatientSummaryAreaVisible = false;
                    IsPatientListAreaVisible = false;
                    break;

                default:
                    break;
            }
        }
    }
}