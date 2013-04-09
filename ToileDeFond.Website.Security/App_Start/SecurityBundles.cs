using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using ToileDeFond.Modularity.Web;

namespace ToileDeFond.Website.Security.App_Start
{
    [Export(typeof(IBundleConfig))]
    public class SecurityBundles : IBundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/content/libraries/bundles/security/js").Include(
                 "~/content/js/jquery-{version}.js",
                  "~/Content/js/jquery.validate-{version}.js",
                  "~/Content/js/jquery.validate.unobtrusive-{version}.js",
                  "~/Content/libraries/bootstrap/js/bootstrap.js",
                  "~/Content/libraries/security/main-{version}.js"));

            bundles.Add(new StyleBundle("~/content/libraries/bundles/security/css").Include(
               "~/Content/libraries/bootstrap/css/bootstrap.css",
               "~/Content/libraries/security/main.css"));
        }
    }
}