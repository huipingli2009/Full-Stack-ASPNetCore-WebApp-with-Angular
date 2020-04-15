using System;
using System.Collections.Generic;

namespace org.cchmc.pho.api.ViewModels
{
    public class StaffDetailViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? DeletedFlag { get; set; }
        public int? PositionId { get; set; }
        public int? CredentialId { get; set; }
        public int? NPI { get; set; }
        public bool IsLeadPhysician { get; set; }
        public bool IsQITeam { get; set; }
        public bool IsPracticeManager { get; set; }
        public bool IsInterventionContact { get; set; }
        public bool IsQPLLeader { get; set; }
        public bool IsPHOBoard { get; set; }
        public bool IsOVPCABoard { get; set; }
        public bool IsRVPIBoard { get; set; }
        public List<LocationViewModel> Locations { get; set; }
    }
    public class StaffAdmin
    {
        public string Id { get; set; }
        public string DeletedFlag { get; set; }
        public string EndDate { get; set; }
    }

}
