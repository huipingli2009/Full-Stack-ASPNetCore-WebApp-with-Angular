using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using org.cchmc.pho.identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace org.cchmc.pho.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ILogger<ContactsController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IContact _contact;

        public ContactsController(ILogger<ContactsController> logger, IMapper mapper, IUserService userService, IContact contact)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _contact = contact;
        }

        [HttpGet()]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(List<ContactViewModel>))]      
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetContacts(bool? qpl, string specialty, string membership, string board, string namesearch)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);

                var data = await _contact.GetContacts(currentUserId, qpl, specialty, membership, board, namesearch);
                var result = _mapper.Map<List<ContactViewModel>>(data);

                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, $"An error occurred: {ex.Message}");
                return StatusCode(500, "Error occurred when fetching contacts");
            }
        }        
        
        [HttpGet("{contact}")]        
        [Authorize(Roles = "Practice Member,Practice Coordinator,Practice Admin,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(ContactPracticeDetailsVidewModel))]
        [SwaggerResponse(400, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetContactPracticeDetails(string contact)
        {
            if (!int.TryParse(contact, out var contactId))
            {                 
                _logger.LogError($"Failed to parse contactId - {contact}");
                return BadRequest("contact is not a valid integer");
            }

            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                // call the data method
                var data = await _contact.GetContactPracticeDetails(currentUserId, contactId);
                // perform the mapping from the data layer to the view model (if you want to expose/hide/transform certain properties)
                var result = _mapper.Map<ContactPracticeDetailsVidewModel>(data);
                // return the result in a "200 OK" response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, $"An error occurred: {ex.Message}");                
                return StatusCode(500, "Error occurred when fetching contacts");
            }
        }

        [HttpGet("practicelocations")]
        [Authorize(Roles = "Practice Member,Practice Coordinator,Practice Admin,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(List<ContactPracticeLocationViewModel>))]     
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetContactPracticeLocations(int practiceId)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);

                var data = await _contact.GetContactPracticeLocations(currentUserId, practiceId);
                var result = _mapper.Map<List<ContactPracticeLocationViewModel>>(data);

                // return the result in a "200 OK" response
                return Ok(result);

            }
            catch(Exception ex)
            {               
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, $"An error occurred: {ex.Message}");
                return StatusCode(500, "Error occurred when fetching contacts");
            }
        }

        [HttpGet("contactstafflist")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(List<ContactPracticeStaffViewModel>))]      
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetContactPracticeStaffList(int practiceId)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);

                var data = await _contact.GetContactPracticeStaffList(currentUserId, practiceId);
                var result = _mapper.Map<List<ContactPracticeStaffViewModel>>(data);

                return Ok(result);
            }
            catch (Exception ex)
            {
                // log any exceptions that happen and return the error to the user
                _logger.LogError(ex, $"An error occurred: {ex.Message}");
                return StatusCode(500, "Error occurred when fetching contacts");
            }
        }

        [HttpGet("contactstaffdetails")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(ContactPracticeStaffDetailsViewModel))]     
        [SwaggerResponse(500, type: typeof(string))]

        public async Task<IActionResult> GetContactStaffDetails(int staffId)
        {
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);

                var data = await _contact.GetContactStaffDetails(currentUserId, staffId);
                var result = _mapper.Map<ContactPracticeStaffDetailsViewModel>(data);

                return Ok(result);
            }

            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occured: {ex.Message}");
                return StatusCode(500, "Error occurred when fetching contacts");
            }
        }

        //dropdown list for specialty
        [HttpGet("contactspecialtylist")]        
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(List<SpecialtyViewModel>))]       
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetContactPracticeSpecialties()
        {
            try
            {
                var data = await _contact.GetContactPracticeSpecialties();
                var result = _mapper.Map<List<SpecialtyViewModel>>(data);

                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occured: {ex.Message}");
                return StatusCode(500, "Error occurred when fetching specialty");
            }
        }
        
        //dropdown list for PHO membership
        [HttpGet("phomembership")]
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(List<PHOMembershipViewModel>))]      
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetContactPracticePHOMembership()
        {
            try
            {
                var data = await _contact.GetContactPracticePHOMembership();
                var result = _mapper.Map<List<PHOMembershipViewModel>>(data);

                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occured: {ex.Message}");
                return StatusCode(500, "Error occurred when fetching PHO membership");
            }
        }

        //dropdown list for boardship
        [HttpGet("boardmembership")]       
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(List<BoardMembershipViewModel>))]       
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetContactPracticeBoardMembership()
        {
            try
            {
                var data = await _contact.GetContactPracticeBoardMembership();
                var result = _mapper.Map<List<BoardMembershipViewModel>>(data);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured: {ex.Message}");
                return StatusCode(500, "Error occurred when fetching board membership");
            }
        }

        //get contact staff email list
        [HttpGet("contactemaillist")]     
        [Authorize(Roles = "Practice Member,Practice Admin,Practice Coordinator,PHO Member,PHO Admin, PHO Leader")]
        [SwaggerResponse(200, type: typeof(List<StaffViewModel>))]    
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IActionResult> GetContactEmailList(bool? managers, bool? admins, bool? all)
        {  
            try
            {
                int currentUserId = _userService.GetUserIdFromClaims(User?.Claims);
                var data = await _contact.GetContactEmailList(currentUserId, managers, admins, all);

                var result = _mapper.Map<List<StaffViewModel>>(data);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured: {ex.Message}");
                return StatusCode(500, "Error occurred when fetching contact staff email list");
            }
          
        }
        //leave the coding below for future use
        // GET api/<ContactsController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<ContactsController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<ContactsController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<ContactsController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
