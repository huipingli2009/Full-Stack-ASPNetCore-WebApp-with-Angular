using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PHO_WebApp.Models
{
    public class Measure
    {
        public int MeasureId { get; set; }
        //public int MeasureId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string MeasureName { get; set; }

        [Required(ErrorMessage = "Description can't be blank")]
        public string MeasureDesc { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Owner { get; set; }

        public string Frequency { get; set; }
        public string SQL { get; set; }
        public string Numerator { get; set; }
        public string Denominator { get; set; }

        public string Factor { get; set; }

        [Required(ErrorMessage = "Required")]
        public int Status { get; set; }
        public string StatusDesc { get; set; }

        public string CreatedbyId { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOnDate { get; set; }

        public string ModifiedbyId { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? ModifiedDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? EffectiveDate { get; set; }

        //use initiative status table as we have some contents
        public List<InitiativeStatus> AllInitiativeStatuses { get; set; }

        //use data from spGetMeasureInfoFromFactAggregate at PHO_DW
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime?LastMeasureDate { get; set;}

        [DisplayFormat]
        public string MeasureValue { get; set;}

        [DisplayFormat]
        public string LastNetworkValue { get; set; }
    }
}