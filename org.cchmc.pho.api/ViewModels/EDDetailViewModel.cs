using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class EDDetailViewModel
    {
        public string PAT_MRN_ID { get; set; }
        public string PAT_ENC_CSN_ID { get; set; }
        public string Facility { get; set; }
        public string PatientName { get; set; }
        public DateTime? PatientDOB { get; set; }
        public string PCP { get; set; }
        public DateTime? HOSP_ADMSN_TIME { get; set; }
        public DateTime? HOSP_DISCH_TIME { get; set; }
        public string VisitType { get; set; }
        public string PrimaryDX { get; set; }
        public string PrimaryDX_10Code { get; set; }
        public string DX2 { get; set; }
        public string DX2_10Code { get; set; }
    }
}
