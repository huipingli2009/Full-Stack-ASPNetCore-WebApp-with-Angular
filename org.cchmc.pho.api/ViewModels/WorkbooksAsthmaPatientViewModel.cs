﻿
namespace org.cchmc.pho.api.ViewModels
{
    public class WorkbooksAsthmaPatientViewModel: WorkbooksPatient
    {
        public bool AssessmentCompleted { get; set; }
        public AsthmaTreatmentPlanViewModel Treatment { get; set; }
        public bool ActionPlanGiven { get; set; }
        public int Asthma_Score { get; set; }
    }
}
