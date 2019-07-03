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
        public int Id { get; set; } //Id in [PHO].[reg].[Login]

        public int PersonId { get; set; } //PersonId in [PHO].[reg].[Login], similar to user id
        

        [Required(ErrorMessage = "User name is required.")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public string LoginError { get; set; }

        public int StaffTypeId { get; set; }

        public List<StaffType> AllStaffTypes { get; set; }
    }
}