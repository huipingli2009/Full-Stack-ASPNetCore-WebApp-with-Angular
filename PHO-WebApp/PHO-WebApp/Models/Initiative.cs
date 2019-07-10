using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PHO_WebApp.Models
{
    using System;
    using System.Collections.Generic;

    public class Initiative
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string Owner { get; set; }
        public string PrimaryAim { get; set; }
        public string SecondaryAim { get; set; }
        public string EvidenceGuidelines { get; set; }
        public int Status { get; set; }
        public string StatusDesc { get; set; }
        public int CreatedById { get; set; }
        public int ModifiedById { get; set; }
        public int DeletedById { get; set; }


        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOnDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        //public DateTime ModifiedDate { get; set; }
        public DateTime? ModifiedDate { get; set; } //this change can handel nullabale Datetime situations

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime DeletedDate { get; set; }

        public List<InitiativeStatus> AllInitiativeStatuses { get; set; }

       

    }
}