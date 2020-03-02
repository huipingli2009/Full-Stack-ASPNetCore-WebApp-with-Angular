using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace org.cchmc.pho.identity.Models
{
    public class JwtAuthentication
    {
        [Required]
        public string SecurityKey { get; set; }
        [Required]
        public string ValidIssuer { get; set; }
        [Required]
        public string ValidAudience { get; set; }
        [Required]
        public int TokenExpirationInHours { get; set; }

        public SymmetricSecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecurityKey));
        public SigningCredentials SigningCredentials => new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
    }
}
