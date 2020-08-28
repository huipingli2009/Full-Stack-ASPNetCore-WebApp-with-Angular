using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class WorkbooksAsthmaPatientViewModel: WorkbooksPatient
    {
        public bool AssessmentCompleted { get; set; }
        public int Treatment { get; set; }
        public bool ActionPlanGiven { get; set; }
        public int Asthma_Score { get; set; }
    }
}
