
using System;

namespace org.cchmc.pho.core.DataModels
{
    public class Staff
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
        public DateTime? LegalDisclaimerSigned { get; set; }
        public Practice MyPractice { get; set; }
    }
}
