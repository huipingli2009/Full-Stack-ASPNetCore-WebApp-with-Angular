using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.identity.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace org.cchmc.pho.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly ILogger<StaffController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IStaff _staffDal;


        public StaffController(ILogger<StaffController> logger, IUserService userService, IMapper mapper, IStaff staffDal)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _staffDal = staffDal;
        }

        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [HttpGet()]
        [SwaggerResponse(200, type: typeof(List<StaffViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListStaff()
        {

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _staffDal.ListStaff(currentUserId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<StaffViewModel>>(data);
                // return the result in a "200 OK" response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [HttpGet("{staff}")]
        [SwaggerResponse(200, type: typeof(StaffDetailViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetStaffDetails(string staff)
        {
            if (!int.TryParse(staff, out var staffId))
            {
                _logger.LogInformation($"Failed to parse staffId - {staff}");
                return BadRequest("staff is not a valid integer");
            }

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _staffDal.GetStaffDetails(currentUserId, staffId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<StaffDetailViewModel>(data);
                // return the result in a "200 OK" response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [Authorize(Roles = "Practice Admin,PHO Admin")]
        [HttpPut("{staff}")]
        [SwaggerResponse(200, type: typeof(StaffDetailViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdateStaffDetails([FromBody] StaffDetailViewModel staffDetailVM, string staff)
        {
            if (!int.TryParse(staff, out var staffId))
            {
                _logger.LogInformation($"Failed to parse staffId - {staff}");
                return BadRequest("staff is not a valid integer");
            }

            if (staffDetailVM == null)
            {
                _logger.LogInformation($"staffDetails object is null");
                return BadRequest("staff is null");
            }

            if (staffDetailVM.Id != staffId)
            {
                _logger.LogInformation($"staffDetails.Id and staffId to not match");
                return BadRequest("staff id does not match");
            }

            try
            {
                // route parameters are strings and need to be translated (and validated) to their proper data type
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);

                if (!_staffDal.IsStaffInSamePractice(currentUserId, staffId))
                {
                    _logger.LogInformation($"staff and user practices do not match");
                    return BadRequest("staff practice does not match user");
                }

                StaffDetail staffDetail = _mapper.Map<StaffDetail>(staffDetailVM);
                // call the data layer to mark the action
                var data = await _staffDal.UpdateStaffDetails(currentUserId, staffDetail);
                var result = _mapper.Map<StaffDetailViewModel>(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }


        [Authorize(Roles = "Practice Admin,PHO Admin")]
        [HttpPut("remove/{staff}")]
        [SwaggerResponse(200, type: typeof(bool))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> RemoveStaff(string staff, string end, string deleted)
        {
            if (!int.TryParse(staff, out var staffId))
            {
                _logger.LogInformation($"Failed to parse staffId - {staff}");
                return BadRequest("staff is not a valid integer");
            }
            if (!DateTime.TryParse(end, out var endDate))
            {
                _logger.LogInformation($"Failed to parse endDate - {end}");
                return BadRequest("endDate is not a valid date");
            }
            if (string.IsNullOrWhiteSpace(deleted))
            {
                _logger.LogInformation($"Failed to parse deletedFlag - {deleted}");
                return BadRequest("deletedFlag is blank");
            }
            if (!Boolean.TryParse(deleted, out var deletedFlag))
            {
                _logger.LogInformation($"Failed to parse deletedFlag - {deleted}");
                return BadRequest("deletedFlag is not a valid boolean");
            }            

            try
            {
                // route parameters are strings and need to be translated (and validated) to their proper data type
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);

                if (!_staffDal.IsStaffInSamePractice(currentUserId, staffId))
                {
                    _logger.LogInformation($"staff and user practices do not match");
                    return BadRequest("staff practice does not match user");
                }

                // call the data layer to mark the action
                var data = await _staffDal.RemoveStaff(currentUserId, staffId, endDate, deletedFlag);
                return Ok(data);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("locations")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<LocationViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListLocations()
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _staffDal.ListLocations(currentUserId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<LocationViewModel>>(data);
                // return the result in a "200 OK" response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("positions")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<PositionViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListPositions()
        {
            try
            {
                // call the data method
                var data = await _staffDal.ListPositions();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<PositionViewModel>>(data);
                // return the result in a "200 OK" response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("credentials")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<CredentialViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListCredentials()
        {
            try
            {
                // call the data method
                var data = await _staffDal.ListCredentials();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<CredentialViewModel>>(data);
                // return the result in a "200 OK" response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("responsibilities")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<ResponsibilityViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListResponsibilities()
        {
            try
            {
                // call the data method
                var data = await _staffDal.ListResponsibilities();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<ResponsibilityViewModel>>(data);
                // return the result in a "200 OK" response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("providers")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<ProviderViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListProviders()
        {
            try
            {
                // route parameters are strings and need to be translated (and validated) to their proper data type
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);

                // call the data method
                var data = await _staffDal.ListProviders(currentUserId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<ProviderViewModel>>(data);
                // return the result in a "200 OK" response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpPut("legalstatus")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(bool))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> SignLegalDisclaimer()
        {
            try
            {
                //get user from the current claims
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                if(currentUserId == 0)
                {
                    //NOTE: technically i guess this shouldn't happen... but just in case
                    //checking to make sure a valid userid returns
                    _logger.LogError($"Failed to Sign Legal Disclaimer, Current users return back as : {currentUserId}");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to sign legal disclaimer");
                }

                var result = await _staffDal.SignLegalDisclaimer(currentUserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("practicelist")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(SelectPracticeViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetPracticeList()
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _staffDal.GetPracticeList(currentUserId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<SelectPracticeViewModel>(data);
                // return the result in a "200 OK" response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }

        }

        [HttpPut("switchpractice")]
        [Authorize(Roles = "PHO Admin")]
        [SwaggerResponse(200, type: typeof(bool))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> SwitchPractice([FromBody]StaffViewModel staffVM)
        {

            if (staffVM.MyPractice == null)
            {
                return BadRequest("You didn't select a practice");
            }

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var result = await _staffDal.SwitchPractice(currentUserId, staffVM.MyPractice.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
