using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PHO_WebApp.Models;
using PHO_WebApp.DataAccessLayer;

namespace PHO_WebApp
{
    public static class SharedLogic
    {
        public static void LogAudit(object userDetails, string controller, string action, string message)
        {
            AuditDAL dal = new AuditDAL();
            try
            {
                UserDetails user = null;
                if (userDetails != null)
                {
                    user = (UserDetails)userDetails;
                }
                dal.LogAudit(user, controller, action, message);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
            }
        }
        public static void LogError(object userDetails, string controller, string action, Exception ex)
        {
            AuditDAL dal = new AuditDAL();
            try
            {
                UserDetails user = null;
                if (userDetails != null)
                {
                    user = (UserDetails)userDetails;
                }

                string innerMessage = null;
                if (ex.InnerException != null)
                {
                    innerMessage = ex.InnerException.Message;
                }
             
                dal.LogError(user, controller, action, ex.Message, innerMessage, ex.StackTrace);
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }
        }

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

        public static DateTime ParseDateTime(string value)
        {
            DateTime returnValue = new DateTime();

            if (!string.IsNullOrWhiteSpace(value))
            {
                DateTime holder;
                DateTime.TryParse(value, out holder);
                if (holder != null)
                {
                    returnValue = holder;
                }
            }

            return returnValue;
        }
        public static DateTime? ParseDateTimeNullable(string value)
        {
            DateTime? returnValue = new DateTime?();

            if (!string.IsNullOrWhiteSpace(value))
            {
                DateTime holder;
                DateTime.TryParse(value, out holder);
                if (holder != null)
                {
                    returnValue = holder;
                }
            }

            return returnValue;
        }

    }
}