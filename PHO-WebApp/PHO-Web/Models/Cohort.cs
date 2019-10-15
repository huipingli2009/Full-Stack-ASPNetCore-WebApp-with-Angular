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

        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }
        
        public string ShortName { get; set; }

        [Required(ErrorMessage = "Description can't be blank")]
        public string Description { get; set; }
        public string Details { get; set; }
        public string SQL { get; set; }
        public string Exceptions { get; set; }
        public string DataSources { get; set; }
        public string Lookback { get; set; }
        public string Calculations { get; set; }
        public string Limitations { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Owner { get; set; }

        public string DeletedFlag { get; set; }

        public string CreatedById { get; set; }

        [Required(ErrorMessage = "Required")]
        public int Status { get; set; }

        public string StatusName { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOnDate { get; set; }

        public string ModifiedById { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime?ModifiedDate { get; set; }

        public string DeletedById { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime DeletedDate { get; set; }

        public List<CohortStatus> AllStatuses { get; set; }

        //public List<Cohort> ShowallCohort { get; set; }
        private List<Measure> _Measures;

        public List<Measure> Measures
        {
            get {
                if (_Measures == null)
                {
                    _Measures = new List<Measure>();
                }
                return _Measures;
            }
            set { _Measures = value; }
        }
    }

}