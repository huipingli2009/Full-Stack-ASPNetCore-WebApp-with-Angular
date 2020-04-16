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
        
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [HttpGet()]
        [SwaggerResponse(200, type: typeof(List<FileViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> ListFiles(int? resourceTypeId, int? initiativeId, string tag, bool? watch, string name)
        {

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
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
        [HttpGet("{file}")]
        [SwaggerResponse(200, type: typeof(FileDetailsViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetFileDetails(string file)
        {
            if (!int.TryParse(file, out var fileId))
            {
                _logger.LogInformation($"Failed to parse filesId - {file}");
                return BadRequest("files is not a valid integer");
            }

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _filesDAL.GetFileDetails(currentUserId, fileId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<FileDetailsViewModel>(data);
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
        [AllowAnonymous]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [HttpPut()]
        [SwaggerResponse(200, type: typeof(FileDetailsViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdateFileDetails([FromBody] FileDetailsViewModel filesDetailVM)
        {
            if (filesDetailVM == null)
            {
                _logger.LogInformation($"filesDetails object is null");
                return BadRequest("files is null");
            }

            try
            {
                // route parameters are strings and need to be translated (and validated) to their proper data type
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);

                FileDetails filesDetail = _mapper.Map<FileDetails>(filesDetailVM);
                // call the data layer to mark the action
                var data = await _filesDAL.UpdateFileDetails(currentUserId, filesDetail);
                var result = _mapper.Map<FileDetailsViewModel>(data);
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
        [HttpPost()]
        [SwaggerResponse(200, type: typeof(FileDetailsViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> AddFileDetails([FromBody] FileDetailsViewModel filesDetailVM)
        {
            if (filesDetailVM == null)
            {
                _logger.LogInformation($"filesDetails object is null");
                return BadRequest("files is null");
            }

            try
            {
                // route parameters are strings and need to be translated (and validated) to their proper data type
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);

                FileDetails filesDetail = _mapper.Map<FileDetails>(filesDetailVM);
                // call the data layer to mark the action
                var data = await _filesDAL.AddFileDetails(currentUserId, filesDetail);
                var result = _mapper.Map<FileDetailsViewModel>(data);
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
        [HttpDelete("{file}")]
        [SwaggerResponse(200, type: typeof(FileDetailsViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> DeleteFile(string file)
        {
            if (!int.TryParse(file, out var fileId))
                return BadRequest("file is not a valid integer");

            try
            {
                // route parameters are strings and need to be translated (and validated) to their proper data type
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);

                // call the data layer to mark the action
                var data = await _filesDAL.RemoveFile(currentUserId, fileId);

                return Ok(data);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [HttpPut("watch/{file}")]
        [SwaggerResponse(200, type: typeof(FileDetailsViewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> UpdateFileWatchList(string file)
        {
            if (!int.TryParse(file, out var fileId))
                return BadRequest("resource is not a valid integer");

            try
            {
                // route parameters are strings and need to be translated (and validated) to their proper data type
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);

                // call the data layer to mark the action
                var data = await _filesDAL.UpdateFileWatch(currentUserId, fileId);

                return Ok(data);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpGet("tags")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
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
        
        [HttpGet("types")]
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
        [SwaggerResponse(200, type: typeof(List<FileTypeViewModel>))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetFileTypesAll()
        {
            try
            {
                // call the data method
                var data = await _filesDAL.GetFileTypesAll();
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<List<FileTypeViewModel>>(data);
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
        [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
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
       [Authorize(Roles = "Practice Member,Practice Admin,PHO Member,PHO Admin")]
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
