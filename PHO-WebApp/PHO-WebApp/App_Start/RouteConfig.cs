using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PHO_WebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //Important note about routes!
            //The routing engine will try to make a match IN ORDER, the first match wins.
            //So the listing order here matters. Default should always be LAST, or it will mess up links.

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
             name: "Survey",
             url: "Survey",
             defaults: new { controller = "Survey", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
             name: "Cohort",
             url: "Cohort",
             defaults: new { controller = "Cohort", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "DataDictionary",
               url: "DataDictionary",
               defaults: new { controller = "Home", action = "DataDictionary", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Home",
               url: "Home",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Login",
               url: "Login",
               defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "CreateLogin",
               url: "Login",
               defaults: new { controller = "Login", action = "LoginForm", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );           

            routes.MapRoute(
            name: "Initiative",
            url: "Initiative",
            defaults: new { controller = "Initiative", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
            name: "Measure",
            url: "Measure",
            defaults: new { controller = "Measure", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
            name: "Staff",
            url: "Staff",
            defaults: new { controller = "Staffs", action = "Index", id = UrlParameter.Optional }
           );

        }
    }
}
