using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace org.cchmc.pho.api.Controllers
{
    [Route("api/contents")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly ILogger<ContentController> _logger;
        private readonly IMapper _mapper;
        private readonly IContent _content;

        //TODO: delete me refactor
        private readonly CustomOptions _customOptions;

        public ContentController(ILogger<ContentController> logger, IMapper mapper, IContent content)
        {
            _logger = logger;
            _mapper = mapper;
            _content = content;
            
            _logger.LogInformation($"Example of options {_customOptions?.RequiredOption}");
        }

        // GET: api/Content
        [HttpGet("spotlights")]

        //need to build vViewModel?
        [SwaggerResponse(200, type: typeof(List<SpotLightViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListActiveSpotLights()
        {
            try
            {
                var data = await _content.ListActiveSpotLights();

                var result = _mapper.Map<List<SpotLightViewModel>>(data);

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
