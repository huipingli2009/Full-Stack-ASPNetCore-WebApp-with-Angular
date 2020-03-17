
using org.cchmc.pho.core.DataModels;
using System;

namespace org.cchmc.pho.api.ViewModels
{
    public class StaffViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Position Position { get; set; }
        public Credential Credentials { get; set; }
        public bool IsRegistry { get; set; }
        public string Responsibilities { get; set; }
        public DateTime? LegalDisclaimerSigned { get; set; }
        public Practice MyPractice { get; set; }
    }
}
