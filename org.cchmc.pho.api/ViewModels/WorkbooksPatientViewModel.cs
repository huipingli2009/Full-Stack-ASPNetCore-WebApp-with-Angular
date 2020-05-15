using System;

namespace org.cchmc.pho.api.ViewModels
{
    public class WorkbooksPatientViewModel
    {
        public int FormResponseId { get; set; }
        public int PatientId { get; set; }
        public string Patient { get; set; }
        public DateTime? DOB { get; set; }
        public string Phone { get; set; }
        public string Provider { get; set; }
        public int ProviderId { get; set; }
        public DateTime? DateOfService { get; set; }
        public string PHQ9_Score { get; set; }
        public bool ActionFollowUp { get; set; }
        public string Improvement { get; set; }
        public bool FollowUpResponse { get; set; }
    }
}
