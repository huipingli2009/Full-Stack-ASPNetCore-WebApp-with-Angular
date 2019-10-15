using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PHO_WebApp.Models
{
    public class CohortList
    {
        public IEnumerable<Cohort> Cohorts { get; set; }
    }
}