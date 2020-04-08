using System;
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
using Microsoft.AspNetCore.Http;

namespace org.cchmc.pho.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IFiles _filesDAL;


        public FilesController(ILogger<FilesController> logger, IUserService userService, IMapper mapper, IFiles filesDal)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _filesDAL = filesDal;
        }

        [AllowAnonymous]
        //[Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [HttpGet()]
        [SwaggerResponse(200, type: typeof(List<FileViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListFiles(int? resourceTypeId, int? initiativeId, string tag, bool? watch, string name)
        {

            try
            {
                int currentUserId = 16; //_userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _filesDAL.ListFiles(currentUserId, resourceTypeId, initiativeId, tag, watch, name);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<FileViewModel>>(data);
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

        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [HttpGet("{files}")]
        [SwaggerResponse(200, type: typeof(FileDetailsViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetFilesDetails(string files)
        {
            if (!int.TryParse(files, out var filesId))
            {
                _logger.LogInformation($"Failed to parse filesId - {files}");
                return BadRequest("files is not a valid integer");
            }

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _filesDAL.GetFilesDetails(currentUserId, filesId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<FilesDetailViewModel>(data);
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

        //[Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        //[HttpPut("{files}")]
        //[SwaggerResponse(200, type: typeof(FilesDetailViewModel))]
        //[SwaggerResponse(400, type: typeof(string))]
        //[SwaggerResponse(500, type: typeof(string))]
        //public async Task<IActionResult> UpdateFilesDetails([FromBody] FilesDetailViewModel filesDetailVM, string files)
        //{
        //    if (!int.TryParse(files, out var filesId))
        //    {
        //        _logger.LogInformation($"Failed to parse filesId - {files}");
        //        return BadRequest("files is not a valid integer");
        //    }

        //    if (filesDetailVM == null)
        //    {
        //        _logger.LogInformation($"filesDetails object is null");
        //        return BadRequest("files is null");
        //    }

        //    if (filesDetailVM.Id != filesId)
        //    {
        //        _logger.LogInformation($"filesDetails.Id and filesId to not match");
        //        return BadRequest("files id does not match");
        //    }

        //    try
        //    {
        //        // route parameters are strings and need to be translated (and validated) to their proper data type
        //        int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);

        //        if (!_filesDAL.IsFilesInSamePractice(currentUserId, filesId))
        //        {
        //            _logger.LogInformation($"files and user practices do not match");
        //            return BadRequest("files practice does not match user");
        //        }

        //        FilesDetail filesDetail = _mapper.Map<FilesDetail>(filesDetailVM);
        //        // call the data layer to mark the action
        //        var data = await _filesDAL.UpdateFilesDetails(currentUserId, filesDetail);
        //        var result = _mapper.Map<FilesDetailViewModel>(data);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        // log any exceptions that happen and return the error to the user
        //        _logger.LogError(ex, "An error occurred");
        //        return StatusCode(500, "An error occurred");
        //    }
        //}


        [HttpGet("tags")]
        [AllowAnonymous]
        //[Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<FileTagViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetFileTagsAll()
        {
            try
            {
                // call the data method
                var data = await _filesDAL.GetFileTagsAll();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<FileTagViewModel>>(data);
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
        [HttpGet("resources")]
        [AllowAnonymous]
        //[Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<ResourceViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetResourcesAll()
        {
            try
            {
                // call the data method
                var data = await _filesDAL.GetResourceAll();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<ResourceViewModel>>(data);
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

       [HttpGet("initiatives")]
       [AllowAnonymous]
       //[Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
       [SwaggerResponse(200, type: typeof(List<InitiativeViewModel>))]
       [SwaggerResponse(400, type: typeof(string))]
       [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetInitiativesAll()
        {
            try
            {
                // call the data method
                var data = await _filesDAL.GetInitiativeAll();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<InitiativeViewModel>>(data);
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
