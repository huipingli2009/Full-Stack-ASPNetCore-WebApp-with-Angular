using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PHO_WebApp.Models
{
    public class StaffType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public enum StaffTypeEnum
    {
        [Display(Name = "Provider")]
        Provider = 1,
        [Display(Name = "PHO_Staff")]
        PHO_Staff = 2,
        [Display(Name = "Practice_Admin")]
        Practice_Admin = 3,
        [Display(Name = "Practice_Staff")]
        Practice_Staff = 4,
        [Display(Name = "CCHMC_IS")]
        CCHMC_IS = 5
    }
}