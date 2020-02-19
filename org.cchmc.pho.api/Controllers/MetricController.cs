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
    [Route("api/Metrics")]
    public class MetricController : ControllerBase
    {
        private readonly ILogger<AlertController> _logger;
        private readonly IMapper _mapper;
        private readonly IMetric _metricDal;

        public MetricController(ILogger<AlertController> logger, IMapper mapper, IMetric metricDal)
        {
            _logger = logger;
            _mapper = mapper;
            _metricDal = metricDal;
        }

        [HttpGet("list/{user}")]
        [SwaggerResponse(200, type: typeof(List<AlertViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListDashboardMetrics(string user)
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
                var data = await _metricDal.ListDashboardMetrics(userId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<MetricViewModel>>(data);
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

        [HttpGet("edchart/{user}")]
        [SwaggerResponse(200, type: typeof(List<AlertViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListEDChart(string user)
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
                var data = await _metricDal.ListEDChart(userId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<EDChartViewModel>>(data);
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

        [HttpGet("eddetails/{user}/{admitdate}")]
        [SwaggerResponse(200, type: typeof(List<AlertViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListEDDetails(string user, string admitdate)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(user, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {user}");
                return BadRequest("user is not a valid integer");
            }
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!DateTime.TryParse(admitdate, out var admitDateTime))
            {
                _logger.LogInformation($"Failed to parse admitDate - {admitdate}");
                return BadRequest("admitdate is not a valid datetime");
            }

            try
            {
                // call the data method
                var data = await _metricDal.ListEDDetails(userId, admitDateTime);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<EDDetailViewModel>>(data);
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