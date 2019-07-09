using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PHO_WebApp.Models;

namespace PHO_WebApp
{
    public static class SharedLogic
    {
        public static bool IsAuthenticated(StaffTypeEnum[] authorizedStaffTypes, object userDetails)
        {
            bool returnValue = false;

            if (userDetails != null)
            {
                UserDetails user = (UserDetails)userDetails;
                if (user.StaffTypeId > 0)
                {
                    foreach (StaffTypeEnum stafftype in authorizedStaffTypes)
                    {
                        if (user.StaffTypeId == (int)stafftype)
                        {
                            returnValue = true;
                        }
                    }
                }
            }

            return returnValue;
        }

    }
}