using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PHO_WebApp.Models
{
    public class UserDetails
    {
        public int LoginId { get; set; }

        public int StaffId { get; set; }

        [Required(ErrorMessage = "User name is required.")]        
              
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string LoginError { get; set; }

        public int StaffTypeId { get; set; }

        public List<StaffType> AllStaffTypes { get; set; }

        public string SessionId { get; set; }

        public int PracticeId { get; set; }
        public string PracticeName { get; set; }

        public bool IsLoggedIn
        {
            get
            {
                if (LoginId > 0)
                {
                    return true;
                }
                return false;
            }
        }
    }
}