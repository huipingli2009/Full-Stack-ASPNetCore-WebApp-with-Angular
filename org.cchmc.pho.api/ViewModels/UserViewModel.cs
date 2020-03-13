using System;

namespace org.cchmc.pho.api.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string NewPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsPending { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLockedOut { get; set; }
        public int StaffId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public string DeactivatedBy { get; set; }
        public RoleViewModel Role { get; set; }
    }
}
