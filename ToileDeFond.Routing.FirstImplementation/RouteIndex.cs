using Raven.Abstractions.Indexing;
using ToileDeFond.ContentManagement.RavenDB;

namespace ToileDeFond.Routing.FirstImplementation
{
    public class RouteIndex : ContentTranslationVersionIndex
    {
        public override IndexDefinition CreateIndexDefinition()
        {
            var indexDefinition = base.CreateIndexDefinition();

            indexDefinition.Stores.Add("RewriteFromUrl", FieldStorage.Yes);
            indexDefinition.Stores.Add("RewriteToUrl", FieldStorage.Yes);
            indexDefinition.Map = BuildMapWithCustomQuery(DefaultQuery + " where version.ContentType == \"ToileDeFond.Routing.FirstImplementation.Route\"", new[] { "RewriteFromUrl", "RewriteToUrl" });

            return indexDefinition;
        }
    }
}