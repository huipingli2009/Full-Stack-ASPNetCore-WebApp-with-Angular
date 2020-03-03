using System;
using System.Collections.Generic;

namespace org.cchmc.pho.api.ViewModels
{
    public class PatientDetailsViewModel
    {
        public int Id { get; set; }
        public string PatientMRNId { get; set; }
        public string ClarityPatientId { get; set; }
        public int PracticeId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime? PatientDOB { get; set; }
        public int PCPId { get; set; }
        public string PCPFirstName { get; set; }
        public string PCPLastName { get; set; }
        public int InsuranceId { get; set; }
        public string InsuranceName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public List<PatientConditionViewModel> Conditions { get; set; }
        public int PMCAScore { get; set; }
        public int ProviderPMCAScore { get; set; }
        public string ProviderNotes { get; set; }
        public bool ActiveStatus { get; set; }
        public bool PotentiallyActiveStatus { get; set; }

        public int GenderId { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public int PracticeVisits { get; set; }
        public int CCHMCEncounters { get; set; }
        public int HealthBridgeEncounters { get; set; }
        public int UniqueDXs { get; set; }
        public int UniqueCPTCodes { get; set; }
        public DateTime? LastPracticeVisit { get; set; }
        public DateTime? LastCCHMCAdmit { get; set; }
        public DateTime? LastHealthBridgeAdmit { get; set; }
        public string LastDiagnosis { get; set; }
        public DateTime? CCHMCAppointment { get; set; }
    }
}
