﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.identity.Models;
using org.cchmc.pho.api.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Options;
using org.cchmc.pho.core.Models;
using org.cchmc.pho.identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Text.RegularExpressions;
using org.cchmc.pho.core.Interfaces;

namespace org.cchmc.pho.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly CustomOptions _customOptions;
        private readonly IStaff _staff;

        public UsersController(ILogger<UsersController> logger, IMapper mapper, IUserService userService, IOptions<CustomOptions> customOptions, IStaff staffDal)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _customOptions = customOptions.Value;
            _staff = staffDal;
        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        [SwaggerResponse(200, type: typeof(AuthenticationResult))]
        [SwaggerResponse(401, description: "User not found or password did not match")]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest userParam)
        {
            try
            {
                User user = await _userService.Authenticate(userParam.Username, userParam.Password);
                if (user == null) return Unauthorized(new AuthenticationResult { Status = "User not found or password did not match" });

                // manually mapping the refresh token so that we can ignore it in AutoMapper.
                UserViewModel outgoingUser = _mapper.Map<UserViewModel>(user);
                outgoingUser.RefreshToken = user.RefreshToken;
                return Ok(new AuthenticationResult { Status = "Authorized", User = outgoingUser });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPost("refresh")]
        [SwaggerResponse(200, type: typeof(AuthenticationResult))]
        [SwaggerResponse(401, description: "Token not valid")]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> Refresh([FromBody] AuthenticationRefreshRequest refreshRequest)
        {
            try
            {
                // don't have to check the token string here, we do that in the service
                if (refreshRequest == null || string.IsNullOrWhiteSpace(refreshRequest.RefreshToken))
                    return Unauthorized("Token not valid");

                int userId = _userService.GetUserIdFromClaims(User?.Claims);

                User user = await _userService.Refresh(userId, refreshRequest.RefreshToken);
                if (user == null) return Unauthorized(new AuthenticationResult { Status = "Token not valid" });

                // manually mapping the refresh token so that we can ignore it in AutoMapper.
                UserViewModel outgoingUser = _mapper.Map<UserViewModel>(user);
                outgoingUser.RefreshToken = user.RefreshToken;
                return Ok(new AuthenticationResult { Status = "Authorized", User = outgoingUser });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("verbiage")]
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> PasswordVerbiage()
        {
            try
            {
                List<string> rules = new List<string>();
                if (_customOptions.MinimumPasswordLength > 0)
                    rules.Add($"Passwords must have a minimum length of {_customOptions.MinimumPasswordLength} characters and cannot contain spaces.");
                if (_customOptions.RequireDigit)
                    rules.Add("Password must contain a digit.");
                if (_customOptions.RequireLowercase)
                    rules.Add("Password must have a lowercase character.");
                if (_customOptions.RequireUppercase)
                    rules.Add("Password must have an uppercase character.");
                if (_customOptions.RequireNonAlphaNumeric)
                    rules.Add("Password must have one special character (such as punctuation).");

                return Ok(string.Join(' ', rules));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }
        [Authorize(Roles = "Practice Member,Practice Coordinator,Practice Admin,PHO Member,PHO Admin, PHO Leader")]
        [HttpGet("{staffId}")] // put because we're getting a specific user
        [SwaggerResponse(200, type: typeof(UserViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(401, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetUser(int staffId)
        {
            try
            {
                // validate the user provided is a user
                User user = await _userService.GetUserByStaffId(staffId);
                string currentUserName = _userService.GetUserNameFromClaims(User?.Claims);
                if (user == null)
                    return NotFound("User does not exist.");

                string currentUserRole = _userService.GetRoleNameFromClaims(User?.Claims);
                // if we're getting a different user, check some additional rules based on roles
                if (!string.Equals(currentUserName, user.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!InAnyAdminRole(currentUserRole))
                    {
                        // if you're not in an Admin role, you can't set another person's details
                        return BadRequest("Cannot get another user's details.");
                    }

                    if (IsPracticeAdmin(currentUserRole))
                    {
                        // if you're a practice admin, you can only get another user if they're in your practice
                        int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                        if (!_staff.IsStaffInSamePractice(currentUserId, user.StaffId))
                        {
                            _logger.LogInformation($"{currentUserName} tried to get info for staff id {staffId}, but the caller is in another practice.");
                            return BadRequest("Cannot get users in another practice.");
                        }
                    }
                }

                return Ok(_mapper.Map<UserViewModel>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [HttpPatch("{userId}/password")] // patch because we're only updating password
        [SwaggerResponse(200, type: typeof(bool))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(401, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdateUserPassword(int userId, [FromBody] PasswordChangeViewModel newPassword)
        {
            try
            {
                if (newPassword == null)
                    return BadRequest("Password null.");
                if (string.IsNullOrWhiteSpace(newPassword.NewPassword))
                    return BadRequest("Password is null or empty.");

                // validate the id provided is a user
                string currentUserName = _userService.GetUserNameFromClaims(User?.Claims);
                User user = await _userService.GetUser(userId);
                if (user == null)
                {
                    _logger.LogInformation($"{currentUserName} tried to update the password for user id {userId}, but that user does not exist.");
                    return BadRequest("User does not exist.");
                }

                string currentUserRole = _userService.GetRoleNameFromClaims(User?.Claims);

                // if we're updating a different user, check some additional rules based on roles
                if(!string.Equals(currentUserName, user.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    if(!InAnyAdminRole(currentUserRole))
                    {
                        // if you're not in an Admin role, you can't set another person's password
                        _logger.LogInformation($"{currentUserName} tried to update the password for user id {userId}, but the caller is not an admin.");
                        return BadRequest("Cannot update another user's password.");
                    }

                    if (IsPracticeAdmin(currentUserRole))
                    {
                        // if you're a practice admin, you can only set another users password if they're in your practice
                        int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                        if (!_staff.IsStaffInSamePractice(currentUserId, user.StaffId))
                        {
                            _logger.LogInformation($"{currentUserName} tried to update the password for user id {userId}, but the caller is in another practice.");
                            return BadRequest("Cannot update users in another practice.");
                        }
                    }
                }

                List<string> errors = ValidatePasswordComplexity(newPassword.NewPassword);
                if (errors.Any())
                    return BadRequest($"Password validation error: {string.Join(", ", errors)}");

                return Ok(await _userService.ResetUserPassword(userId, newPassword.NewPassword, currentUserName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [HttpPut("{userId}")] // put because we're updating a specific user
        [SwaggerResponse(200, type: typeof(UserViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(401, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserViewModel userViewModel)
        {
            try
            {
                if (userViewModel == null)
                    return BadRequest("User is null.");

                if (userId != userViewModel.Id)
                    return BadRequest("UserId mismatch.");

                // validate the username provided is a user
                User user = await _userService.GetUser(userId);
                string currentUserName = _userService.GetUserNameFromClaims(User?.Claims);
                if (user == null)
                {
                    _logger.LogInformation($"{currentUserName} tried to update user id {userId}, but that user does not exist.");
                    return BadRequest("User does not exist.");
                }

                List<Role> rolesInSystem = await _userService.ListRoles();
                if (userViewModel.Role != null && !rolesInSystem.Any(r => r.Id == userViewModel.Role.Id))
                    return BadRequest("Role not found.");

                Role selectedRole = rolesInSystem.First(r => r.Id == userViewModel.Role.Id);
                string currentUserRole = _userService.GetRoleNameFromClaims(User?.Claims);

                // if we're updating a different user, check some additional rules based on roles
                if (!string.Equals(currentUserName, user.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!InAnyAdminRole(currentUserRole))
                    {
                        // if you're not in an Admin role, you can't set another person's details
                        return BadRequest("Cannot update another user's details.");
                    }

                    if (IsPracticeAdmin(currentUserRole))
                    {
                        // if you're a practice admin, you can only update another user if they're in your practice
                        int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                        if (!_staff.IsStaffInSamePractice(currentUserId, user.StaffId))
                        {
                            _logger.LogInformation($"{currentUserName} tried to update user id {userId}, but the caller is in another practice.");
                            return BadRequest("Cannot update users in another practice.");
                        }
                    }
                }

                // if we're changing roles, check some additional rules based on current role
                if (user.Role.Id != userViewModel.Role.Id)
                {
                    if (!InAnyAdminRole(currentUserRole))
                    {
                        // if you're not in an admin role, you can't change roles
                        return BadRequest("Cannot change roles.");
                    }

                    if (!IsPhoAdmin(currentUserRole) && selectedRole.Name.Contains("PHO"))
                    {
                        // If you're not PHO Admin you can't use the PHO roles
                        return BadRequest("Invalid role.");
                    }
                }

                var userDetails = _mapper.Map<User>(userViewModel);
                user = await _userService.UpdateUser(userDetails, currentUserName);
                return Ok(_mapper.Map<UserViewModel>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }
        [Authorize(Roles = "Practice Admin,PHO Admin")]
        [HttpPost] // post because we're inserting a new user
        [SwaggerResponse(200, type: typeof(UserViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(401, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> InsertUser([FromBody] UserViewModel userViewModel)
        {
            try
            {
                if (userViewModel == null)
                    return BadRequest("Input is null.");
                if (string.IsNullOrWhiteSpace(userViewModel.NewPassword))
                    return BadRequest("Password is null or empty.");

                // validate the username provided is a user
                User user = await _userService.GetUser(userViewModel.UserName);
                if (user != null)
                    return BadRequest("User with this username already exists.");

                if (await _userService.GetUserByStaffId(userViewModel.StaffId) != null)
                    return BadRequest("User with this staff Id already exists.");

                List<Role> rolesInSystem = await _userService.ListRoles();
                if (userViewModel.Role != null && !rolesInSystem.Any(r => r.Id == userViewModel.Role.Id))
                    return BadRequest("Role not found.");

                Role selectedRole = rolesInSystem.First(r => r.Id == userViewModel.Role.Id);
                string currentUserName = _userService.GetUserNameFromClaims(User?.Claims);
                string currentUserRole = _userService.GetRoleNameFromClaims(User?.Claims);

                List<string> errors = ValidatePasswordComplexity(userViewModel.NewPassword);
                if (errors.Any())
                    return BadRequest($"Password validation error: {string.Join(", ", errors)}");

                if (!IsPhoAdmin(currentUserRole) && selectedRole.Name.Contains("PHO"))
                {
                    // If you're not PHO Admin you can't use the PHO roles
                    return BadRequest("Invalid role.");
                }

                if (await _staff.GetStaffDetailsById(userViewModel.StaffId) == null)
                    return BadRequest("No such staff record.");

                int userId = _userService.GetUserIdFromClaims(User?.Claims);
                if (!_staff.IsStaffInSamePractice(userId, userViewModel.StaffId))
                    return BadRequest("Cannot add users from another practice.");
                
                var userDetails = _mapper.Map<User>(userViewModel);
                user = await _userService.InsertUser(userDetails, userViewModel.NewPassword, currentUserName);
                user = await _userService.AssignStaffIdToUser(user.Id, userViewModel.StaffId, currentUserName);
                
                return Ok(_mapper.Map<UserViewModel>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }
        [Authorize(Roles = "PHO Admin")]
        [HttpPatch("{userId}/lockout")] // patch because we're only updating lockoutflag
        [SwaggerResponse(200, type: typeof(UserViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> RemoveLockoutFromUser(int userId)
        {
            try
            {
                // validate the username provided is a user
                User user = await _userService.GetUser(userId);
                string currentUserName = _userService.GetUserNameFromClaims(User?.Claims);
                if (user == null)
                {
                    _logger.LogInformation($"{currentUserName} tried to unlock user id {userId}, but that user does not exist.");
                    return BadRequest("User does not exist.");
                }

                user = await _userService.RemoveLockoutFromUser(userId, currentUserName);
                return Ok(_mapper.Map<UserViewModel>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }
        [Authorize(Roles = "PHO Admin")]
        [HttpPatch("{userId}/delete")] // patch because we're only updating deleteflag
        [SwaggerResponse(200, type: typeof(UserViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ToggleDeleteOnUser(int userId, [FromBody] bool shouldDelete)
        {
            try
            {
                // validate the username provided is a user
                User user = await _userService.GetUser(userId);
                string currentUserName = _userService.GetUserNameFromClaims(User?.Claims);
                if (user == null)
                {
                    _logger.LogInformation($"{currentUserName} tried to {(shouldDelete?"delete":"undelete")} user id {userId}, but that user does not exist.");
                    return BadRequest("User does not exist.");
                }

                user = await _userService.ToggleDeleteOnUser(userId, shouldDelete, currentUserName);
                return Ok(_mapper.Map<UserViewModel>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [HttpGet("roles")]
        [SwaggerResponse(200, type: typeof(List<RoleViewModel>))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListRoles()
        {
            try
            {
                List<Role> roles = await _userService.ListRoles();
                return Ok(_mapper.Map<List<RoleViewModel>>(roles));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }

        private bool IsPhoAdmin(string role)
        {
            return string.Equals(role, "PHO Admin", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsPracticeAdmin(string role)
        {
            return string.Equals(role, "Practice Admin", StringComparison.OrdinalIgnoreCase);
        }

        private bool InAnyAdminRole(string role)
        {
            return string.Equals(role, "PHO Admin", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(role, "Practice Admin", StringComparison.OrdinalIgnoreCase);
        }

        private List<string> ValidatePasswordComplexity(string password)
        {
            List<string> errorMessages = new List<string>();
            if (_customOptions.MinimumPasswordLength > 0 && password.Length < _customOptions.MinimumPasswordLength)
                errorMessages.Add($"Password must be at least {_customOptions.MinimumPasswordLength} characters.");
            if (_customOptions.RequireDigit && !Regex.Match(password, @"[0-9]+").Success)
                errorMessages.Add("Password must contain a digit.");
            if (_customOptions.RequireLowercase && !Regex.Match(password, @"[a-z]+").Success)
                errorMessages.Add("Password must contain a lowercase character.");
            if (_customOptions.RequireUppercase && !Regex.Match(password, @"[A-Z]+").Success)
                errorMessages.Add("Password must contain an uppercase character.");
            if (_customOptions.RequireNonAlphaNumeric && !Regex.Match(password, @"[^a-zA-Z0-9]+").Success)
                errorMessages.Add("Password must contain a special character.");
            if (password.Contains(" "))
                errorMessages.Add("Passwords cannot contain spaces.");

            return errorMessages;
        }
    }
}
