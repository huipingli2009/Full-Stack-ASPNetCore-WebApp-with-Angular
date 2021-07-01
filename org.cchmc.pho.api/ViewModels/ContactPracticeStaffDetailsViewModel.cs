namespace org.cchmc.pho.api.ViewModels
{
    public class ContactPracticeStaffDetailsViewModel
    {
        public int Id { get; set; }
        public string StaffName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; }
        public int? NPI { get; set; }
        public string Locations { get; set; }
        public string Specialty { get; set; }
        public bool CommSpecialist { get; set; }
        public bool OVPCAPhysician { get; set; }
        public bool OVPCAMidLevel { get; set; }
        public string Responsibilities { get; set; }
        public string BoardMembership { get; set; }
        public string NotesAboutProvider { get; set; }
    }
}
