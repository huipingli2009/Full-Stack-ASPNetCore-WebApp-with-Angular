using Microsoft.AspNetCore.Identity;

namespace org.cchmc.pho.identity.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
    }
}
