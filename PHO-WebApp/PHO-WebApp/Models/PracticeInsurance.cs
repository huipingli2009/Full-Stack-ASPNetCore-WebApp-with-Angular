using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PHO_WebApp.Models
{
    public class PracticeInsurance
    {
        public int Id { get; set;}
        public string InsuranceName { get; set;}
        public bool Medicaid { get; set; }
        public string PayorType { get; set; }
    }
}