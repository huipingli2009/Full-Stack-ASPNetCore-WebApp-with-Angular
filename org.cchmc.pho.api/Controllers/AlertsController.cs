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
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // Uncomment this out later when the authorization parts are working
    public class AlertsController : ControllerBase
    {
        private readonly ILogger<AlertsController> _logger;
        private readonly IMapper _mapper;
        private readonly IAlert _alertDal;



         //todo hardcoded for now.... future will get from session
        private readonly string _DEFAULT_USER = "3";

        public AlertsController(ILogger<AlertsController> logger, IMapper mapper, IAlert alertDal)
        {
            _logger = logger;
            _mapper = mapper;
            _alertDal = alertDal;

        }

        [HttpGet()]
        [SwaggerResponse(200, type: typeof(List<AlertViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListActiveAlerts()
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
                var data = await _alertDal.ListActiveAlerts(userId);

                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<AlertViewModel>>(data);

                // return the result in a "200 OK" response
                return Ok(result);
            }
            catch(Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPost("{alertSchedule}")]
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> MarkAlertAction(string alertSchedule, [FromBody] AlertActionViewModel action)
        {

            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(_DEFAULT_USER, out var userId))
                return BadRequest("user is not a valid integer");


            if (!int.TryParse(alertSchedule, out var alertScheduleId))
                return BadRequest("alertSchedule is not a valid integer");

            try
            {
                // call the data layer to mark the action
                await _alertDal.MarkAlertAction(alertScheduleId, userId, action.AlertActionId);
                return Ok();
            }
            catch(Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
