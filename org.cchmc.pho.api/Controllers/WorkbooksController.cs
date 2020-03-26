using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.identity.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace org.cchmc.pho.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkbooksController : ControllerBase
    {
        private readonly ILogger<WorkbooksController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IWorkbooks _workbooks;


        public WorkbooksController(ILogger<WorkbooksController> logger, IUserService userService, IMapper mapper, IWorkbooks workbooks)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
            _workbooks = workbooks;
        }

        // GET: api/Workbooks
        [HttpGet("patients")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<WorkbooksPatientViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]

        //public async Task<IActionResult> ListPatients(int userId, int formResponseId, string nameSearch)
        public async Task<IActionResult> ListPatients(int formResponseId)
        {

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var data = await _workbooks.ListPatients(currentUserId, formResponseId);

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
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(WorkbooksPracticeViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetPracticeWorkbooks(int formResponseId)
        {

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _workbooks.GetPracticeWorkbooks(currentUserId, formResponseId);
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
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<WorkbooksProviderViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]

        public async Task<IActionResult> GetPracticeWorkbooksProviders(int formResponseId)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _workbooks.GetPracticeWorkbooksProviders(currentUserId, formResponseId);
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
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<WorkbooksLookupViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetPracticeWorkbooksLookups(string nameSearch)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _workbooks.GetWorkbooksLookups(currentUserId, nameSearch);
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
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> AddPatientToWorkbooks(int id, [FromBody] WorkbooksPatientViewModel workbookspatientVM)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var result = await _workbooks.AddPatientToWorkbooks(currentUserId, workbookspatientVM.FormResponseId, id, workbookspatientVM.ProviderId, workbookspatientVM.DateOfService, int.Parse(workbookspatientVM.PHQ9_Score), workbookspatientVM.ActionFollowUp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpDelete("patients/{id}")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(bool))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> DeletePatientFromWorkbooks(int id, [FromBody] WorkbooksPatientViewModel workbookspatientVM)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var result = await _workbooks.RemovePatientFromWorkbooks(currentUserId, workbookspatientVM.FormResponseId, id);
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
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdateWorkbooksProvider(int id, [FromBody] WorkbooksProviderViewModel workbooksproviderVM)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                await _workbooks.UpdateWorkbooksProviders(currentUserId, workbooksproviderVM.FormResponseID, id, workbooksproviderVM.PHQS, workbooksproviderVM.TOTAL);
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
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(WorkbooksPatientFollowupViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetWorkbooksPatientPHQ9FollowUp(int formResponseId, int patientID)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _workbooks.GetWorkbooksPatientPHQ9FollowUp(currentUserId, formResponseId, patientID);
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
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(WorkbooksPatientFollowupViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
       
        public async Task<IActionResult> UpdateWorkbooksPatientFollowup(int id, [FromBody]WorkbooksPatientFollowupViewModel wbptfollowup)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                await _workbooks.UpdateWorkbooksPatientFollowup(currentUserId, wbptfollowup.FormResponseId, wbptfollowup.PatientId, wbptfollowup.ActionPlanGiven, wbptfollowup.ManagedByExternalProvider, wbptfollowup.DateOfLastCommunicationByExternalProvider, wbptfollowup.FollowupPhoneCallOneToTwoWeeks, wbptfollowup.DateOfFollowupCall, wbptfollowup.OneMonthFollowupVisit, wbptfollowup.DateOfOneMonthVisit, wbptfollowup.OneMonthFolllowupPHQ9Score);
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
