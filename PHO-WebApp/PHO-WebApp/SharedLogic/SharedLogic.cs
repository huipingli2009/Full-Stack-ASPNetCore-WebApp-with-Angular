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

        public static int ParseNumeric(string value)
        {
            int returnValue = -1;

            if (!string.IsNullOrWhiteSpace(value))
            {
                Int32.TryParse(value, out returnValue);
            }

            return returnValue;
        }

        public static int? ParseNumericNullable(string value)
        {
            int? returnValue = new int?();

            if (!string.IsNullOrWhiteSpace(value))
            {
                int holder = -1;
                int.TryParse(value, out holder);
                if (holder > -1)
                {
                    returnValue = holder;
                }
            }

            return returnValue;
        }

    }
}