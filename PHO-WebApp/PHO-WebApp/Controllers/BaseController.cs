using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHO_WebApp.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            //Log the error!!
            string controllerName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();

            SharedLogic.LogError(Session["UserDetails"], controllerName, actionName, filterContext.Exception);

            //Redirect or return a view, but not both.
            //filterContext.Result = RedirectToAction("Error", "Base");
            // OR 
            //filterContext.Result = new ViewResult
            //{
            //    ViewName = "~/Views/Base/Error.cshtml"
            //};
        }

        public ActionResult Error()
        {
            return View("Error");
        }
    }
}