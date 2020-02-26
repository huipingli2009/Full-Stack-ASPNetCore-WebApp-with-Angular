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
    [Route("api/patients")]
    [ApiController]
    public class PatientController : ControllerBase
    {       
        private readonly ILogger<AlertController> _logger;
        private readonly IMapper _mapper;
        private readonly IPatient _patient;
       
        public PatientController(ILogger<AlertController> logger, IMapper mapper, IPatient patient)
        {
            _logger = logger;
            _mapper = mapper;
            _patient = patient;
        }

        // GET: api/Patient
        
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
    }
}
