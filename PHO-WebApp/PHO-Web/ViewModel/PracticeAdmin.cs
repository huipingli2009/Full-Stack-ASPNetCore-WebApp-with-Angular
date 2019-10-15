using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PHO_WebApp.Models;
using PHO_WebApp.DataAccessLayer;

namespace PHO_WebApp.ViewModel
{
    public class PracticeAdmin
    {
        //constructor
        public PracticeAdmin()
        {
            PracStaff = new List<Staff>();
            EventCommand = "StaffList";
        }
        public List<Staff> PracStaff { get; set; }
        //public UserDetails UserLogin { get; set; }

        public string EventCommand { get; set; }

        public void HandleRequest()
        {
            switch (EventCommand.ToLower())
            {
                case "stafflist":
                    GetStaffs();
                    break;                
                default:
                    break;
            }
        }
        private void GetStaffs()
        {
            StaffDAL std = new StaffDAL();
            PracStaff = std.getPracticeStaffs(); 
        }
    }
}