using System;

namespace org.cchmc.pho.core.DataModels
{
    public class WorkbookPatientNameSearch
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DOB { get; set; }
        public string Phone { get; set; }
    }
}