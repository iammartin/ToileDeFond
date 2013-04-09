using System.Linq;
using System.Threading;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Imports.Newtonsoft.Json;

namespace ToileDeFond.Utilities.RavenDB
{
    public static class RavenDBUtilities
    {
        public static void WaitForStaleIndexes(IDocumentStore store)
        {
            if (store.DatabaseCommands.GetStatistics().StaleIndexes != null)
            {
                while (store.DatabaseCommands.GetStatistics().StaleIndexes.Any())
                {
                    Thread.Sleep(100);
                }
            }
        }

        public static IDocumentStore CreateNewDocumentStoreInitializeAndCreateUtilIndexes(string connectionStringName)
        {
            var documentStore = new DocumentStore
            {
                ConnectionStringName = connectionStringName /*"ToileDeFond.Tests"*//*,
                                     Conventions =
                                         {
                                             JsonContractResolver = new DefaultContractResolver(shareCache: true)
                                                    {
                                                        DefaultMembersSearchFlags =  BindingFlags.NonPublic | BindingFlags.Instance
                                                    },
                                             CustomizeJsonSerializer = serializer => serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects
                                         }*/
                //,
                //Conventions =
                //                        {
                //                            CustomizeJsonSerializer = serializer =>
                //                                                          {
                //                                                              serializer.Converters.Add(new ContentJsonConverter(() => doc));
                //                                                              serializer.Converters.Add(new ModuleJsonConverter());
                //                                                          }
                //                        }
                , Conventions =
                        {
                            CustomizeJsonSerializer = serializer =>
                                                            {
                                                                serializer.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                                                            }
                        }
            };

            documentStore.Initialize();

            IndexCreation.CreateIndexes(typeof(AllDocumentsById).Assembly, documentStore);
            return documentStore;
        }

        public static void DeleteAllDocumentsAndWaitForStaleIndexes(IDocumentStore documentStore)
        {
            WaitForStaleIndexes(documentStore);
            using (var documentSession = documentStore.OpenSession())
            {
                documentStore.DatabaseCommands.DeleteByIndex("AllDocumentsById", new IndexQuery());
                documentSession.SaveChanges();
            }
            WaitForStaleIndexes(documentStore);
        }

        public static void SaveChangesAndWaitForStaleIndexes(IDocumentSession session, IDocumentStore store)
        {
            session.SaveChanges();
            WaitForStaleIndexes(store);
        }
    }
}
