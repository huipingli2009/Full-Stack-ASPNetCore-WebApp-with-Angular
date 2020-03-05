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
    public class MetricsController : ControllerBase
    {
        private readonly ILogger<MetricsController> _logger;
        private readonly IMapper _mapper;
        private readonly IMetric _metricDal;

        public MetricsController(ILogger<MetricsController> logger, IMapper mapper, IMetric metricDal)
        {
            _logger = logger;
            _mapper = mapper;
            _metricDal = metricDal;
        }

        [HttpGet("kpis")]
        [SwaggerResponse(200, type: typeof(List<DashboardMetricViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListDashboardMetrics()
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
                var data = await _metricDal.ListDashboardMetrics(userId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<DashboardMetricViewModel>>(data);
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
        [HttpGet("pop")]
        [SwaggerResponse(200, type: typeof(List<PopulationMetricViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListPopulationMetrics()
        {
            try
            {
                // call the data method
                var data = await _metricDal.ListPopulationMetrics();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<PopulationMetricViewModel>>(data);
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

        [HttpGet("edcharts")]
        [SwaggerResponse(200, type: typeof(List<EDChartViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListEDChart()
        {
            string user = "3"; //todo :hard coded for now replace later from sessions

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

        [HttpGet("edcharts/{admitdate}")]
        [SwaggerResponse(200, type: typeof(List<EDDetailViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListEDDetails(string admitdate)
        {
            //todo: look at describign a date format as yyyymmdd : eg 20200227
            string user = "3"; //todo :hard coded for now replace later from sessions

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