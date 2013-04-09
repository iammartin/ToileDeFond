using System.Web;
using System.Web.Mvc;

namespace ToileDeFond.Modularity.Web
{
    public class ContainerHttpModule : IHttpModule
    {
        public void Dispose()
        {
            var httpModuleRetriever = System.Web.Mvc.DependencyResolver.Current.GetService<IHttpModuleRetriever>();

            foreach (var httpModule in httpModuleRetriever.Modules)
            {
                httpModule.Value.Dispose();
            }
        }

        public void Init(HttpApplication context)
        {
            var httpModuleRetriever = System.Web.Mvc.DependencyResolver.Current.GetService<IHttpModuleRetriever>();

            foreach (var httpModule in httpModuleRetriever.Modules)
            {
                httpModule.Value.Init(context);
            }
        }
    }
}
