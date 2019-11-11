using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PHO_WebApp.Models;
using System.Web.Configuration;

namespace PHO_WebApp.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            string redirectToErrorPage = WebConfigurationManager.AppSettings["RedirectToErrorPage"];
            
            //Log the error!!
            string controllerName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();

            SharedLogic.LogError(SavedUserDetails, controllerName, actionName, filterContext.Exception);
            
            if (!string.IsNullOrWhiteSpace(redirectToErrorPage) && redirectToErrorPage.ToUpper() == "TRUE")
            {
                filterContext.ExceptionHandled = true;

                //Redirect or return a view, but not both.
                filterContext.Result = RedirectToAction("Error", "Base");
            }
        }

        public ActionResult Error()
        {
            return View("Error");
        }

        protected int PracticeId
        {
            get
            {
                int returnValue = -1;
                if (SavedUserDetails != null)
                {
                    returnValue = SavedUserDetails.PracticeId;
                }
                return returnValue;
            }
        }

        protected UserDetails SavedUserDetails
        {
            get
            {
                UserDetails returnValue = null;
                if (Session["UserDetails"] != null && Session["UserDetails"] is UserDetails)
                {
                    returnValue = (UserDetails)Session["UserDetails"];
                }
                return returnValue;
            }
            set
            {
                if (value != null && value is UserDetails)
                {
                    Session["UserDetails"] = value;
                }
                else
                {
                    //bad object or null, reset
                    Session["UserDetails"] = null;
                }
            }
        }

    }
}