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
}