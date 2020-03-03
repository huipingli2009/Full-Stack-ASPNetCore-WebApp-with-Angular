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
    public class WorkbooksController : ControllerBase
    {
        private readonly ILogger<WorkbooksController> _logger;
        private readonly IMapper _mapper;
        private readonly IWorkbooks _workbooks;

        //todo: hardcoded userid for now, we will be using session later
        private readonly int _DEFAULT_USER = 3;

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
        public async Task<IActionResult> ListPatients(int formResponseId, string nameSearch)
        {           
            try
            {
                var data = await _workbooks.ListPatients(_DEFAULT_USER, formResponseId, nameSearch);

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
            try
            {
                // call the data method
                var data = await _workbooks.GetPracticeWorkbooks(_DEFAULT_USER, formResponseId);
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
            try
            {
                // call the data method
                var data = await _workbooks.GetPracticeWorkbooksProviders(_DEFAULT_USER, formResponseId);
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
        public async Task<IActionResult> GetPracticeWorkbooksLookups()
        {           
            try
            {
                // call the data method
                var data = await _workbooks.GetWorkbooksLookups(_DEFAULT_USER);
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
    }
}
