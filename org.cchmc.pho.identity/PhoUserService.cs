using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using org.cchmc.pho.core.Models;
using org.cchmc.pho.core.Settings;
using org.cchmc.pho.identity.EntityModels;
using org.cchmc.pho.identity.Interfaces;
using org.cchmc.pho.identity.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace org.cchmc.pho.identity
{
    public class PhoUserService : IUserService
    {
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly ILogger<PhoUserService> _logger;
        private readonly int _tokenExpirationInHours;
        private readonly JwtAuthentication _jwtConfig;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly PHOIdentityContext _context;
        private readonly CustomOptions _customOptions;

        public PhoUserService(IOptions<JwtAuthentication> jwtConfig, PHOIdentityContext context,
            IPasswordHasher<User> passwordHasher, IOptions<CustomOptions> customOptions, ILogger<PhoUserService> logger)
        {
            var key = jwtConfig.Value.SymmetricSecurityKey;
            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            _tokenHandler = new JwtSecurityTokenHandler();
            _logger = logger;
            _tokenExpirationInHours = jwtConfig.Value.TokenExpirationInHours;
            _jwtConfig = jwtConfig.Value;
            _passwordHasher = passwordHasher;
            _context = context;
            _customOptions = customOptions.Value;
        }

        public async Task<User> Authenticate(string userName, string password)
        {
            try
            {
                User user = await GetUser(userName);
                if (user == null || user.IsPending || user.IsDeleted)
                    return null;

                var login = await _context.Login.FirstOrDefaultAsync(l => l.UserName == userName);
                string hashedPassword = login.Password;

                PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, password);
                if (result == PasswordVerificationResult.Failed)
                {
                    int loginAttempts = await IncrementLockoutAttempts(login);
                    if (loginAttempts >= _customOptions.MaximumAttemptsBeforeLockout)
                        await LockoutUser(login);
                    return null;
                }

                await ResetLockoutAttempts(login);

                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
                identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
                identity.AddClaim(new Claim(ClaimTypes.UserData, user.StaffId.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Role, user.Role?.Name));

                var token = new JwtSecurityToken(_jwtConfig.ValidIssuer, _jwtConfig.ValidAudience, identity.Claims,
                    null, DateTime.Now.AddHours(_tokenExpirationInHours), _signingCredentials);

                user.Token = _tokenHandler.WriteToken(token);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<User> GetUser(int userId)
        {
            try
            {
                var user = await _context.Login.FirstOrDefaultAsync(p => p.Id == userId);
                if (user == null)
                    return null;

                var userType = await _context.TlkUserType.FirstOrDefaultAsync(ut => ut.Id == user.Id);
                if (userType == null)
                {
                    _logger.LogInformation($"User '{user.UserName}' is not linked to an actual UserType record.");
                    return null;
                }

                // The Login table does not foreign key to the Staff table so we need to get that data separately
                var staffData = await _context.Staff.FirstOrDefaultAsync(s => s.Id == user.StaffId);
                if (staffData == null)
                {
                    _logger.LogInformation($"User '{user.UserName}' is not linked to an actual Staff record.");
                    return null;
                }

                return user.BuildUser(staffData, userType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<User> GetUser(string userName)
        {
            try
            {
                var user = await _context.Login.FirstOrDefaultAsync(p => p.UserName == userName);
                if (user == null)
                    return null;

                var userType = await _context.TlkUserType.FirstOrDefaultAsync(ut => ut.Id == user.Id);
                if (userType == null)
                {
                    _logger.LogInformation($"User '{userName}' is not linked to an actual UserType record.");
                    return null;
                }

                // The Login table does not foreign key to the Staff table so we need to get that data separately
                var staffData = await _context.Staff.FirstOrDefaultAsync(s => s.Id == user.StaffId);
                if(staffData == null)
                {
                    _logger.LogInformation($"User '{userName}' is not linked to an actual Staff record.");
                    return null;
                }

                return user.BuildUser(staffData, userType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<User> UpdateUser(User user, string userNameMakingChange)
        {
            try
            {
                var userRecord = await _context.Login.FirstOrDefaultAsync(p => p.Id == user.Id);
                if (userRecord == null)
                {
                    _logger.LogInformation($"User {userNameMakingChange} tried to update user id {user.Id}, but that user does not exist.");
                    return null;
                }

                userRecord.Email = user.Email;
                userRecord.ModifiedBy = userNameMakingChange;
                userRecord.ModifiedDate = DateTime.Now;
                userRecord.UserTypeId = user.Role.Id;

                _context.Login.Update(userRecord);
                await _context.SaveChangesAsync();

                return await GetUser(userRecord.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<User> InsertUser(User user, string userNameMakingChange)
        {
            try
            {
                Login userRecord = new Login()
                {
                    CreatedBy = userNameMakingChange,
                    CreatedOnDate = DateTime.Now,
                    PendingFlag = true,
                    StaffId = 0,
                    DeletedFlag = user.IsDeleted,
                    Email = user.Email,
                    ModifiedBy = userNameMakingChange,
                    ModifiedDate = DateTime.Now,
                    UserName = user.UserName,
                    UserTypeId = user.Role.Id,
                    Password = _passwordHasher.HashPassword(user, Guid.NewGuid().ToString())
                };

                _context.Login.Add(userRecord);
                await _context.SaveChangesAsync();

                return await GetUser(userRecord.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<List<Role>> ListRoles()
        {
            try
            {
                var userTypes = await _context.TlkUserType.ToListAsync();
                return userTypes.Select(ut => ut.BuildRole()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public string GetRoleNameFromClaims(IEnumerable<Claim> claims)
        {
            try
            {
                return claims.FirstOrDefault(p => p.Type == ClaimTypes.Role)?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
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
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<bool> ResetUserPassword(int userId, string newPassword, string userNameMakingChange)
        {
            try
            {
                var userRecord = await _context.Login.FirstOrDefaultAsync(l => l.Id == userId);
                if (userRecord == null)
                    return false;
                userRecord.Password = _passwordHasher.HashPassword(await GetUser(userId), newPassword);
                userRecord.ModifiedBy = userNameMakingChange;
                userRecord.ModifiedDate = DateTime.Now;
                _context.Login.Update(userRecord);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task<User> AssignStaffIdToUser(int userId, int staffId, string userNameMakingChange)
        {
            try
            {
                var userRecord = await _context.Login.FirstOrDefaultAsync(p => p.Id == userId);
                if (userRecord == null)
                {
                    _logger.LogInformation($"User {userNameMakingChange} tried to assign staff id {staffId} user id {userId}, but that user does not exist.");
                    return null;
                }

                userRecord.StaffId = staffId;
                userRecord.ModifiedBy = userNameMakingChange;
                userRecord.ModifiedDate = DateTime.Now;
                userRecord.PendingFlag = false;

                _context.Login.Update(userRecord);
                await _context.SaveChangesAsync();

                return await GetUser(userRecord.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<User> RemoveLockoutFromUser(int userId, string userNameMakingChange)
        {
            try
            {
                var userRecord = await _context.Login.FirstOrDefaultAsync(p => p.Id == userId);
                if (userRecord == null)
                {
                    _logger.LogInformation($"User {userNameMakingChange} tried to unlock user id {userId}, but that user does not exist.");
                    return null;
                }

                userRecord.LockoutFlag = false;
                userRecord.AccessFailedCount = 0;
                userRecord.ModifiedBy = userNameMakingChange;
                userRecord.ModifiedDate = DateTime.Now;
                userRecord.PendingFlag = false;

                _context.Login.Update(userRecord);
                await _context.SaveChangesAsync();

                return await GetUser(userRecord.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<User> ToggleDeleteOnUser(int userId, bool shouldDelete, string userNameMakingChange)
        {
            try
            {
                var userRecord = await _context.Login.FirstOrDefaultAsync(p => p.Id == userId);
                if (userRecord == null)
                {
                    _logger.LogInformation($"User {userNameMakingChange} tried to toggle delete on user id {userId}, but that user does not exist.");
                    return null;
                }

                if (shouldDelete && !userRecord.DeletedFlag.GetValueOrDefault(false))
                {
                    userRecord.DeletedBy = userNameMakingChange;
                    userRecord.DeletedDate = DateTime.Now;
                }
                else if (!shouldDelete && userRecord.DeletedFlag.GetValueOrDefault(false))
                {
                    userRecord.DeletedBy = null;
                    userRecord.DeletedDate = null;
                }
                userRecord.DeletedFlag = shouldDelete;

                _context.Login.Update(userRecord);
                await _context.SaveChangesAsync();

                return await GetUser(userRecord.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        private async Task LockoutUser(Login user)
        {
            user.LockoutFlag = true;
            _context.Login.Update(user);
            await _context.SaveChangesAsync();
        }

        private async Task<int> IncrementLockoutAttempts(Login user)
        {
            user.AccessFailedCount = user.AccessFailedCount.GetValueOrDefault(0) + 1;
            _context.Login.Update(user);
            await _context.SaveChangesAsync();
            return user.AccessFailedCount.GetValueOrDefault(0);
        }

        private async Task ResetLockoutAttempts(Login user)
        {
            user.AccessFailedCount = 0;
            _context.Login.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
