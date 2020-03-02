using System;

namespace org.cchmc.pho.core.DataModels
{
    public class StaffDetail
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? StartDate { get; set; }
        public int PositionId { get; set; }
        public int CredentialId { get; set; }
        public bool LeadPhysician { get; set; }
        public bool QITeam { get; set; }
        public bool PracticeManager { get; set; }
        public bool InterventionContact { get; set; }
        public bool QPLLeader { get; set; }
        public bool PHOBoard { get; set; }
        public bool OVPCABoard { get; set; }
        public bool RVPIBoard { get; set; }
    }
}
