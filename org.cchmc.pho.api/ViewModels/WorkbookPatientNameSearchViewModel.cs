using System;

namespace org.cchmc.pho.api.ViewModels
{
    public class WorkbookPatientNameSearchViewModel
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DOB { get; set; }
        public string Phone { get; set; }
    }
}
