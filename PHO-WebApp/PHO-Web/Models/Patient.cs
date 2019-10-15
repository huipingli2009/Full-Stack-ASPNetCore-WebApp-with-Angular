using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PHO_WebApp.Models
{
    public class Patient
    {
        public int patientId {get;set;} 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateId { get; set; }
        public string Zip { get; set; }
        public int practiceId { get; set;}
    }

    public class Practice
    {
        public int practiceId { get; set; }
        public string PracticeName { get; set; }
        public string PracticeDescription { get; set; }
    }
}