using System.Collections.Generic;
using System.Linq;

namespace ToileDeFond.Modularity
{
    public static class DependencyResolverExtensions
    {
        public static T GetService<T>(this IDependencyResolver dependencyResolver)
        {
            return (T) dependencyResolver.GetService(typeof (T));
        }

        public static IEnumerable<T> GetServices<T>(this IDependencyResolver dependencyResolver)
        {
            return dependencyResolver.GetServices(typeof (T)).Cast<T>();
        }
    }
}