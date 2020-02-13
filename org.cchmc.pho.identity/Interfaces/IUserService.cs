
using org.cchmc.pho.identity.Models;
using System.Threading.Tasks;

namespace org.cchmc.pho.identity.Interfaces
{
    public interface IUserService
    {
        Task<User> Authenticate(string userName, string password);
    }
}
