using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PHO_WebApp.Models
{
    public class StaffType
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public enum StaffTypeEnum
    {
        Provider=1,
        PHO_Staff=2,
        Practice_Admin=3,
        Practice_Staff=4,
        CCHMC_IS=5
    }
}