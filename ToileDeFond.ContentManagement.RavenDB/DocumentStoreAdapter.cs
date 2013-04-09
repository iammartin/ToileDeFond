using System;
using System.ComponentModel.Composition;
using Raven.Client;
using ToileDeFond.Modularity;

namespace ToileDeFond.ContentManagement.RavenDB
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DocumentStoreAdapter : IDisposable
    {
        readonly IDocumentStoreFactory _factory;
        private IDocumentStore _documentStore;

        [ImportingConstructor]
        public DocumentStoreAdapter(IDocumentStoreFactory factory)
        {
            _factory = factory;
        }

        [PrioritisedExport(typeof(IDocumentStore))]
        public IDocumentStore DocumentStore
        {
            get
            {
                return _documentStore ?? (_documentStore = _factory.Create());
            }
        }

        public void Dispose()
        {
            if (_documentStore != null)
                _documentStore.Dispose();
        }
    }
}