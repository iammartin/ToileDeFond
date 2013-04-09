using System;
using System.ComponentModel.Composition;
using Raven.Client;
using ToileDeFond.Modularity;

namespace ToileDeFond.ContentManagement.RavenDB
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DocumentSessionAdapter : IDisposable
    {
        readonly IDocumentSessionFactory _factory;
        private IDocumentSession _documentSession;

        [ImportingConstructor]
        public DocumentSessionAdapter(IDocumentSessionFactory factory)
        {
            _factory = factory;
        }

        [PrioritisedExport(typeof(IDocumentSession))]
        public IDocumentSession DocumentSession
        {
            get
            {
                return _documentSession ?? (_documentSession = _factory.Create());
            }
        }

        public void Dispose()
        {
           if(_documentSession != null)
               _documentSession.Dispose();
        }
    }
}