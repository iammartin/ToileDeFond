using System.ComponentModel.Composition;
using System.Web.Optimization;
using ToileDeFond.Modularity.Web;

namespace ToileDeFond.Website.Administration.App_Start
{
    [Export(typeof(IBundleConfig))]
    public class AdminBundleConfig : IBundleConfig
    {
        public  void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/content/bundles/admin-header.js").Include(
                "~/content/libraries/modernizr.js",
                "~/content/libraries/respond.js"));

            var ie7css = new StyleBundle("~/content/bundles/admin-ie7.css")
                .Include("~/Content/libraries/administration/less/admin-ie7.less");
            ie7css.Transforms.Add(new LessMinify());
            bundles.Add(ie7css);

            bundles.Add(new ScriptBundle("~/content/bundles/admin-footer.js").Include(
                "~/Content/libraries/jquery/plugins/jquery.unobtrusive-ajax.js",
                "~/Content/libraries/jquery/plugins/jquery-validate/jquery.validate.js",
                "~/Content/libraries/jquery/plugins/jquery-validate/jquery.validate.unobtrusive.js",
                "~/content/libraries/bootstrap-fontawesome/js/bootstrap.js",
                "~/content/libraries/administration/js/main.js",
                "~/content/libraries/administration/js/EditContentTypeProperty.js",
                "~/content/libraries/jquery/plugins/lou-multi-select/js/jquery.multi-select.js"));

            var css = new StyleBundle("~/Content/bundles/admin.css")
                .Include("~/Content/libraries/administration/less/admin.less")
                .Include("~/Content/libraries/jquery/plugins/lou-multi-select/css/multi-select.css")
                .Include("~/Content/libraries/administration/css/main.css");
            css.Transforms.Add(new LessMinify());
            bundles.Add(css);  
        }
    }
}