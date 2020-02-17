using System;
using System.Collections.Generic;
using System.Text;

namespace org.cchmc.pho.core.Models
{
    public class ConnectionStrings
    {
        public static readonly string SECTION_KEY = "ConnectionStrings";
        public string phodb { get; set; }
        public string phodw { get; set; }
        public string phoidentity { get; set; }
    }
}
