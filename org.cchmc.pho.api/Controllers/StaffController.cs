using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.Interfaces;
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

        public StaffController(ILogger<StaffController> logger, IMapper mapper, IStaff staffDal)
        {
            _logger = logger;
            _mapper = mapper;
            _staffDal = staffDal;
        }

        [HttpGet("/{userid}/{topfilter}/{tagfilter}/{namesearch}")]
        [SwaggerResponse(200, type: typeof(List<StaffViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListStaff(string userid, string topfilter, string tagfilter, string namesearch)
        {
            string user = "3"; //todo: default for now
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(user, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {user}");
                return BadRequest("user is not a valid integer");
            }

            try
            {
                // call the data method
                var data = await _staffDal.ListStaff(0, "", "", "");
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

    }
}
