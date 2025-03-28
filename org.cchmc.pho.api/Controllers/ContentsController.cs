﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Models;
using org.cchmc.pho.identity.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace org.cchmc.pho.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly ILogger<ContentsController> _logger;
        private readonly IMapper _mapper;
        private readonly IContent _content;

        public ContentsController(ILogger<ContentsController> logger, IMapper mapper, IContent content)
        {
            _logger = logger;
            _mapper = mapper;
            _content = content;
        }

        // GET: api/Content
        [HttpGet("spotlights")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
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

        // GET: api/Content
        [HttpGet("quicklinks")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(List<QuicklinkViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListActiveQuicklinks()
        {
            try
            {
                var data = await _content.ListActiveQuicklinks();

                var result = _mapper.Map<List<QuicklinkViewModel>>(data);

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
