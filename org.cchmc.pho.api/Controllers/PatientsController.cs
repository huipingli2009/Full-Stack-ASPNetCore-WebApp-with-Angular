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
    public class PatientsController : ControllerBase
    {
        private readonly ILogger<PatientsController> _logger;
        private readonly IMapper _mapper;
        private readonly IPatient _patient;
        
        private readonly string _DEFAULT_USER = "3";
        public PatientsController(ILogger<PatientsController> logger, IMapper mapper, IPatient patient)
        {
            _logger = logger;
            _mapper = mapper;
            _patient = patient;
        }

        // GET: api/Patient
        //[HttpGet("PatientList/{userId}/{staffID?}/{popmeasureID?}/{watch?}/{chronic?}/{conditionIDs?}/{namesearch?}/{sortcolumn?}/{pagenumber}/{rowspage}")]
        [HttpGet]

        [SwaggerResponse(200, type: typeof(List<PatientViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListActivePatient(int? staffID, int? popmeasureID, bool? watch, bool? chronic, string conditionIDs, string namesearch, string sortcolumn, string sortdirection, int? pagenumber, int? rowsPerPage)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(_DEFAULT_USER, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                return BadRequest("user is not a valid integer");
            }

            try
            {
                var data = await _patient.ListActivePatient(int.Parse(_DEFAULT_USER.ToString()), staffID, popmeasureID, watch, chronic, conditionIDs, namesearch,sortcolumn,sortdirection,pagenumber,rowsPerPage);

                var result = _mapper.Map<List<PatientViewModel>>(data);

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


        [HttpGet("{patient}")]
        [SwaggerResponse(200, type: typeof(List<PatientDetailsViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetPatientDetails(string patient)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(_DEFAULT_USER, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                return BadRequest("user is not a valid integer");
            }

            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(patient, out var patientId))
            {
                _logger.LogInformation($"Failed to parse patientId - {patient}");
                return BadRequest("patient is not a valid integer");
            }

            try
            {
                // call the data method
                var data = await _patient.GetPatientDetails(patientId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<PatientDetailsViewModel>(data);
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

        [HttpPut("{patient}")]
        [SwaggerResponse(200, type: typeof(PatientDetailsViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdatePatientDetails([FromBody] PatientDetailsViewModel patientDetailsVM, string patient)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(_DEFAULT_USER, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                return BadRequest("user is not a valid integer");
            }

            if (!int.TryParse(patient, out var patientId))
            {
                _logger.LogInformation($"Failed to parse patientId - {patient}");
                return BadRequest("patient is not a valid integer");
            }

            if (patientDetailsVM == null)
            {
                _logger.LogInformation("patientDetails object is null");
                return BadRequest("patient is null");
            }

            if (patientDetailsVM.Id != patientId)
            {
                _logger.LogInformation($"patientDetails.Id and patientId to not match");
                return BadRequest("patient id does not match");
            }

            if (!_patient.IsPatientInSamePractice(userId, patientId))
            {
                _logger.LogInformation($"patient and user practices do not match");
                return BadRequest("patient practice does not match user");
            }

            //TODO - 

            try
            {
                PatientDetails patientDetail = _mapper.Map<PatientDetails>(patientDetailsVM);
                // call the data layer to mark the action
                var data = await _patient.UpdatePatientDetails(userId,patientDetail);
                var result = _mapper.Map<PatientDetailsViewModel>(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("conditions")]
        [SwaggerResponse(200, type: typeof(List<PatientConditionViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetConditionsAll()
        {
            try
            {
                // call the data method
                var data = await _patient.GetPatientConditionsAll();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<PatientConditionViewModel>>(data);
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

        [HttpGet("insurance")]
        [SwaggerResponse(200, type: typeof(List<PatientInsuranceViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetInsuranceAll()
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
                var data = await _patient.GetPatientInsuranceAll(userId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<PatientInsuranceViewModel>>(data);
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
