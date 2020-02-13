using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using org.cchmc.pho.identity.Interfaces;
using org.cchmc.pho.identity.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace org.cchmc.pho.identity
{
    public class UserService : IUserService
    {
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly string _connectionString;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly int _tokenExpirationInHours;

        public UserService(string connectionString, IOptions<JwtAuthentication> jwtConfig,
            UserManager<User> userManager, SignInManager<User> signInManager)
        {
            var key = jwtConfig.Value.SymmetricSecurityKey;
            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            _connectionString = connectionString;
            _tokenHandler = new JwtSecurityTokenHandler();
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenExpirationInHours = jwtConfig.Value.TokenExpirationInHours;
        }

        public async Task<User> Authenticate(string userName, string password)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return null;

                if (!await _userManager.CheckPasswordAsync(user, password))
                    return null;

                // get the users roles to add them to the claim list
                var roles = new List<string>(await _userManager.GetRolesAsync(user));

                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
                identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
                roles?.ForEach(p => identity.AddClaim(new Claim(ClaimTypes.Role, p)));

                var token = new JwtSecurityToken("PHO", "PHO", identity.Claims, null, DateTime.Now.AddHours(_tokenExpirationInHours), _signingCredentials);
                
                // This token is not getting written to the database, just returned in the object
                user.Token = new JwtSecurityTokenHandler().WriteToken(token);
                return user;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
