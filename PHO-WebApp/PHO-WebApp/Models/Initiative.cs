using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PHO_WebApp.Models
{
    public class Initiative
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public string PrimaryAim { get; set; }
        public string SecondaryAim { get; set; }
        public string EvidenceGuidelines { get; set; }
        public int StatusId { get; set; }
        public string StatusDesc { get; set; }
        public int CreatedById { get; set; }
        public int ModifiedById { get; set; }
        public int DeletedById { get; set; }


        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOnDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime DeletedDate { get; set; }

        //public int ResourceId { get; set; }

    }
}