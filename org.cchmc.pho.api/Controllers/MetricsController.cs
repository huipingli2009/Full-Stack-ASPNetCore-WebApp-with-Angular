using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.identity.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace org.cchmc.pho.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly ILogger<MetricsController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IMetric _metricDal;

        public MetricsController(ILogger<MetricsController> logger, IUserService userService, IMapper mapper, IMetric metricDal)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
            _metricDal = metricDal;
        }

        [HttpGet("kpis")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<DashboardMetricViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListDashboardMetrics()
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _metricDal.ListDashboardMetrics(currentUserId);
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
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
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
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<EDChartViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListEDChart()
        {

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _metricDal.ListEDChart(currentUserId);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("edcharts/{admitdate}")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<EDDetailViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListEDDetails(string admitdate)
        {
            //todo: look at describign a date format as yyyymmdd : eg 20200227

            // route parameters are strings and need to be translated (and validated) to their proper data type           
            if (!DateTime.TryParseExact(admitdate, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out var admitDateTime))
            {
                _logger.LogInformation($"Failed to parse admitDate - {admitdate}");
                return BadRequest("admitdate is not a valid datetime");
            }

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _metricDal.ListEDDetails(currentUserId, admitDateTime);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("drillthru/{measure}/{filter}")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(DrillthruMetricTableViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetDrillthruTable(string measure, string filter)
        {
            if (!int.TryParse(measure, out var measureId))
                return BadRequest("measure is not a valid integer"); 
            if (!int.TryParse(filter, out var filterId))
                return BadRequest("filter is not a valid integer");

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _metricDal.GetDrillthruTable(currentUserId, measureId, filterId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<DrillthruMetricTableViewModel>(data);
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

       
        [HttpGet("outcomepop")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<PopulationOutcomeMetricViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetPopulationOutcomeMetrics()
        {
            try
            {
                // call the data method
                var data = await _metricDal.GetPopulationOutcomeMetrics();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<PopulationOutcomeMetricViewModel>>(data);
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