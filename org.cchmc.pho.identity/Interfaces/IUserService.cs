
using org.cchmc.pho.identity.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace org.cchmc.pho.identity.Interfaces
{
    public interface IUserService
    {
        Task<User> AssignStaffIdToUser(int userId, int staffId, string userNameMakingChange); 
        Task<User> Authenticate(string userName, string password);
        string GetRoleNameFromClaims(IEnumerable<Claim> claims);
        string GetUserNameFromClaims(IEnumerable<Claim> claims);
        Task<User> GetUser(int userId);
        Task<User> GetUser(string userName);
        Task<User> InsertUser(User user, string userNameMakingChange);
        Task<List<Role>> ListRoles();
        Task<User> RemoveLockoutFromUser(int userId, string userNameMakingChange);
        Task<bool> ResetUserPassword(int userId, string newPassword, string userNameMakingChange);
        Task<User> ToggleDeleteOnUser(int userId, bool shouldDelete, string userNameMakingChange);
        Task<User> UpdateUser(User user, string userNameMakingChange);
    }
}
