using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace org.cchmc.pho.api.Controllers
{
    [ApiController]
    [Route("api/Alerts")]
    //[Authorize] // Uncomment this out later when the authorization parts are working
    public class AlertController : ControllerBase
    {
        private readonly ILogger<AlertController> _logger;
        private readonly IMapper _mapper;
        private readonly IAlert _alertDal;

        //TODO: delete me refactor
        private readonly CustomOptions _customOptions;


        public AlertController(ILogger<AlertController> logger, IMapper mapper, IAlert alertDal, IOptions<CustomOptions> customOptions)
        {
            _logger = logger;
            _mapper = mapper;
            _alertDal = alertDal;

            //TODO : CAN add some validation on this
           // _customOptions = customOptions.Value;

            _logger.LogInformation($"Example of options {_customOptions?.RequiredOption}");


        }

              

        [HttpGet("active/{user}")]
        [SwaggerResponse(200, type: typeof(List<AlertViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListActiveAlerts(string user)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(user, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {user}");
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

        [HttpPost("{user}/alert/{alertSchedule}")]
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> MarkAlertAction(string user, string alertSchedule, [FromBody] AlertActionViewModel action)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(user, out var userId))
                return BadRequest("user is not a valid integer");

            if (!int.TryParse(alertSchedule, out var alertScheduleId))
                return BadRequest("alertSchedule is not a valid integer");

            try
            {
                // call the data layer to mark the action
                await _alertDal.MarkAlertAction(userId, alertScheduleId, action.AlertActionId, action.ActionDateTime);
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
