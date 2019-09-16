using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using PHO_WebApp.Models;
using FluentValidation.Mvc;

namespace PHO_WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            FluentValidationModelValidatorProvider.Configure();
        }

        void Session_Start(object sender, EventArgs e)
        {
            // your code here, it will be executed upon session start
            UserDetails userDetails = new UserDetails();
            userDetails.SessionId = Session.SessionID;
            Session["UserDetails"] = userDetails;

            SharedLogic.LogAudit(userDetails, "Session_Start", "Global.asax.cs", "Session Started");
        }
    }
}
