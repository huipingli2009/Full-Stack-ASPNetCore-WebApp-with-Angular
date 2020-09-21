
namespace org.cchmc.pho.api.ViewModels
{
    public class WorkbooksDepressionPatientViewModel: WorkbooksPatient
    {       
        public string PHQ9_Score { get; set; }
        public bool ActionFollowUp { get; set; }
        public string Improvement { get; set; }
        public bool FollowUpResponse { get; set; }
        public bool AllProvidersConfirmed { get; set; }
        public bool NoPatientsConfirmed { get; set; }
    }
}
