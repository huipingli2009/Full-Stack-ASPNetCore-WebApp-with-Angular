using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PHO_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace PHO_WebApp.Models
{
    public class Measures
    {
        [Display(Name = "Measure")]
        public List<Measure> MeasureList { get; set; }
    }
}