using System;

namespace org.cchmc.pho.api.ViewModels
{
    public class SimplifiedPatientViewModel
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DOB { get; set; }
        public string Phone { get; set; }
    }


    public class DuplicatePatientViewModel : SimplifiedPatientViewModel
    {
        public string Gender { get; set; }
        public int GenderId { get; set; }
        public int PCPId { get; set; }
        public string HeaderText { get; set; }
        public string DetailHeaderText { get; set; }
        public int MatchType { get; set; }
        public bool AllowContinue { get; set; }
        public bool AllowReactivate { get; set; }
        public bool AllowKeepAndSave { get; set; }
        public bool AllowMerge { get; set; }
    }
}
