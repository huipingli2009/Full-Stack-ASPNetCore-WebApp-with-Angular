﻿using System;
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
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(List<WorkbooksDepressionPatientViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
     
        public async Task<IActionResult> GetDepressionPatientList(int formResponseId)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var data = await _workbooks.GetDepressionPatientList(currentUserId, formResponseId);

                var result = _mapper.Map<List<WorkbooksDepressionPatientViewModel>>(data);

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
        [HttpGet("asthmapatients/{id}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]       
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(List<WorkbooksAsthmaPatientViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]       
        public async Task<IActionResult> GetAsthmaPatientList(string id)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(id, out var formResponseId))
                return BadRequest("id is not a valid integer");

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
               
                var data = await _workbooks.GetAsthmaPatientList(currentUserId, formResponseId);

                var result = _mapper.Map<List<WorkbooksAsthmaPatientViewModel>>(data);

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
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
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
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
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

        [HttpGet("lookups/")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(List<WorkbooksLookupViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetPracticeWorkbooksLookups(int formId, string nameSearch)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _workbooks.GetWorkbooksLookups(formId, currentUserId, nameSearch);
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

        [HttpPost("depressionpatients/{id}")]  
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]        
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> AddPatientToDepressionWorkbooks(string id, [FromBody] WorkbooksDepressionPatientViewModel workbookspatientVM)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(id, out var patientId))
                return BadRequest("id is not a valid integer");

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var result = await _workbooks.AddPatientToDepressionWorkbooks(currentUserId, workbookspatientVM.FormResponseId, patientId, workbookspatientVM.ProviderId, workbookspatientVM.DateOfService, int.Parse(workbookspatientVM.PHQ9_Score), workbookspatientVM.ActionFollowUp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPost("asthmapatients/{id}")]
        [AllowAnonymous]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]       
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> AddPatientToAsthmaWorkbooks(string id, [FromBody] WorkbooksAsthmaPatientViewModel workbookspatientVM)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(id, out var patientId))
                return BadRequest("id is not a valid integer");

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var result = await _workbooks.AddPatientToAsthmaWorkbooks(currentUserId, workbookspatientVM.FormResponseId, patientId, workbookspatientVM.ProviderId, workbookspatientVM.DateOfService, workbookspatientVM.Asthma_Score, workbookspatientVM.AssessmentCompleted, workbookspatientVM.Treatment.TreatmentId,  workbookspatientVM.ActionPlanGiven);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpDelete("patients/{formResponseId}/{id}")]        
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(bool))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> DeletePatientFromWorkbooks(int formResponseId,int id)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var result = await _workbooks.RemovePatientFromWorkbooks(currentUserId, formResponseId, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpPost("provider/{formResponseId}/{providerId}")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> AddProviderToWorkbooks(int formResponseId, int providerId)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var result = await _workbooks.AddProviderToWorkbooks(currentUserId, formResponseId, providerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpDelete("provider/{formResponseId}/{providerId}")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(bool))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> DeleteProviderFromWorkbooks(int formResponseId, int providerId)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var result = await _workbooks.RemoveProviderFromWorkbooks(currentUserId, formResponseId, providerId);
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
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
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
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
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
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(WorkbooksPatientFollowupViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
       
        public async Task<IActionResult> UpdateWorkbooksPatientFollowup(int id, [FromBody]WorkbooksPatientFollowupViewModel wbptfollowup)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                object p = await _workbooks.UpdateWorkbooksPatientFollowup(currentUserId, wbptfollowup.FormResponseId, wbptfollowup.PatientId, wbptfollowup.ActionPlanGiven, wbptfollowup.ManagedByExternalProvider, wbptfollowup.DateOfLastCommunicationByExternalProvider, wbptfollowup.FollowupPhoneCallOneToTwoWeeks, wbptfollowup.DateOfFollowupCall, wbptfollowup.OneMonthFollowupVisit, wbptfollowup.DateOfOneMonthVisit, wbptfollowup.OneMonthFolllowupPHQ9Score, wbptfollowup.PHQ9FollowUpNotes);
                return Ok();
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }

        }

        [HttpGet("WorkbooksForms")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(List<WorkbooksFormsViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetWorkbooksForms()
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _workbooks.GetWorkbooksForms(currentUserId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<WorkbooksFormsViewModel>>(data);
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


        [HttpGet("asthmatreatmentplan")]       
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(List<AsthmaTreatmentPlanViewModel>))] 
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetAsthmaTreatmentPlan()
        {
            try
            {
                // call the data method
                var data = await _workbooks.GetAsthmaTreatmentPlan();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<AsthmaTreatmentPlanViewModel>>(data);
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

        [HttpGet("asthmaworkbookspractice")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(AsthmaWorkbooksPracticeViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetAsthmaPracticeWorkbooks(int formResponseId)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _workbooks.GetAsthmaPracticeWorkbooks(currentUserId, formResponseId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<AsthmaWorkbooksPracticeViewModel>(data);
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

        [HttpGet("confirmation")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(WorkbooksDepressionConfirmationViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetDepressionConfirmations(int formResponseId)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _workbooks.GetDepressionConfirmation(currentUserId, formResponseId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<WorkbooksDepressionConfirmationViewModel>(data);
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

        [HttpPut("confirmation")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(bool))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]

        public async Task<IActionResult> UpdateWorkbooksDepressionConfirmations([FromBody]WorkbooksDepressionConfirmationViewModel confirm)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var dataModel = _mapper.Map<WorkbooksDepressionConfirmation>(confirm);
                await _workbooks.UpdateDepressionConfirmation(currentUserId, dataModel);
                return Ok();
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }

        }

        [HttpPut("qiconfirmation/{id}/{entered}")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdateWorkbooksQIConfirmation(string id, string entered, [FromBody]QuestionViewModel confirm)
        {
            // route parameters are strings and need to be translated (and validated) to their proper data type
            if (!int.TryParse(id, out var formResponseId))
                return BadRequest("id is not a valid integer");
            if (!Boolean.TryParse(entered, out var dataEntered))
                return BadRequest("dataEntered is not a valid bool");

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var dataModel = _mapper.Map<Question>(confirm);
                await _workbooks.UpdateQIQuestion(currentUserId, formResponseId, dataEntered, dataModel);
                return Ok();
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }

        }

        [HttpGet("practiceqiworkbooks")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(QIWorkbookPracticeViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetQIWorkbookPractice(int formResponseId)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _workbooks.GetPracticeQIWorkbooks(currentUserId, formResponseId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<QIWorkbookPracticeViewModel>(data);
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

        [HttpGet("qiworkbookquestions")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(QIWorkbookQuestionsViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetQIWorkbookQuestions(int formResponseId)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _workbooks.GetQIWorkbookQuestions(currentUserId, formResponseId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<QIWorkbookQuestionsViewModel>(data);
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
