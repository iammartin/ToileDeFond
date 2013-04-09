using System;
using System.Linq;
using Raven.Client;

namespace ToileDeFond.ContentManagement.RavenDB
{
    //TODO: Voir si les order by fonctionne dans le cas du publié et du cas non publié!!
    //et voir l'impact sur les query ayant des orderby pour le nom par exemple... ordre des orderby...
   public static class RavenDBExtensions
    {
        //http://ravendb.net/docs/faq/lucene-queries-examples?version=2
        //https://github.com/ravendb/ravendb/commit/721c50ea51ff7721928cca76de957e9f7d9e3786
        public static IDocumentQuery<T> AddContentManagementQueryTerms<T>(this IDocumentQuery<T> documentQuery, IContentPublicationStateManager contentPublicationStateManager, IContentPublicationDateTimeManager contentPublicationDateTimeManager)
        {
            if (contentPublicationStateManager.ContentPublicationStateIsDraft())
            {
                return documentQuery.OrderBy("CreationDate").AndAlso().WhereEquals("PublicationCreationDate", null);
            }

            var dateTime = contentPublicationDateTimeManager.GetContentPublicationDateTime().Value;

            return documentQuery.OrderByDescending("CreationDate").AndAlso()
                    .Not.WhereEquals("PublicationCreationDate", null)
                    .AndAlso().WhereGreaterThan("PublicationEndingDate", dateTime)
                    .AndAlso().WhereLessThanOrEqual("PublicationStartingDate", dateTime);
        }
    }
}
