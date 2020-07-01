using System;

namespace org.cchmc.pho.core.DataModels
{
    public class EDDetail
    {
        public string PatientMRN { get; set; }
        public string PatientEncounterID { get; set; }
        public string Facility { get; set; }
        public string PatientName { get; set; }
        public DateTime? PatientDOB { get; set; }
        public string PCP { get; set; }
        public DateTime? HospitalAdmission { get; set; }
        public DateTime? HospitalDischarge { get; set; }
        public string VisitType { get; set; }
        public string PrimaryDX { get; set; }
        public string PrimaryDX_10Code { get; set; }
        public string DX2 { get; set; }
        public string DX2_10Code { get; set; }
        public string InpatientVisit { get; set; }
        public string EDVisitCount { get; set; }
        public string UCVisitCount { get; set; }
        public string AdmitCount { get; set; }
    }
}
