using Raven.Client;

namespace ToileDeFond.ContentManagement.RavenDB
{
    public interface IDocumentStoreFactory
    {
        IDocumentStore Create();
    }
}