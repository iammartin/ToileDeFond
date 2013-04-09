using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Optimization;
using ToileDeFond.Modularity;
using ToileDeFond.Modularity.Web;

namespace ToileDeFond.Website.Administration.App_Start
{
    [PrioritisedExport(typeof(IBundleConfig))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AdministrationBundles : IBundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/content/libraries/bundles/administration/js").Include(
                "~/content/js/jquery-{version}.js",
                 "~/Content/js/jquery.validate-{version}.js",
                 "~/Content/js/jquery.validate.unobtrusive-{version}.js",
                "~/Content/libraries/bootstrap/js/bootstrap.js",
                 "~/Content/libraries/administration/main-{version}.js"));


            bundles.Add(new StyleBundle("~/content/libraries/bundles/administration/css").Include(
                 "~/Content/libraries/bootstrap/css/bootstrap.css",
                "~/Content/libraries/administration/main.css"));
        }
    }
}