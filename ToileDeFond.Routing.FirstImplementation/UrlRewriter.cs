using System.ComponentModel.Composition;
using System.Web;
using ToileDeFond.Modularity;

namespace ToileDeFond.Routing.FirstImplementation
{
    [PrioritisedExport(typeof(IUrlRewriter))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UrlRewriter : UrlRewriterBase
    {
        private readonly IRouteRepository _routeRepository;

        protected override bool BypassUrlRewriter(HttpApplication httpApplication)
        {
            return base.BypassUrlRewriter(httpApplication) || IsWebAPI(GetLocalPath(httpApplication));
        }

        protected virtual bool IsWebAPI(string localPath)
        {
            return localPath.StartsWith("/api/");
        }

        [ImportingConstructor]
        public UrlRewriter(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }

        protected override IRoute GetRoute(HttpApplication httpApplication)
        {
            var rewriteFromUrl = httpApplication.Request.Url.LocalPath.ToLower().TrimStart(new[] { '/' });

            return _routeRepository.GetRouteByRewriteFromUrl(rewriteFromUrl);

        }
    }
}