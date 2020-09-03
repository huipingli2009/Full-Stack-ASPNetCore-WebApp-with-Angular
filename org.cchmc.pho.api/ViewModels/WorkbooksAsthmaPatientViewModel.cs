
namespace org.cchmc.pho.api.ViewModels
{
    public class WorkbooksAsthmaPatientViewModel: WorkbooksPatient
    {
        public bool AssessmentCompleted { get; set; }
        public string Treatment { get; set; }
        public bool ActionPlanGiven { get; set; }
        public string Asthma_Score { get; set; }
    }
}
