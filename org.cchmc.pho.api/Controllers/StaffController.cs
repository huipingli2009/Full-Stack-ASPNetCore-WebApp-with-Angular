using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.DataModels;
using Swashbuckle.AspNetCore.Annotations;


namespace org.cchmc.pho.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly ILogger<StaffController> _logger;
        private readonly IMapper _mapper;
        private readonly IStaff _staffDal;

        private readonly string _DEFAULT_USER = "3";

        public StaffController(ILogger<StaffController> logger, IMapper mapper, IStaff staffDal)
        {
            _logger = logger;
            _mapper = mapper;
            _staffDal = staffDal;
        }

        [HttpGet()]
        [SwaggerResponse(200, type: typeof(List<StaffViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListStaff([FromQuery]string topfilter, [FromQuery]string tagfilter, [FromQuery]string namesearch)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(_DEFAULT_USER, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                return BadRequest("user is not a valid integer");
            }

            try
            {
                // call the data method
                var data = await _staffDal.ListStaff(userId, topfilter, tagfilter, namesearch);
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

        [HttpGet("{staff}")]
        [SwaggerResponse(200, type: typeof(StaffDetailViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetStaffDetails(string staff)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(_DEFAULT_USER, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                return BadRequest("user is not a valid integer");
            }

            if (!int.TryParse(staff, out var staffId))
            {
                _logger.LogInformation($"Failed to parse staffId - {staff}");
                return BadRequest("staff is not a valid integer");
            }

            try
            {
                // call the data method
                var data = await _staffDal.GetStaffDetails(userId, staffId);
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

        [HttpPut("{staff}")]
        [SwaggerResponse(200, type: typeof(StaffDetailViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdateStaffDetails([FromBody] StaffDetailViewModel staffDetailVM, string staff)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(_DEFAULT_USER, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                return BadRequest("user is not a valid integer");
            }

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

            if (!_staffDal.IsStaffInSamePractice(userId, staffId))
            {
                _logger.LogInformation($"staff and user practices do not match");
                return BadRequest("staff practice does not match user");
            }

            try
            {
                StaffDetail staffDetail = _mapper.Map<StaffDetail>(staffDetailVM);
                // call the data layer to mark the action
                var data = await _staffDal.UpdateStaffDetails(userId, staffDetail);
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

        [HttpGet("positions")]
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
        [SwaggerResponse(200, type: typeof(List<ProviderViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListProviders()
        {
            try
            {
                // route parameters are strings and need to be translated (and validated) to their proper data type
                if (!int.TryParse(_DEFAULT_USER, out var userId))
                {
                    _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                    return BadRequest("user is not a valid integer");
                }

                // call the data method
                var data = await _staffDal.ListProviders(userId);
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

        [HttpPut("{staff}/legalstatus")]
        [SwaggerResponse(200, type: typeof(bool))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> SignLegalDisclaimer(string staff)
        {
            //TODO: need a method to check the current user has the staff id specified           
            if (!int.TryParse(_DEFAULT_USER, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                return BadRequest("user is not a valid integer");
            }

            if (!int.TryParse(staff, out var staffId))
            {
                _logger.LogInformation($"Failed to parse staffId - {staff}");
                return BadRequest("staff is not a valid integer");
            }         

            try
            {
                var result = await _staffDal.SignLegalDisclaimer(userId);            
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
        [SwaggerResponse(200, type: typeof(SelectPracticeViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetPracticeList()
        {
            if (!int.TryParse(_DEFAULT_USER, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                return BadRequest("user is not a valid integer");
            }

            try
            {
                // call the data method
                var data = await _staffDal.GetPracticeList(userId);
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
        [SwaggerResponse(200, type: typeof(bool))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> SwitchPractice([FromBody]StaffViewModel staffVM)
        {
            if (!int.TryParse(_DEFAULT_USER, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                return BadRequest("user is not a valid integer");
            }

            if (staffVM.MyPractice == null)
            {               
                return BadRequest("You didn't select a practice");
            }           

            try
            {
                var result = await _staffDal.SwitchPractice(userId, staffVM.MyPractice.Id);
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
