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
    public class PatientsController : ControllerBase
    {
        private readonly ILogger<PatientsController> _logger;
        private readonly IMapper _mapper;
        private readonly IPatient _patient;

        public PatientsController(ILogger<PatientsController> logger, IMapper mapper, IPatient patient)
        {
            _logger = logger;
            _mapper = mapper;
            _patient = patient;
        }

        // GET: api/Patient
        //[HttpGet("PatientList/{userId}/{staffID?}/{popmeasureID?}/{watch?}/{chronic?}/{conditionIDs?}/{namesearch?}/{sortcolumn?}/{pagenumber}/{rowspage}")]
        [HttpGet("patientlist/{userId}")]

        [SwaggerResponse(200, type: typeof(List<PatientViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListActivePatient(int userId, int staffID, int popmeasureID, bool watch, bool chronic, string conditionIDs, string namesearch)
        {         

            try
            {
                var data = await _patient.ListActivePatient(userId, staffID, popmeasureID, watch, chronic, conditionIDs, namesearch);

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
    }
}
