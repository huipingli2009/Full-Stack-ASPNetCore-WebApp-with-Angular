
namespace org.cchmc.pho.api.ViewModels
{
    public class StaffViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int PositionId { get; set; }
        public int CredentialId { get; set; }
        public bool IsRegistry { get; set; }
        public string Responsibilities { get; set; }
    }
}
