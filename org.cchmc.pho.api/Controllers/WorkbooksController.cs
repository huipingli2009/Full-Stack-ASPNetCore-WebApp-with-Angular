using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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

        public WorkbooksController(ILogger<WorkbooksController> logger, IMapper mapper, IWorkbooks workbooks)
        {
            _logger = logger;
            _mapper = mapper;
            _workbooks = workbooks;
        }

        // GET: api/Workbooks
        [HttpGet]
        [SwaggerResponse(200, type: typeof(List<WorkbooksPatientViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]

        public async Task<IActionResult> ListPatients(int userId, DateTime reportingDate, string nameSearch)       
        {
            try
            {
                var data = await _workbooks.ListPatients(userId, reportingDate, nameSearch);

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
    }
}
