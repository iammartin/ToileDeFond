using Raven.Abstractions.Indexing;

namespace ToileDeFond.ContentManagement.RavenDB
{
    public class ContentTranslationVersionByNameIndex : ContentTranslationVersionIndex
    {
        public override IndexDefinition CreateIndexDefinition()
        {
            var indexDefinition = base.CreateIndexDefinition();

            indexDefinition.Stores.Add("Name", FieldStorage.Yes);
            indexDefinition.Map = BuildMap("Name");

            return indexDefinition;
        }
    }
}