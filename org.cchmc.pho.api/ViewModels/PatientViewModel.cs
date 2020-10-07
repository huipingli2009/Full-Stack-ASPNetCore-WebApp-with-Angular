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
        public int GenderId { get; set; }
        public bool ActiveStatus { get; set; }
        public bool PendingStatusConfirmation { get; set; }
        public DateTime? LastEDVisit { get; set; }
        public bool Chronic { get; set; }
        public bool WatchFlag { get; set; }
        public int TotalRecords { get; set; }
        public List<PatientConditionViewModel> Conditions { get; set; }
        public int OutcomeMetricFilterId { get; set; }       
    }


    public class PatientConditionViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class PatientInsuranceViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }   
}
