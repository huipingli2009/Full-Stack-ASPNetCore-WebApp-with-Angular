using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace org.cchmc.pho.core.DataModels
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PracticeID { get; set; }
        //public int SortCol { get; set; }
        public int PCP_StaffID { get; set; }
    }
}
