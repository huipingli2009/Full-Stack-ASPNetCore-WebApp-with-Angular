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
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
           
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "Patients",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Patient", action = "GetPatients", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "DataDictionary",
               url: "DataDictionary",
               defaults: new { controller = "Home", action = "DataDictionary", }
           );

        }
    }
}
