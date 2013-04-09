using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;

namespace ToileDeFond.Modularity.Web
{
    public class WebApiDependencyResolverConverter : System.Web.Http.Dependencies.IDependencyResolver
    {
        private readonly IDependencyResolver _dependencyResolver;

        public WebApiDependencyResolverConverter(IDependencyResolver dependencyResolver)
      {
          _dependencyResolver = dependencyResolver;
      }

        public void Dispose()
        {
            _dependencyResolver.Dispose();
        }

        public object GetService(Type serviceType)
        {
            return _dependencyResolver.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _dependencyResolver.GetServices(serviceType);
        }

        //TODO: http://www.asp.net/web-api/overview/extensibility/using-the-web-api-dependency-resolver
        public IDependencyScope BeginScope()
        {
            // This example does not support child scopes, so we simply return 'this'.
            return this; 
        }
    }
}
