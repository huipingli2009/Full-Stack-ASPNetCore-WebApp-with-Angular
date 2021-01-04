using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using org.cchmc.pho.identity.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace org.cchmc.pho.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // Uncomment this out later when the authorization parts are working
    public class AlertsController : ControllerBase
    {
        private readonly ILogger<AlertsController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IAlert _alertDal;

        
        public AlertsController(ILogger<AlertsController> logger, IUserService userService, IMapper mapper, IAlert alertDal)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
            _alertDal = alertDal;

        }

        [HttpGet()]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<AlertViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListActiveAlerts()
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _alertDal.ListActiveAlerts(currentUserId);

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
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> MarkAlertAction(string alertSchedule, [FromBody] AlertActionViewModel action)
        {

            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(alertSchedule, out var alertScheduleId))
                return BadRequest("alertSchedule is not a valid integer");

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data layer to mark the action
                await _alertDal.MarkAlertAction(alertScheduleId, currentUserId, action.AlertActionId);
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
