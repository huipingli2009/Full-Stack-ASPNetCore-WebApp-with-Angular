﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PHO_WebApp.Models
{
    public class Patient
    {
        public int patientId {get;set;} 
        public string FirstName { get; set; }
        public string LastName { get; set; }
       
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }
        public string Gender { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State{ get; set; }
        public string StateId { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Condition { get; set; }
        public string PCPFirstName { get; set; }
        public string PCPLastName { get; set; }
        public string PMCAScore { get; set; }
        public string ProviderPMCAScore { get; set; }

        public string Insurance { get; set; }
        public int? InsuranceId { get; set; }
        public int practiceId { get; set;}
        public string PAT_MRN_ID { get; set; }
        public string ActiveStatus { get; set; }
        public DateTime? LastHospitalAdmitDate { get; set; }
        public string FinancialClass { get; set; } //commercial or medicaid or self insured
        public DateTime? LastPracticeVisitDate { get; set; }

    }

    public class Practice
    {
        public int practiceId { get; set; }
        public string PracticeName { get; set; }
        public string PracticeDescription { get; set; }
    }
}