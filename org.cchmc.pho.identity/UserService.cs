using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using org.cchmc.pho.identity.Interfaces;
using org.cchmc.pho.identity.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace org.cchmc.pho.identity
{
    public class UserService : IUserService
    {
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserService> _logger;
        //private readonly SignInManager<User> _signInManager; Is this even needed?
        private readonly int _tokenExpirationInHours;
        private readonly JwtAuthentication _jwtConfig;

        public UserService(IOptions<JwtAuthentication> jwtConfig,
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            ILogger<UserService> logger)
        {
            var key = jwtConfig.Value.SymmetricSecurityKey;
            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            _tokenHandler = new JwtSecurityTokenHandler();
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _tokenExpirationInHours = jwtConfig.Value.TokenExpirationInHours;
            _jwtConfig = jwtConfig.Value;
        }

        public async Task<User> Authenticate(string userName, string password)
        {
            try
            {
                User user = await _userManager.FindByNameAsync(userName);
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

                var token = new JwtSecurityToken(_jwtConfig.ValidIssuer, _jwtConfig.ValidAudience, identity.Claims,
                    null, DateTime.Now.AddHours(_tokenExpirationInHours), _signingCredentials);
                
                // This token is intentionally not getting written to the database, just returned in the object
                user.Token = _tokenHandler.WriteToken(token);

                return user;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<User> GetUserByEmail(string emailAddress)
        {
            try
            {
                return await _userManager.FindByEmailAsync(emailAddress);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            try
            {
                return await _userManager.FindByNameAsync(userName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<List<string>> GetRoles(string userName)
        {
            try
            {
                User user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                    return new List<string>(await _userManager.GetRolesAsync(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return new List<string>();
        }

        public List<string> GetRolesFromClaims(IEnumerable<Claim> claims)
        {
            try
            {
                return claims.Where(p => p.Type == ClaimTypes.Role).Select(p => p.Value).ToList();
            }
            catch(Exception ex)
            {
                return new List<string>();
            }
        }

        public string GetUserNameFromClaims(IEnumerable<Claim> claims)
        {
            try
            {
                return claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<string>> ValidatePassword(string userName, string password)
        {
            var listOfErrors = new List<string>();
            try
            {
                User user = await GetUserByUserName(userName);
                foreach(var validator in _userManager.PasswordValidators)
                {
                    var result = await validator.ValidateAsync(_userManager, user, password);
                    if (result.Errors.Any())
                        listOfErrors.AddRange(result.Errors.Select(p => p.Description));
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                listOfErrors.Add("An error occurred.");
            }
            return listOfErrors;
        }

        public async Task<bool> ResetUserPassword(string userName, string newPassword)
        {
            try
            {
                // get the user
                User user = await GetUserByUserName(userName);
                if (user == null)
                    return false;

                // reset the user to get the token
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                // use the token to change the password
                IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
                if (result.Errors.Any())
                    _logger.LogInformation($"Unable to reset password for user {userName}, {result.Errors.Count()} errors. First: {result.Errors.First()}");
                return result.Succeeded;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task<bool> ChangeUserRoles(string userName, List<string> newRoles)
        {
            try
            {
                foreach(var r in newRoles)
                {
                    if(!_roleManager.Roles.Any(p => string.Equals(p.Name, r)))
                    {
                        _logger.LogWarning($"Tried to add unknown role {r} to user {userName}.");
                        return false;
                    }
                }

                Task<User> userTask = GetUserByUserName(userName);
                Task<List<string>> currentRolesTask = GetRoles(userName);

                if (userTask.Result == null)
                    return false;

                List<string> rolesToAdd = newRoles.Where(p => !currentRolesTask.Result.Contains(p)).ToList();
                List<string> rolesToRemove = currentRolesTask.Result.Where(p => !newRoles.Contains(p)).ToList();

                var localSuccess = true;

                if (rolesToAdd.Any())
                {
                    IdentityResult addResult = await _userManager.AddToRolesAsync(userTask.Result, rolesToAdd);
                    localSuccess = addResult.Succeeded;
                }

                if(localSuccess && rolesToRemove.Any())
                    return (await _userManager.RemoveFromRolesAsync(userTask.Result, rolesToRemove)).Succeeded;
                return localSuccess;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task<bool> ChangeUserEmail(string userName, string newEmailAddress)
        {
            try
            {
                // get the user
                User user = await GetUserByUserName(userName);
                if (user == null)
                    return false;

                // reset the user to get the token
                string resetToken = await _userManager.GenerateChangeEmailTokenAsync(user, newEmailAddress);

                // use the token to change the password
                IdentityResult result = await _userManager.ChangeEmailAsync(user, newEmailAddress, resetToken);
                if (result.Errors.Any())
                    _logger.LogInformation($"Unable to reset email for user {userName}, {result.Errors.Count()} errors. First: {result.Errors.First()}");
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task<List<string>> ListRoles()
        {
            try
            {
                return _roleManager.Roles.Select(p => p.Name).ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new List<string>();
            }
        }

        /* TODOS:
         * - Associating users to their data in PHO
         *      Best bet here I think is to use email. Identity has a "FindByEmail" method. If the user changes their email in
         *      the Staff table, prior to pushing the change into the Staff table, we need to get the previous email from the Staff
         *      table to use to look up their Identity user.
         * - Regarding role changes, how should the app pick up role changes for users without forcing login/logout?
         *      Maybe a "reauthenticate" method using the token instead of username/password?
         * - Regarding password changes, how should the app treat the user if they're logged in? Should we be storing these tokens
         *      in the Identity store and validating against them each time?
         */
    }
}
