using System;
using System.Collections.Generic;

namespace org.cchmc.pho.api.ViewModels
{
    public class PatientViewModel
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PracticeID { get; set; }       
        public int PCP_StaffID { get; set; }
        public DateTime? DOB { get; set; }        
        public PatientStatusViewModel Status { get; set; }
        public DateTime? LastEDVisit { get; set; }
        public bool Chronic { get; set; }
        public bool WatchFlag { get; set; }            
        public List<PatientConditionViewModel> Conditions { get; set; }
    }

    public class PatientStatusViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class PatientConditionViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
