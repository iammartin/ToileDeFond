using ToileDeFond.Modularity;
using Raven.Client;
using Raven.Client.Indexes;

namespace ToileDeFond.Routing.FirstImplementation
{
    public class ModuleInitializer : ModuleInitializerBase
    {
        public override void Initialize(IDependencyResolver dependencyResolver)
        {
            IndexCreation.CreateIndexes(typeof(RouteIndex).Assembly, DependencyResolver.Current.GetService<IDocumentStore>());
        }
    }
}
