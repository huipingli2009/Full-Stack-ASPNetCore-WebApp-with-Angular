using System;
namespace org.cchmc.pho.core.DataModels
{
    public class WorkbooksPatientFollowup
    {
        public int FormResponseId { get; set; }
        public int PatientId { get; set; }
        public bool ActionPlanGiven { get; set; }
        public bool ManagedByExternalProvider { get; set; }
        public DateTime? DateOfLastCommunicationByExternalProvider { get; set; }
        public bool FollowupPhoneCallOneToTwoWeeks { get; set; }
        public DateTime? DateOfFollowupCall { get; set; }
        public bool OneMonthFollowupVisit { get; set; }
        public DateTime? DateOfOneMonthVisit { get; set; }
        public int OneMonthFolllowupPHQ9Score { get; set; }
        public bool Improvement { get; set; }

    }
}
