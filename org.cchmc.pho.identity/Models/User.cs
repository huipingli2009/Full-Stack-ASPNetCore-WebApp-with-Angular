using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace org.cchmc.pho.identity.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int StaffId { get; set; }
        public bool IsPending { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; } 
        public DateTime? LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public string DeactivatedBy { get; set; }
        public Role Role { get; set; }
    }
}
