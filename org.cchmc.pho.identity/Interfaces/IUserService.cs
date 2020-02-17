
using org.cchmc.pho.identity.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace org.cchmc.pho.identity.Interfaces
{
    public interface IUserService
    {
        Task<User> Authenticate(string userName, string password);
        Task<bool> ChangeUserEmail(string userName, string newEmailAddress);
        Task<bool> ChangeUserRoles(string userName, List<string> newRoles); 
        Task<List<string>> GetRoles(string userName);
        List<string> GetRolesFromClaims(IEnumerable<Claim> claims);
        string GetUserNameFromClaims(IEnumerable<Claim> claims);
        Task<User> GetUserByEmail(string emailAddress);
        Task<User> GetUserByUserName(string userName);
        Task<List<string>> ListRoles();
        Task<bool> ResetUserPassword(string userName, string newPassword);
        Task<List<string>> ValidatePassword(string userName, string password);
    }
}
