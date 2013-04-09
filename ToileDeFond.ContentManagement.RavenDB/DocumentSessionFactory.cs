using System.ComponentModel.Composition;
using ToileDeFond.Modularity;
using Raven.Client;

namespace ToileDeFond.ContentManagement.RavenDB
{
    [PrioritisedExport(typeof(IDocumentSessionFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DocumentSessionFactory : IDocumentSessionFactory
    {
        public IDocumentSession Create()
        {
            return DependencyResolver.Current.GetService<IDocumentStore>().OpenSession();
        }
    }
}
