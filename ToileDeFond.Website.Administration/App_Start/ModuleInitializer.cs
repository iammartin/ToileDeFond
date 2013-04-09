using Raven.Client;
using Raven.Client.Indexes;
using ToileDeFond.Modularity;

namespace ToileDeFond.Website.Administration.App_Start
{
    public class ModuleInitializer : ModuleInitializerBase
    {
        public override void Initialize(IDependencyResolver dependencyResolver)
        {
            IndexCreation.CreateIndexes(typeof(PropertyEditorIndex).Assembly, DependencyResolver.Current.GetService<IDocumentStore>());
        }
    }
}