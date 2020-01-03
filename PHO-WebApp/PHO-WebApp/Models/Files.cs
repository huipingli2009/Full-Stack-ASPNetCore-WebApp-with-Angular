using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PHO_WebApp.Models
{    
    public class Files
    {
        public int PracticeId { get; set; }
        public int Watch_ActionId { get; set; }
        public string Watch { get; set; } //it's a 1 or 0. Display at front end with Yes or No
        //resource could be a folder, a sub folder, 
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceDesc { get; set; }       
        //file is a resource
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string FileUrl { get; set; }  
        public string Format { get; set; }
        public string Folder { get; set; }
        public string SubFolder { get; set; }        
        public string AdminURL { get; set; }
        public string PermissionTo { get; set; }
        public string PermissionToId { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? DateModified { get; set; }
    }
}