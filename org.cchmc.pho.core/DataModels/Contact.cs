using System.Collections.Generic;

namespace org.cchmc.pho.core.DataModels
{
    public class Contact
    {
        public int PracticeId { get; set; }
        public string PracticeName { get; set; }
        public string PracticeType { get; set; }
        public string EMR { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }       
        public string WebsiteURL { get; set; }
    }
}
