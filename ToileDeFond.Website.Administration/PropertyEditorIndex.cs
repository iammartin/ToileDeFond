using Raven.Abstractions.Indexing;
using ToileDeFond.ContentManagement.RavenDB;

namespace ToileDeFond.Website.Administration
{
    public class PropertyEditorIndex : ContentTranslationVersionIndex
    {
        public override IndexDefinition CreateIndexDefinition()
        {
            var indexDefinition = base.CreateIndexDefinition();

            indexDefinition.Stores.Add("Name", FieldStorage.Yes);
            indexDefinition.Stores.Add("GetRoute", FieldStorage.Yes);
            indexDefinition.Stores.Add("PostRoute", FieldStorage.Yes);
            indexDefinition.Map = BuildMapWithCustomQuery(DefaultQuery + " where version.ContentType == \"ToileDeFond.Website.Administration.PropertyEditor\"", 
                new[] { "Name", "GetRoute", "PostRoute" });

            return indexDefinition;
        }
    }
}