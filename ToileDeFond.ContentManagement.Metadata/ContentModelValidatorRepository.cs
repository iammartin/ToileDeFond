using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using ToileDeFond.ContentManagement;
using ToileDeFond.ContentManagement.ModuleBuilding;
using ToileDeFond.Modularity;
using Raven.Client;

namespace ToileDeFond.Metadata.ContentManagement
{
    [PrioritisedExport(typeof(IContentModelValidatorRepository))]
    public class ContentModelValidatorRepository : IContentModelValidatorRepository
    {
        private readonly IReflectionContentManager _contentManager;
        private readonly IDocumentSession _documentSession;

        public ContentModelValidatorRepository(IReflectionContentManager contentManager, IDocumentSession documentSession)
        {
            _contentManager = contentManager;
            _documentSession = documentSession;
        }

        public IEnumerable<Content> GetValidationRulesForContentTypeProperty(string propertyName)
        {

            var routeContent = _documentSession.Advanced.LuceneQuery<Content, DraftRoutesByRewriteFromUrlIndex>()
                    .WhereEquals("RewriteFromUrl", rewriteFromUrl).FirstOrDefault();
            //contents = new List<Content> { session.Load<Content>(content.Id) };

            if (routeContent == null)
                return null;

            return _reflectionContentManager.GetObjectFromContent<Route>(routeContent, /*TODO: Ignore case ? */
                routeContent.Translations.First(t => t["RewriteFromUrl"].GetValue<string>().Equals(rewriteFromUrl, StringComparison.OrdinalIgnoreCase)).Culture);
        }

        public IEnumerable<Content> GetValidationRulesForContentType(string fullName)
        {
            throw new System.NotImplementedException();
        }
    }
}