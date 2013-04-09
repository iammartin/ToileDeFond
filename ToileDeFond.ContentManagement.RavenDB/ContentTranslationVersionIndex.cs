using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ToileDeFond.ContentManagement.RavenDB
{
    public class ContentTranslationVersionIndex : AbstractIndexCreationTask
    {
        public override IndexDefinition CreateIndexDefinition()
        {
            return new IndexDefinition
                {
                    Stores = new Dictionary<string, FieldStorage>
                        {
                            {"CultureName", FieldStorage.Yes},
                            {"Id", FieldStorage.Yes},
                            {"ContentType", FieldStorage.Yes},
                            {"ContentTypeId", FieldStorage.Yes},
                            {"ModuleId", FieldStorage.Yes},
                            {"PublicationCreationDate", FieldStorage.Yes},
                            {"PublicationStartingDate", FieldStorage.Yes},
                            {"PublicationEndingDate", FieldStorage.Yes},
                            {"CreationDate", FieldStorage.Yes}
                        },
                    Indexes = new Dictionary<string, FieldIndexing>
                            {
                                {"PublicationCreationDate", FieldIndexing.Analyzed},
                                {"PublicationStartingDate", FieldIndexing.Analyzed},
                                {"PublicationEndingDate", FieldIndexing.Analyzed},
                                {"CreationDate", FieldIndexing.Analyzed}
                            },
                    Map = BuildMap()
                };
        }

        //TODO: Empecher d'avoir des propriétés sur un contenttype/content qui ont l'un de ces noms
        public static readonly string[] BaseFields = { "CultureName", "Id", "ContentType", "ContentTypeId", "ModuleId",
           "PublicationCreationDate", "PublicationStartingDate", "PublicationEndingDate", "CreationDate" };

        protected const string FirstFieldPrefix = "version.";
        protected const string FieldPrefix = ", version.";
        protected const string DefaultQuery = "from content in docs.Contents from version in content.GetVersions";

        protected virtual string BuildMap(params string[] fields)
        {
           return BuildMapWithCustomQuery(DefaultQuery, fields);
        }

        protected virtual string BuildMapWithCustomQuery(string customQuery, IEnumerable<string> fields = null)
        {
            if (customQuery == null) throw new ArgumentNullException("customQuery");
            if(customQuery.Length == 0)
                throw new ArgumentException("customQuery cannot be empty - use BuildMap to use default query", "customQuery");

            var builder = new StringBuilder(customQuery);
            builder.Append(" select new { ");
            var isFirst = true;

            foreach (var field in BaseFields.Union(fields ?? Enumerable.Empty<string>()))
            {
                if (isFirst)
                {
                    builder.Append(FirstFieldPrefix);
                    isFirst = false;
                }
                else
                {
                    builder.Append(FieldPrefix);
                }

                builder.Append(field);
            }

            builder.Append(" };");

            return builder.ToString();
        }
    }
}
