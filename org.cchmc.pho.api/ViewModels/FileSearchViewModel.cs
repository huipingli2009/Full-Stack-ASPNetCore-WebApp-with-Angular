using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class FileSearchViewModel
    {
       public int? resourceTypeId { get; set; }
       public int? initiativeId { get; set; }
       public string tag { get; set; }
       public bool? watch { get; set; }
       public string name { get; set; }
    }
}
