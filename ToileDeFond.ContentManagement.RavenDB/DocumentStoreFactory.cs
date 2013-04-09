using Raven.Client;
using Raven.Imports.Newtonsoft.Json;
using ToileDeFond.Utilities.RavenDB;

namespace ToileDeFond.ContentManagement.RavenDB
{
    public abstract class DocumentStoreFactory : IDocumentStoreFactory
    {
        public IDocumentStore Create()
        {
            var instance = RavenDBUtilities.CreateNewDocumentStoreInitializeAndCreateUtilIndexes(ConnectionStringName);

            instance.Conventions.CustomizeJsonSerializer = serializer =>
            {
                serializer.Converters.Add(new ContentJsonConverter());
                serializer.Converters.Add(new ModuleJsonConverter());
                serializer.Converters.Add(new MetadataConverter());
            };

            return instance;
        }

        protected abstract string ConnectionStringName { get; }
    }
}