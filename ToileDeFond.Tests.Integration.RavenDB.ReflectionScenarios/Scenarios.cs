using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Raven.Client;
using ToileDeFond.ContentManagement;
using ToileDeFond.ContentManagement.RavenDB;
using ToileDeFond.ContentManagement.Reflection;
using ToileDeFond.Modularity;
using ToileDeFond.Tests.Common;
using ToileDeFond.Tests.FakeModules.First;
using ToileDeFond.Utilities.RavenDB;

namespace ToileDeFond.Tests.Integration.RavenDB.ReflectionScenarios
{
    [TestFixture]
    public class Scenarios 
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            new Starter().Start();
      
        }

        [Test]
        public void FromClassInstanceToRavenDBAndBack()
        {
            var fakeClassInstance = new FakeClass
            {
                Age = 1,
                Date = new DateTime(2012, 12, 12, 12, 12, 12),
                Name = "Maxime Séguin"
            };

            using (var reflectionContentManager = DependencyResolver.Current.GetService<IReflectionContentManager>())
            {
                //TODO: Pas certain d'aimer que pour avoir un Content ça génère une query...
                var content = reflectionContentManager.GetNewOrUpdatedContent(fakeClassInstance).Item;
                reflectionContentManager.Store(content);
                reflectionContentManager.SaveChanges();
            }

            RavenDBUtilities.WaitForStaleIndexes(DependencyResolver.Current.GetService<IDocumentStore>());

            List<ContentTranslationVersion> contents;
            using (var session = DependencyResolver.Current.GetService<IDocumentSession>())
            {
                contents = session.Query<Content, ContentTranslationVersionIndex>()
                         .AsProjection<ContentTranslationVersion>()
                         .ToList();
            }

            Assert.That(contents.Count, Is.EqualTo(1));

            List<FakeClass> fakeClasses;
            using (var session = DependencyResolver.Current.GetService<IDocumentSession>())
            {
                fakeClasses = session.Query<Content, ContentTranslationVersionIndex>()
                         .AsProjection<FakeClass>()
                         .ToList();
            }

            Assert.That(fakeClasses.Count, Is.EqualTo(1));

            var savedFakeClassInstance = fakeClasses.ElementAt(0);

            Assert.That(savedFakeClassInstance.Age, Is.EqualTo(fakeClassInstance.Age));
            Assert.That(savedFakeClassInstance.Date, Is.EqualTo(fakeClassInstance.Date));
            Assert.That(savedFakeClassInstance.Name, Is.EqualTo(fakeClassInstance.Name));
        }

        //TODO: Comment pourrait-on query un content selon une propriété de ses sous content (index lucene)...
        [Test]
        [NUnit.Framework.Ignore]
        public void FromClassInstanceGraphToRavenDBAndBack()
        {
            //TODO: Pas certain que ce soit un scenario qu'on veuille supporter finalement (à voir)

            //var fakeClassGraphInstance = new FakeClassGraph
            //{
            //    Name = "Roberto",
            //    FakeClass = new FakeClass
            //                    {
            //                        Age = 1,
            //                        Date = new DateTime(2012, 12, 12, 12, 12, 12),
            //                        Name = "Maxime Séguin",
            //                    },
            //    FakeClasses = new List<FakeClass>
            //                      {
            //                          new FakeClass
            //                    {
            //                        Age = 1,
            //                        Date = new DateTime(2012, 12, 12, 12, 12, 12),
            //                        Name = "Maxime Séguin",
            //                    },
            //                    new FakeClass
            //                    {
            //                        Age = 1,
            //                        Date = new DateTime(2012, 12, 12, 12, 12, 12),
            //                        Name = "Maxime Séguin",
            //                    }
            //                      }
            //};

            //using (var reflectionContentManager = DependencyResolver.Current.GetService<IReflectionContentManager>())
            //{
            //    //TODO: Pas certain d'aimer que pour avoir un Content ça génère une query...
            //    var content = reflectionContentManager.GetNewOrUpdatedContent(fakeClassGraphInstance).Item;
            //    reflectionContentManager.Store(content);
            //    reflectionContentManager.SaveChanges();
            //}

            //RavenDBUtilities.WaitForStaleIndexes(DependencyResolver.Current.GetService<IDocumentStore>());

            //List<ContentTranslationVersion> contents;
            //using (var session = DependencyResolver.Current.GetService<IDocumentSession>())
            //{
            //    contents = session.Query<Content, ContentTranslationVersionIndex>()
            //             .AsProjection<ContentTranslationVersion>()
            //             .ToList();
            //}

            //Assert.That(contents.Count, Is.EqualTo(1));

            //List<FakeClassGraph> fakeClasses;
            //using (var session = DependencyResolver.Current.GetService<IDocumentSession>())
            //{
            //    fakeClasses = session.Query<Content, ContentTranslationVersionIndex>()
            //             .AsProjection<FakeClassGraph>()
            //             .ToList();
            //}

            //Assert.That(fakeClasses.Count, Is.EqualTo(1));

            //var savedFakeClassGraphInstance = fakeClasses.ElementAt(0);


            //Assert.That(savedFakeClassGraphInstance.Name, Is.EqualTo(fakeClassGraphInstance.Name));

            //Assert.That(savedFakeClassGraphInstance.FakeClass.Age, Is.EqualTo(fakeClassGraphInstance.FakeClass.Age));
            //Assert.That(savedFakeClassGraphInstance.FakeClass.Name, Is.EqualTo(fakeClassGraphInstance.FakeClass.Name));
            //Assert.That(savedFakeClassGraphInstance.FakeClass.Date, Is.EqualTo(fakeClassGraphInstance.FakeClass.Date));

            //Assert.That(savedFakeClassGraphInstance.FakeClasses.Count, Is.EqualTo(2));
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            RavenDBUtilities.DeleteAllDocumentsAndWaitForStaleIndexes(DependencyResolver.Current.GetService<IDocumentStore>());
        }
    }
}
