using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace org.cchmc.pho.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkbooksController : ControllerBase
    {
        private readonly ILogger<WorkbooksController> _logger;
        private readonly IMapper _mapper;
        private readonly IWorkbooks _workbooks;

        //todo: hardcoded userid for now, we will be using session later
        private readonly string _DEFAULT_USER = "3";

        public WorkbooksController(ILogger<WorkbooksController> logger, IMapper mapper, IWorkbooks workbooks)
        {
            _logger = logger;
            _mapper = mapper;
            _workbooks = workbooks;
        }

        // GET: api/Workbooks
        [HttpGet("patients")]
        [SwaggerResponse(200, type: typeof(List<WorkbooksPatientViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]

        //public async Task<IActionResult> ListPatients(int userId, int formResponseId, string nameSearch)
        public async Task<IActionResult> ListPatients(int formResponseId)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(_DEFAULT_USER, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                return BadRequest("user is not a valid integer");
            }

            try
            {
                var data = await _workbooks.ListPatients(int.Parse(_DEFAULT_USER.ToString()), formResponseId);

                var result = _mapper.Map<List<WorkbooksPatientViewModel>>(data);

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

        [HttpGet("practice")]
        [SwaggerResponse(200, type: typeof(WorkbooksPracticeViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetPracticeWorkbooks(int formResponseId)
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
                var data = await _workbooks.GetPracticeWorkbooks(int.Parse(_DEFAULT_USER.ToString()), formResponseId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<WorkbooksPracticeViewModel>(data);
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

        [HttpGet("providers")]
        [SwaggerResponse(200, type: typeof(List<WorkbooksProviderViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]

        public async Task<IActionResult> GetPracticeWorkbooksProviders(int formResponseId)
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
                var data = await _workbooks.GetPracticeWorkbooksProviders(int.Parse(_DEFAULT_USER.ToString()), formResponseId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<WorkbooksProviderViewModel>>(data);
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

        [HttpGet("lookups")]
        [SwaggerResponse(200, type: typeof(List<WorkbooksLookupViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetPracticeWorkbooksLookups(string nameSearch)
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
                var data = await _workbooks.GetWorkbooksLookups(int.Parse(_DEFAULT_USER.ToString()), nameSearch);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<WorkbooksLookupViewModel>>(data);
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
       
        [HttpPost("patients/{id}")]
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]       
        public async Task<IActionResult> AddPatientToWorkbooks(int id,[FromBody] WorkbooksPatientViewModel workbookspatientVM)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(_DEFAULT_USER, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                return BadRequest("user is not a valid integer");
            }

            try
            {
                var result = await _workbooks.AddPatientToWorkbooks(userId, workbookspatientVM.FormResponseId, id, workbookspatientVM.ProviderId, workbookspatientVM.DateOfService, int.Parse(workbookspatientVM.PHQ9_Score), workbookspatientVM.ActionFollowUp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPut("provider/{id}")]
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdateWorkbooksProvider(int id, [FromBody] WorkbooksProviderViewModel workbooksproviderVM)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(_DEFAULT_USER, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                return BadRequest("user is not a valid integer");
            }

            try
            {
                await _workbooks.UpdateWorkbooksProviders(userId, workbooksproviderVM.FormResponseID, id, workbooksproviderVM.PHQS, workbooksproviderVM.TOTAL);
                return Ok();
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("patientfollowup")]
        [SwaggerResponse(200, type: typeof(WorkbooksPatientFollowupViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetWorkbooksPatientPHQ9FollowUp(int formResponseId, int patientID)
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
                var data = await _workbooks.GetWorkbooksPatientPHQ9FollowUp(int.Parse(_DEFAULT_USER.ToString()), formResponseId, patientID);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<WorkbooksPatientFollowup>(data);
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

        [HttpPut("patientfollowup/{id}")]
        [SwaggerResponse(200, type: typeof(WorkbooksPatientFollowupViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
       
        public async Task<IActionResult> UpdateWorkbooksPatientFollowup(int id, [FromBody]WorkbooksPatientFollowupViewModel wbptfollowup)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(_DEFAULT_USER, out var userId))
            {
                _logger.LogInformation($"Failed to parse userId - {_DEFAULT_USER}");
                return BadRequest("user is not a valid integer");
            }

            try
            {
                await _workbooks.UpdateWorkbooksPatientFollowup(userId, wbptfollowup.FormResponseId, wbptfollowup.PatientId, wbptfollowup.ActionPlanGiven, wbptfollowup.ManagedByExternalProvider, wbptfollowup.DateOfLastCommunicationByExternalProvider, wbptfollowup.FollowupPhoneCallOneToTwoWeeks, wbptfollowup.DateOfFollowupCall, wbptfollowup.OneMonthFollowupVisit, wbptfollowup.DateOfOneMonthVisit, wbptfollowup.OneMonthFolllowupPHQ9Score);
                return Ok();
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
