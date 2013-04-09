using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ToileDeFond.ContentManagement;
using ToileDeFond.ContentManagement.RavenDB;
using ToileDeFond.ContentManagement.Reflection;
using ToileDeFond.Modularity;
using Raven.Abstractions.Data;
using Raven.Client;
using ToileDeFond.Utilities.RavenDB;

namespace ToileDeFond.Routing.FirstImplementation
{
    [PrioritisedExport(typeof(IRouteRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RouteRepository : IRouteRepository
    {
        private readonly IReflectionContentManager _reflectionContentManager;
        private readonly IDocumentSession _documentSession;
        private readonly IContentPublicationStateManager _contentPublicationStateManager;
        private readonly IContentPublicationDateTimeManager _contentPublicationDateTimeManager;

        //TODO: Faire en sorte que s'il n'y a pas de ImportingConstructor on prend le eager constructor - voir comme pour le controllers

        public RouteRepository(IReflectionContentManager contentManager, IDocumentSession documentSession,
            IContentPublicationStateManager contentPublicationStateManager,
            IContentPublicationDateTimeManager contentPublicationDateTimeManager)
        {
            _reflectionContentManager = contentManager;
            _documentSession = documentSession;
            _contentPublicationStateManager = contentPublicationStateManager;
            _contentPublicationDateTimeManager = contentPublicationDateTimeManager;
        }

        [ImportingConstructor]
        public RouteRepository(IReflectionContentManager contentManager, IDocumentSession documentSession,
          [ImportMany]IEnumerable<Lazy<IContentPublicationStateManager, IPrioritisedMefMetaData>> contentPublicationStateManagers,
        [ImportMany]IEnumerable<Lazy<IContentPublicationDateTimeManager, IPrioritisedMefMetaData>> contentPublicationDateTimeManagers)
            : this(contentManager, documentSession,
            contentPublicationStateManagers.OrderByDescending(x => x.Metadata.Priority).First().Value,
            contentPublicationDateTimeManagers.OrderByDescending(x => x.Metadata.Priority).First().Value)
        {

        }

        public void Dispose()
        {
            _reflectionContentManager.Dispose();
            //TODO: Enlever tout les IDisposable et utiliser un sessionfactory partout
        }

        public IRoute GetRouteByRewriteFromUrl(string rewriteFromUrl)
        {
            var route = _documentSession.Advanced.LuceneQuery<Content,RouteIndex>()
                .WhereEquals("RewriteFromUrl", rewriteFromUrl)
                .AddContentManagementQueryTerms(_contentPublicationStateManager, _contentPublicationDateTimeManager)
                         .SelectFields<Route>().FirstOrDefault();

            return route;
        }

        public void AddRoute(IRoute route, Publication publication = null)
        {
            var contentReport = _reflectionContentManager.GetNewOrUpdatedContent(route as Route);

            if (publication != null)
                contentReport.Item.CreateTranslationVersions(publication);

            _reflectionContentManager.Store(contentReport.Item);
        }

        public void SaveChanges()
        {
            _reflectionContentManager.SaveChanges();
        }

        public void DeleteAllRoutes()
        {
            RavenDBUtilities.WaitForStaleIndexes(_documentSession.Advanced.DocumentStore);

            _documentSession.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex("RouteIndex", new IndexQuery());

            RavenDBUtilities.WaitForStaleIndexes(_documentSession.Advanced.DocumentStore);
        }
    }
}