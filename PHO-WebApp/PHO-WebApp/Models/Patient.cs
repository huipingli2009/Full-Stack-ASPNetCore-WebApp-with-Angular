﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PHO_WebApp.Models
{
    public class Patient
    {
        public int Id {get;set;}       
        public int patientId { get; set; }
        [Required(ErrorMessage = "First Name can't be blank")]
        public string FirstName { get; set; }       
        [Required(ErrorMessage = "Last Name can't be blank")]
        public string LastName { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Date of Birth can't be blank")]
        public DateTime? DOB { get; set; }
        public string Gender { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string StateId { get; set; }
        public string Zip { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
        public string Condition { get; set; }
        public int StaffId { get; set; }    //for provider dropdownlist, here for PCP provider
        public string PCPFirstName { get; set; }
        public string PCPLastName { get; set; }
        public string PCPName { get; set; }  //new change per request
        public string PMCAScore { get; set; }
        public string ProviderPMCAScore { get; set; }
        public string ProviderPMCANotes { get; set; }
        public string PMCA_ProvFirst { get; set; }
        public string PMCA_ProvLast { get; set; }
        public string Insurance { get; set; }
        public int? InsuranceId { get; set; }
        public int practiceId { get; set; }
        public string PAT_MRN_ID { get; set; }
        public bool ActiveStatus { get; set; }
        public DateTime? LastHospitalAdmitDate { get; set;}
        public string FinancialClass { get; set; } //commercial or medicaid or self insured
        public DateTime? LastPracticeVisitDate { get; set; }
        public List<PracticeInsurance> Payors { get; set;} //for dropdown of practice insurance
        public List<PatientProvider> PatientProviders { get; set;}
        public string LookupDisplayText
        {
            get
            {
                string returnValue = string.Empty;

                if (!string.IsNullOrEmpty(this.LastName))
                {
                    returnValue += this.LastName;
                    if (!string.IsNullOrEmpty(this.FirstName))
                    {
                        returnValue += ", ";
                        returnValue += this.FirstName;

                    }

                    if (this.DOB.HasValue)
                    {

                        returnValue += "DOB: ";
                        returnValue += this.DOB.Value.ToShortDateString();
                    }

                    if (this.patientId > 0)
                    {
                        returnValue += " ID: ";
                        returnValue += this.patientId.ToString();
                    }
                }

                return returnValue.Trim().ToUpper();

            }
        }

    }

    public class Practice
    {
        public int practiceId { get; set; }
        public string PracticeName { get; set; }
        public string PracticeDescription { get; set; }
    }
    public class PatientProvider
    {
        public int StaffId { get; set; }
        public string Staff_Name { get; set; }  //provider's last name
        //public string Staff_FirstName { get; set; } //provider's first name
        //public string Staff_LastName { get; set; }  //provider's last name        
    }
}