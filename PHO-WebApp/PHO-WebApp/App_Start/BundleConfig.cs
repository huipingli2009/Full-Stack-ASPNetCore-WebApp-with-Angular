﻿using System.Web;
using System.Web.Optimization;

namespace PHO_WebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/DataTables/jQuery.dataTables.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-3.3.1.js")
                .Include("~/Scripts/jquery-ui-1.12.1.js")
                .Include("~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/themes/base/jquery-ui.css",
                      "~/Content/bootstrap.css",
                      "~/Content/fontawesome.min.css",
                      "~/Content/site.css",
                      "~/Content/solid.css",
                      "~/Content/DataTables/css/jquery.dataTables.min.css"));
        }
    }
}
