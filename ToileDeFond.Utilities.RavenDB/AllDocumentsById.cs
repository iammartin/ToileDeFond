using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ToileDeFond.Utilities.RavenDB
{
    public class AllDocumentsById : AbstractIndexCreationTask
    {
        public override IndexDefinition CreateIndexDefinition()
        {
            return new IndexDefinition
                       {
                           Name = "AllDocumentsById",
                           Map = "from doc in docs let DocId = doc[\"@metadata\"][\"@id\"] select new {DocId};"
                       };
        }
    }
}