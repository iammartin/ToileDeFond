using System;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using ClaySharp;
using NUnit.Framework;
using Newtonsoft.Json;
using Raven.Client;
using ToileDeFond.Modularity;

namespace ToileDeFond.Tools
{
    public class RavenDBDiscovery
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            new Starter().Start();
        }

        [Test]
        [Ignore]
        public void AddDefaultRoutes()
        {
            dynamic New = new ClayFactory();

            var document = New.Array(
                New.Person(
                    FirstName: "Louis",
                    LastName: "Dejardin",
                    Aliases: new[] { "Lou" }
                ),
                New.Person(
                    FirstName: "Bertrand",
                    LastName: "Le Roy"
                ).Aliases("bleroy", "boudin"),
                New.Person(
                    FirstName: "Renaud",
                    LastName: "Paquay"
                ).Aliases("Your Scruminess", "Chef")
            ).Name("Some Orchard folks").Id(Guid.NewGuid());


            //var document = new Document {Id = Guid.NewGuid()};

          var serializedDocument =  JsonConvert.SerializeObject(document, Formatting.Indented, new ClayJsonConverter());

            var x = serializedDocument;

            //using (var documentSession = DependencyResolver.Current.GetService<IDocumentSession>())
            //{
            //    documentSession.Store(document);
            //    documentSession.SaveChanges();
            //}

            //dynamic loadedDocument;
            //using (var documentSession = DependencyResolver.Current.GetService<IDocumentSession>())
            //{
            //    loadedDocument = documentSession.Load<Document>(document.Id);
            //}

            //Assert.IsTrue(Extensions.PublicInstancePropertiesEqual<Document>(loadedDocument, document));
        }
    }


    //https://github.com/ravendb/ravendb/blob/master/Raven.Abstractions/Json/JsonDynamicConverter.cs
}
