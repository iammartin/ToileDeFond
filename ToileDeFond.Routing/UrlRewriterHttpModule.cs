using System;
using System.ComponentModel.Composition;
using System.Web;
using ToileDeFond.Modularity.Web;

namespace ToileDeFond.Routing
{
    [AutoRegisterHttpModule]
    public class UrlRewriterHttpModule : IHttpModule
    {
        public const string UrlRewriterCurrentRouteKey = "UrlRewriterCurrentRouteKey";

        private readonly IUrlRewriterFactory _urlRewriterFactory;

        [ImportingConstructor]
        public UrlRewriterHttpModule(IUrlRewriterFactory urlRewriterFactory)
        {
            _urlRewriterFactory = urlRewriterFactory;
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;
        }

        private void BeginRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;

            if (app == null) return;

            //http://blogs.thesitedoctor.co.uk/tim/2011/02/21/Beware+ContextRewritePath+Does+Not+End+The+Current+Execution+Path.aspx
            _urlRewriterFactory.Create().UrlRewrite(app);
        }

        public void Dispose()
        {

        }
    }
}
