using Raven.Client;

namespace ToileDeFond.ContentManagement.RavenDB
{
    public interface IDocumentSessionFactory
    {
        IDocumentSession Create();
    }
}