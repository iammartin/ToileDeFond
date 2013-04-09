using System.Configuration;
using ToileDeFond.Utilities.RavenDB;
using NUnit.Framework;
using Raven.Client;

namespace ToileDeFond.Tools
{
    public class RavenDBTools
    {
        private IDocumentStore _documentStore;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _documentStore = RavenDBUtilities.CreateNewDocumentStoreInitializeAndCreateUtilIndexes(ConfigurationManager.AppSettings["DefaultConnectionString"]);
        }

        [Test]
        [Ignore]
        public void ClearAll()
        {
            RavenDBUtilities.DeleteAllDocumentsAndWaitForStaleIndexes(_documentStore);
        }

        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown()
        {
            _documentStore.Dispose();
            _documentStore = null;
        }
    }
}