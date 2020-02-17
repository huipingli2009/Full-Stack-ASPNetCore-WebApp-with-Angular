using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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

namespace org.cchmc.pho.api.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly List<string> _validRoles = new List<string>() { "PracticeMember", "PracticeAdmin", "PHOMember", "PHOAdmin" };

        //TODO: delete me refactor
        private readonly CustomOptions _customOptions;

        public UserController(ILogger<UserController> logger, IMapper mapper, IUserService userService, IOptions<CustomOptions> customOptions)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;

            //TODO : CAN add some validation on this
            _customOptions = customOptions.Value;

            _logger.LogInformation($"Example of options {_customOptions?.RequiredOption}");
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

                return Ok(new AuthenticationResult { Status = "Authorized", Token = user.Token });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }

        // TODO: [Authorize(Roles = "PracticeAdmin,PHOAdmin")]
        [HttpGet("userByEmail/{emailAddress}")]
        [SwaggerResponse(200, type: typeof(UserViewModel))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetUserByEmailAddress(string emailAddress)
        {
            try
            {
                User user = await _userService.GetUserByEmail(emailAddress);
                if (user == null)
                    return null;

                UserViewModel returnUser = _mapper.Map<UserViewModel>(user);
                returnUser.Roles = await _userService.GetRoles(user.UserName);

                return Ok(returnUser);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }

        // TODO: [Authorize(Roles = "PracticeMember,PracticeAdmin,PHOMember,PHOAdmin")]
        [HttpPatch("user/{userName}/password")] // patch because we're only updating parts
        [SwaggerResponse(200, type: typeof(bool))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdateUserPassword(string userName, [FromBody] PasswordChangeViewModel newPassword)
        {
            try
            {
                if (newPassword == null)
                    return BadRequest();
                if (String.IsNullOrEmpty(userName))
                    return BadRequest("UserName is null or empty.");
                if (String.IsNullOrEmpty(newPassword.NewPassword))
                    return BadRequest("Password is null or empty.");

                string currentUserName = _userService.GetUserNameFromClaims(User.Claims);
                List<string> currentUserRoles = _userService.GetRolesFromClaims(User.Claims);

                // validate the username provided is a user
                User user = await _userService.GetUserByUserName(userName);
                if (user == null)
                    return BadRequest("User does not exist.");

                if(!InAnyAdminRole(currentUserRoles)
                    && !string.Equals(currentUserName, user.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    // if you're not in an Admin role, you can't set another person's password
                    return BadRequest("Cannot update another user's password.");
                }

                // TODO: If you're in the practice admin role, you can only change passwords for people in your practice (need StaffDAL or service)

                // TODO: What's the best way to communicate the password requirements to the UI?
                var errors = await _userService.ValidatePassword(userName, newPassword.NewPassword);
                if(errors.Any())
                    return BadRequest(string.Join(";", errors));

                // TODO: How do we invalidate the current user's session, or do we just let it time out naturally?
                return Ok(await _userService.ResetUserPassword(userName, newPassword.NewPassword));                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }

        // TODO: [Authorize(Roles = "PracticeAdmin,PHOAdmin")]
        [HttpPatch("user/{userName}/roles")] // patch because we're only updating parts
        [SwaggerResponse(200, type: typeof(bool))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdateUserRoles(string userName, [FromBody] RoleChangeViewModel newRoles)
        {
            try
            {
                if (newRoles == null)
                    return BadRequest();
                if (String.IsNullOrEmpty(userName))
                    return BadRequest("UserName is null or empty.");
                if (newRoles.NewRoles == null)
                    newRoles.NewRoles = new List<string>();

                string currentUserName = _userService.GetUserNameFromClaims(User.Claims);
                List<string> currentUserRoles = _userService.GetRolesFromClaims(User.Claims);

                // validate the username provided is a user
                User user = await _userService.GetUserByUserName(userName);
                if (user == null)
                    return BadRequest("User does not exist.");

                // if the user is not a PHO Admin, they can't add/remove any role that has PHO in it
                List<string> roles = await _userService.GetRoles(userName);
                if (!IsPhoAdmin(currentUserRoles))
                {
                    newRoles.NewRoles = PatchIncomingRoleList(newRoles.NewRoles, roles);
                }

                // TODO: If you're in the practice admin role, you can only change roles for people in your practice (need StaffDAL or service)

                foreach (string r in newRoles.NewRoles)
                {
                    if (!IsValidRole(r))
                        return BadRequest("Invalid Role specified.");
                }

                // TODO: How do we invalidate the current users session?
                return Ok(await _userService.ChangeUserRoles(userName, newRoles.NewRoles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }

        // TODO: [Authorize(Roles = "PracticeMember,PracticeAdmin,PHOMember,PHOAdmin")]
        [HttpGet("roles")]
        [SwaggerResponse(200, type: typeof(List<string>))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListRoles()
        {
            try
            {
                return Ok(await _userService.ListRoles());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }

        private bool IsPhoAdmin(List<string> roles)
        {
            return roles.Contains("PHOAdmin");
        }

        private bool InAnyAdminRole(List<string> roles)
        {
            return roles.Contains("PHOAdmin") || roles.Contains("PracticeAdmin");
        }

        private bool IsValidRole(string role)
        {
            return _validRoles.Contains(role);
        }

        private List<string> PatchIncomingRoleList(List<string> incomingRoleList, List<string> currentRoleList)
        {
            var phoRoleList = new List<string> { "PHOAdmin", "PHOMember" };
            foreach (var r in phoRoleList)
            {
                if (currentRoleList.Contains(r) && !incomingRoleList.Contains(r))
                    incomingRoleList.Add(r);
            }
            return incomingRoleList;
        }
    }
}
