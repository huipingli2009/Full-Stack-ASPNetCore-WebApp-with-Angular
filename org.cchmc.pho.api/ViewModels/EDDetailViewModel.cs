using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class EDDetailViewModel
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
    }
}
