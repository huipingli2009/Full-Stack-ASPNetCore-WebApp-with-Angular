using System;

namespace org.cchmc.pho.core.DataModels
{
    public class SimplifiedPatient
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DOB { get; set; }
        public string Phone { get; set; }
    }

    public class DuplicatePatient : SimplifiedPatient
    {
        public string Gender { get; set; }
        public int GenderId { get; set; }
        public int PCPId { get; set; }
        public string PatientMRNId { get; set; }
        public string HeaderText { get; set; }
        public string DetailHeaderText { get; set; }
        public int MatchType { get; set; }
        public bool AllowContinue { get; set; }
        public bool AllowReactivate { get; set; }
        public bool AllowKeepAndSave { get; set; }
        public bool AllowMerge { get; set; }
    }

    public class MergeConfirmation
    {            
        public int MatchType { get; set; }
        public int MergeAction { get; set; }
        public int TopPatientId { get; set; }
        public int BottomPatientId { get; set; }
        public int PCPStaffId { get; set; }
        public string TopPatientFirstName { get; set; }
        public string TopPatientLastName { get; set; }
        public string TopPatientDOB { get; set; }
        public int TopPatientGenderId { get; set; }
    }
}