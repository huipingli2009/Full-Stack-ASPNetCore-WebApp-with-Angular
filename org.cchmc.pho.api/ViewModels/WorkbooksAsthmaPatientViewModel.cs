using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class WorkbooksAsthmaPatientViewModel: WorkbooksPatient
    {
        public string AssessmentCompleted { get; set; }
        public string Treatment { get; set; }
        public string ActionPlanGiven { get; set; }
        public string Asthma_Score { get; set; }
    }
}
