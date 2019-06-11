using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PHO_WebApp.Models
{
    using System;
    using System.Collections.Generic;

    public class Cohort
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string SQL { get; set; }
        public string Exceptions { get; set; }
        public string DataSources { get; set; }
        public string Lookback { get; set; }
        public string Owner { get; set; }
        public string DeletedFlag { get; set; }
        public string CreatedById { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOnDate { get; set; }

        public string ModifiedById { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedDate { get; set; }

        public string DeletedById { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime DeletedDate { get; set; }

        //public List<Cohort> ShowallCohort { get; set; }
    }

}