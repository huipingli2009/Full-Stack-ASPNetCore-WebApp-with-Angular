using org.cchmc.pho.identity.Models;

namespace org.cchmc.pho.identity.EntityModels
{
    public partial class Login
    {
        public User BuildUser(Staff staff, TlkUserType userType)
        {
            return new User()
            {
                CreatedBy = CreatedBy,
                CreatedDate = CreatedOnDate,
                DeactivatedBy = DeletedBy,
                DeactivatedDate = DeletedDate,
                Email = Email,
                FirstName = staff?.FirstName,
                Id = Id,
                IsPending = PendingFlag.GetValueOrDefault(false),
                LastName = staff?.LastName,
                LastUpdatedBy = ModifiedBy,
                LastUpdatedDate = ModifiedDate,
                Role = userType.BuildRole(),
                UserName = UserName,
                IsDeleted = DeletedFlag.GetValueOrDefault(false),
                IsLockedOut = LockoutFlag.GetValueOrDefault(false),
                StaffId = StaffId,
                RefreshToken = RefreshToken
            };
        }
    }
}
