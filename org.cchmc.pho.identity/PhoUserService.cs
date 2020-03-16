using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using org.cchmc.pho.core.Models;
using org.cchmc.pho.identity.EntityModels;
using org.cchmc.pho.identity.Interfaces;
using org.cchmc.pho.identity.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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
                _logger.LogInformation($"User {userName} attempting to log in.");
                User user = await GetUser(userName);
                if (user == null || user.IsPending || user.IsDeleted)
                {
                    string reason = "there is no such user";
                    if(user != null)
                    {
                        if (user.IsPending)
                            reason = "the user is pending activation";
                        else reason = "the user is deleted";
                    }
                    _logger.LogInformation($"User {userName} unable to log in because { reason }");
                    return null;
                }

                var login = await _context.Login.FirstOrDefaultAsync(l => l.UserName == userName);
                string hashedPassword = login.Password;

                PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, password);
                if (result == PasswordVerificationResult.Failed)
                {
                    int loginAttempts = await IncrementLockoutAttempts(login);
                    _logger.LogInformation($"User {userName} verification failure, {loginAttempts} consecutive failure(s).");
                    if (loginAttempts >= _customOptions.MaximumAttemptsBeforeLockout)
                    {
                        await LockoutUser(login);
                    }
                    return null;
                }

                Task resetLockoutTask = ResetLockoutAttempts(login);

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

                // This refresh token will get overwritten if a user logs in with more than one device. Per Brad, this is acceptable.
                // Refresh tokens are only valid while the token itself is valid, so we don't need a separate refresh token expiration.
                user.RefreshToken = await GenerateAndWriteRefreshToken(login);

                await resetLockoutTask;

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<User> Refresh(string tokenString, string refreshToken)
        {
            try
            {
                if (!_tokenHandler.CanReadToken(tokenString))
                    return null;

                JwtSecurityToken myToken = _tokenHandler.ReadJwtToken(tokenString);
                int userId = GetUserIdFromClaims(myToken.Claims);
                string userName = GetUserNameFromClaims(myToken.Claims);

                _logger.LogInformation($"User {userName} attempting to refresh token.");

                if(myToken.ValidTo < DateTime.Now)
                {
                    _logger.LogInformation($"User {userName} unable to refresh because current token is expired.");
                    return null;
                }

                User user = await GetUser(userId);
                if (user == null || user.IsPending || user.IsDeleted)
                {
                    string reason = "there is no such user";
                    if (user != null)
                    {
                        if (user.IsPending)
                            reason = "the user is pending activation";
                        else reason = "the user is deleted";
                    }
                    _logger.LogInformation($"User {userName} unable to refresh because { reason }");
                    return null;
                }

                if (user.RefreshToken != refreshToken)
                {
                    _logger.LogInformation($"User {userName} unable to refresh because refresh tokens don't match.");
                    return null;
                }

                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
                identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
                identity.AddClaim(new Claim(ClaimTypes.UserData, user.StaffId.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Role, user.Role?.Name));

                myToken = new JwtSecurityToken(_jwtConfig.ValidIssuer, _jwtConfig.ValidAudience, identity.Claims,
                    null, DateTime.Now.AddHours(_tokenExpirationInHours), _signingCredentials);

                user.Token = _tokenHandler.WriteToken(myToken);

                var login = await _context.Login.FirstOrDefaultAsync(l => l.UserName == userName);

                // This refresh token will get overwritten if a user logs in with more than one device. Per Brad, this is acceptable.
                user.RefreshToken = await GenerateAndWriteRefreshToken(login);

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

                return await GetUserInternal(user);
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

                return await GetUserInternal(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        private async Task<User> GetUserInternal(Login login)
        {
            var userType = await _context.TlkUserType.FirstOrDefaultAsync(ut => ut.Id == login.UserTypeId);
            if (userType == null)
            {
                _logger.LogInformation($"User '{login.UserName}' is not linked to an actual UserType record.");
                return null;
            }

            // The Login table does not foreign key to the Staff table so we need to get that data separately
            var staffData = await _context.Staff.FirstOrDefaultAsync(s => s.Id == login.StaffId);

            return login.BuildUser(staffData, userType);
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
                var hashedPassword = _passwordHasher.HashPassword(user, Guid.NewGuid().ToString());
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
                    Password = hashedPassword
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

        public int GetStaffIdFromClaims(IEnumerable<Claim> claims)
        {
            try
            {
                if (claims.Any(p => p.Type == ClaimTypes.UserData))
                    return int.Parse(claims.First(p => p.Type == ClaimTypes.UserData).Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return 0;
        }

        public int GetUserIdFromClaims(IEnumerable<Claim> claims)
        {
            try
            {
                if (claims.Any(p => p.Type == ClaimTypes.NameIdentifier))
                    return int.Parse(claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return 0;
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
                _logger.LogInformation($"User {userNameMakingChange} attempting to change password for user {userRecord.UserName}.");

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
                _logger.LogInformation($"User {userNameMakingChange} unlocking user {userRecord.UserName}.");

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
                    _logger.LogInformation($"User {userNameMakingChange} logically deleting user {userRecord.UserName}.");
                    userRecord.DeletedBy = userNameMakingChange;
                    userRecord.DeletedDate = DateTime.Now;
                }
                else if (!shouldDelete && userRecord.DeletedFlag.GetValueOrDefault(false))
                {
                    _logger.LogInformation($"User {userNameMakingChange} logically undeleting user {userRecord.UserName}.");
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
            _logger.LogInformation($"User {user.UserName} is locked out after {_customOptions.MaximumAttemptsBeforeLockout} attempts.");
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

        private async Task<string> GenerateAndWriteRefreshToken(Login user)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                user.RefreshToken = Convert.ToBase64String(randomNumber);
            }

            _context.Login.Update(user);
            await _context.SaveChangesAsync();

            return user.RefreshToken;
        }
    }
}
