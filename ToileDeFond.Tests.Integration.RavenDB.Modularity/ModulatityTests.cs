using NUnit.Framework;
using Raven.Client;
using ToileDeFond.ContentManagement;
using ToileDeFond.Modularity;
using ToileDeFond.Utilities.RavenDB;

namespace ToileDeFond.Tests.Integration.RavenDB.Modularity
{
    public class ModulatityTests : InMemory.Modularity.ModulatityTests
    {
        //http://mef.codeplex.com/wikipage?title=Parts%20Lifetime 

        [Test]
        public void DocumentStoresShouldBeShared()
        {
            var first = DependencyResolver.Current.GetService<IDocumentStore>();
            var second = DependencyResolver.Current.GetService<IDocumentStore>();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void DocumentSessionsShouldNotBeShared()
        {
            var first = DependencyResolver.Current.GetService<IDocumentSession>();
            var second = DependencyResolver.Current.GetService<IDocumentSession>();

            Assert.AreNotEqual(first, second);
        }

        [Test]
        public void ContentManagerShouldNotBeShared()
        {
            var first = DependencyResolver.Current.GetService<IContentManager>();
            var second = DependencyResolver.Current.GetService<IContentManager>();

            Assert.AreNotEqual(first, second);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            RavenDBUtilities.DeleteAllDocumentsAndWaitForStaleIndexes(DependencyResolver.Current.GetService<IDocumentStore>());
        }
    }
}
