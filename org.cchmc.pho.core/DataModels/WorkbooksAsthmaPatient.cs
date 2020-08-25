using System;
using System.Collections.Generic;
using System.Text;

namespace org.cchmc.pho.core.DataModels
{
    public class WorkbooksAsthmaPatient
    {
        public int FormResponseId { get; set; }
        public int PatientId { get; set; }
        public string Patient { get; set; }
        public DateTime? DOB { get; set; }
        public string Phone { get; set; }
        public string Provider { get; set; }
        public int ProviderId { get; set; }
        public DateTime? DateOfService { get; set; }
        public bool AssessmentCompleted { get; set; }
        public string Treatment { get; set; }
        public bool ActionPlanGiven { get; set; }
        public string Asthma_Score { get; set; }
    }  
}
