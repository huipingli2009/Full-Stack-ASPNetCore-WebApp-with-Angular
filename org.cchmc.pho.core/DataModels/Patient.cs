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
        public DateTime? DOB { get; set; }
        public int ActiveStatus { get; set; }
        public DateTime? LastEDVisit { get; set; }
        public int Chronic { get; set; }
        public int WatchFlag { get; set; }
        public string SortColumn { get; set; }
        public string Conditions { get; set; }
        public string ConditionIDs { get; set; }
    }
}
