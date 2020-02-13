using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.identity.Models;
using org.cchmc.pho.api.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Options;
using org.cchmc.pho.core.Models;
using org.cchmc.pho.identity.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace org.cchmc.pho.api.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        //TODO: delete me refactor
        private readonly CustomOptions _customOptions;

        public UserController(ILogger<UserController> logger, IMapper mapper, IUserService userService, IOptions<CustomOptions> customOptions)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;

            //TODO : CAN add some validation on this
            _customOptions = customOptions.Value;

            _logger.LogInformation($"Example of options {_customOptions?.RequiredOption}");
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        [SwaggerResponse(200, type: typeof(AuthenticationResult))]
        [SwaggerResponse(401, description: "User not found or password did not match")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest userParam)
        {
            var user = await _userService.Authenticate(userParam.Username, userParam.Password);
            if (user == null) return Unauthorized(new AuthenticationResult { Status = "User not found or password did not match" });

            return Ok(new AuthenticationResult { Status = "Authorized", Token = user.Token });
        }
    }
}
