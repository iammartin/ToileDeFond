using System.ComponentModel.Composition;
using ToileDeFond.Modularity;

namespace ToileDeFond.Routing.FirstImplementation
{
    [PrioritisedExport(typeof(IUrlRewriterFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UrlRewriterFactory : IUrlRewriterFactory
    {
        public IUrlRewriter Create()
        {
            return DependencyResolver.Current.GetService<IUrlRewriter>();
        }
    }
}
