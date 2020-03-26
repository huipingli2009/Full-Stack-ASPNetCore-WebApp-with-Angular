﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.identity.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;


namespace org.cchmc.pho.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly ILogger<PatientsController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IPatient _patient;
        
        public PatientsController(ILogger<PatientsController> logger, IUserService userService, IMapper mapper, IPatient patient)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
            _patient = patient;
        }

        // GET: api/Patient
        //[HttpGet("PatientList/{userId}/{staffID?}/{popmeasureID?}/{watch?}/{chronic?}/{conditionIDs?}/{namesearch?}/{sortcolumn?}/{pagenumber}/{rowspage}")]
        [HttpGet]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<PatientViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListActivePatient(int? staffID, int? popmeasureID, bool? watch, bool? chronic, string conditionIDs, string namesearch, string sortcolumn, string sortdirection, int? pagenumber, int? rowsPerPage)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var data = await _patient.ListActivePatient(currentUserId, staffID, popmeasureID, watch, chronic, conditionIDs, namesearch,sortcolumn,sortdirection,pagenumber,rowsPerPage);
                var result = _mapper.Map<SearchResultsViewModel<PatientViewModel>>(data);

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
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
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

        // GET: api/Workbooks
        [HttpGet("simple/{searchTerm}")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<SimplifiedPatientViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]

        //public async Task<IActionResult> ListPatients(int userId, int formResponseId, string nameSearch)
        public async Task<IActionResult> ListPatients(string searchTerm)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var data = await _patient.SearchSimplifiedPatients(currentUserId, searchTerm);

                var result = _mapper.Map<List<SimplifiedPatientViewModel>>(data);

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
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(PatientDetailsViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdatePatientDetails([FromBody] PatientDetailsViewModel patientDetailsVM, string patient)
        {
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

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                
                if (!_patient.IsPatientInSamePractice(currentUserId, patientId))
                {
                    _logger.LogInformation($"patient and user practices do not match");
                    return BadRequest("patient practice does not match user");
                }

                PatientDetails patientDetail = _mapper.Map<PatientDetails>(patientDetailsVM);
                // call the data layer to mark the action
                var data = await _patient.UpdatePatientDetails(currentUserId, patientDetail);
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

        [HttpPut("watchlist/{patient}")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(bool))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdatePatientWatchlist(string patient)
        {
            if (!int.TryParse(patient, out var patientId))
            {
                _logger.LogInformation($"Failed to parse patientId - {patient}");
                return BadRequest("patient is not a valid integer");
            }

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                
                if (!_patient.IsPatientInSamePractice(currentUserId, patientId))
                {
                    _logger.LogInformation($"patient and user practices do not match");
                    return BadRequest("patient practice does not match user");
                }

                // call the data layer to mark the action
                var data = await _patient.UpdatePatientWatchlist(currentUserId, patientId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("conditions")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
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
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<PatientInsuranceViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetInsuranceAll()
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _patient.GetPatientInsuranceAll(currentUserId);
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


        [HttpGet("gender")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<GenderViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListGender()
        {
            try
            {
                // call the data method
                var data = await _patient.ListGender();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<GenderViewModel>>(data);
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

        [HttpGet("pmca")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<PMCAViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListPMCA()
        {
            try
            {
                // call the data method
                var data = await _patient.ListPMCA();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<PMCAViewModel>>(data);
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

        [HttpGet("state")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<StateViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListState()
        {
            try
            {
                // call the data method
                var data = await _patient.ListState();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<StateViewModel>>(data);
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
