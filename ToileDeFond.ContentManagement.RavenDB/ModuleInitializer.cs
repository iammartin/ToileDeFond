using ToileDeFond.Modularity;
using Raven.Client.Indexes;

namespace ToileDeFond.ContentManagement.RavenDB
{
    public class ModuleInitializer : ModuleInitializerBase
    {
        //PreInitialize = Avant que le DependencyResolver est été créé
        public override void PreInitialize()
        {
            //TODO: Revoir... ici on force ConventionDocumentStoreFactory.. humm!
            IndexCreation.CreateIndexes(typeof(ModulesByNameIndex).Assembly, new ConventionDocumentStoreFactory().Create());
        }

        //Initialize = Après que le DependencyResolver est été créé
        //public override void Initialize(IDependencyResolver dependencyResolver)
        //{
        //    IndexCreation.CreateIndexes(typeof(ModulesByNameIndex).Assembly, DependencyResolver.Current.GetService<IDocumentStore>());
        //}
    }
}
