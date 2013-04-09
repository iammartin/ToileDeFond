using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Newtonsoft.Json;
using ToileDeFond.ContentManagement;
using ToileDeFond.Modularity;
using ToileDeFond.Tests.Common;
using ToileDeFond.Utilities;

namespace ToileDeFond.Tests.Integration.InMemory.ContentScenarios
{
    //TODO: Re-permettre d'overrider à tous les niveaux (defaultvalue, metadata, etc.)
    //TODO: Tests a faire
    //Overriding DefaultValue with null
    //Tester tous les types de données (voir IsBuiltIn dans TypeHelper)
    //Créer des types à partir de POCOs existants
    //TODO: Faire des tests avec des RelationProperty
    //TODO: Thread safe avec les collections - readerwriterlockslim... !?
    //TODO: Tester le scenario ou le contenttype est modifié entre deux révision (devrait fail)
    //TODO: Faire en sorte de ne plus avoir a mettre des     Thread.Sleep(50);

    [TestFixture]
    public class Scenarios
    {
        protected Module Module;

        //TODO: Le soft delete va etre accomplie  l'aide d'un ContentType de base (implicite) genre object en c# 
        //qui va contenir une prop Deleted 
        //et il faudrait ajouter une propriété a chanque translation Delete pour le soft delete des Translation

        //[TestFixtureSetUp]
        //private void TestFixtureSetUp()
        //{

        //}

        [TestFixtureSetUp]
        protected virtual void TestFixtureSetUp()
        {
            new Starter().Start();
            var contentManager = DependencyResolver.Current.GetService<IContentManager>();
            var module = BuildCompleteModule();
            contentManager.Store(module);
            contentManager.SaveChanges();
            contentManager = DependencyResolver.Current.GetService<IContentManager>();
            Module = contentManager.LoadModule(module.Id);
        }

        [Test]
        public void RelationProperty()
        {
            //Rien de spécial ici - une RelationProperty n'est qu'un Guid? (id du contenu lié)

            //Arrange
            var content = new Content(Module[TestsConstants.ContentType.SubFolderTypeName], Cultures.FrenchCanadianCulture);

            //Act
            content[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.ParentFolderPropertyName].SetValue(TestsConstants.ContentType.ParentFolderPropertyDefaultValue);

            var loadedContent = SaveAndLoadContent(content);

            //Assert
            Assert.That(loadedContent[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.ParentFolderPropertyName].GetValue<Guid>(), Is.EqualTo(TestsConstants.ContentType.ParentFolderPropertyDefaultValue));

            var serializedContent = JsonConvert.SerializeObject(content, new Newtonsoft.Json.JsonSerializerSettings { DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc });

          var deserializedContent =  JsonConvert.DeserializeObject<Content>(serializedContent);
          Assert.That(deserializedContent[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.ParentFolderPropertyName].GetValue<Guid>(), Is.EqualTo(TestsConstants.ContentType.ParentFolderPropertyDefaultValue));
        }

        [Test]
        public void AggregationProperty()
        {
            //Arrange
            var aggregatedContent = new Content(Module[TestsConstants.ContentType.FolderTypeName], Cultures.FrenchCanadianCulture);
            var content = new Content(Module[TestsConstants.ContentType.SubFolderTypeName], Cultures.FrenchCanadianCulture);

            //Act
            content[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.AggregatedFolderPropertyName].SetValue(aggregatedContent) ;

            var loadedContent = SaveAndLoadContent(content);

            //Assert - Attention au nested content... il faudrait faire u object NestedContent sans versionning
            Assert.That(loadedContent[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.AggregatedFolderPropertyName].GetValue<Content>(), Is.EqualTo(aggregatedContent));
        }

        [Test]
        public void AggregationsProperty()
        {
            //Arrange
            var aggregatedContentOne = new Content(Module[TestsConstants.ContentType.FolderTypeName], Cultures.FrenchCanadianCulture);
            aggregatedContentOne[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].SetValue(TestsConstants.ContentType.NamePropertyDefaultValue);
            var aggregatedContentTwo = new Content(Module[TestsConstants.ContentType.FolderTypeName], Cultures.FrenchCanadianCulture);
            aggregatedContentTwo[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].SetValue(TestsConstants.ContentType.NamePropertyModifiedValue); 
            var content = new Content(Module[TestsConstants.ContentType.SubFolderTypeName], Cultures.FrenchCanadianCulture);

            var aggregationValue = new List<Content> { aggregatedContentOne, aggregatedContentTwo };

            //Act
            content[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.AggregatedFoldersPropertyName].SetValue(aggregationValue);

            var loadedContent = SaveAndLoadContent(content);

            //Assert
            CollectionAssert.AreEquivalent((IEnumerable)loadedContent[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.AggregatedFoldersPropertyName].GetValue<List<Content>>(), aggregationValue);
        }

        [Test]
        public void RelationsProperty()
        {
            //RelationsProperty est une liste de Guid (id des contenus liés)

            //Arrange
            var content = new Content(Module[TestsConstants.ContentType.SubFolderTypeName], Cultures.FrenchCanadianCulture);

            //Act
            content[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.RelatedFoldersPropertyName].SetValue(TestsConstants.ContentType.RelatedFoldersPropertyDefaultValue); 

            var loadedContent = SaveAndLoadContent(content);

            //Assert
            Assert.That(loadedContent[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.RelatedFoldersPropertyName].GetValue<List<Guid>>(), Is.EqualTo(TestsConstants.ContentType.RelatedFoldersPropertyDefaultValue));
        }

        [Test]
        public void ContentTypeInheritance()
        {
            ContentType loadedSubFolderContentType;

            Module.TryGetContentType(TestsConstants.ContentType.SubFolderTypeName, out loadedSubFolderContentType);

            Assert.That(loadedSubFolderContentType, Is.Not.Null);
            Assert.That(loadedSubFolderContentType.HasPropertyNamed(TestsConstants.ContentType.NamePropertyName));
            Assert.That(loadedSubFolderContentType.HasPropertyNamed(TestsConstants.ContentType.ParentFolderPropertyName));
        }

        [Test]
        public void OverridingDefaultValueShouldNotAlterBaseContentTypeDefaultValue()
        {
            var subFolderContentType = Module[TestsConstants.ContentType.SubFolderTypeName];

            subFolderContentType[TestsConstants.ContentType.NamePropertyName].SetDefaultValue("NewValue");

            Assert.That(Module[TestsConstants.ContentType.FolderTypeName][TestsConstants.ContentType.NamePropertyName].GetDefaultValue<string>(), Is.EqualTo(TestsConstants.ContentType.NamePropertyDefaultValue));
            Assert.That(subFolderContentType[TestsConstants.ContentType.NamePropertyName].GetDefaultValue<string>(), Is.EqualTo("NewValue"));
        }

        [Test]
        public void WhenCultureInvariantContentPropertyValueIsModifiedInASpecificCultureItShouldBeModifiedInOtherCulturesAsWell()
        {
            var content = new Content(Module[TestsConstants.ContentType.FolderTypeName], Cultures.FrenchCanadianCulture);

            content.Translate(Cultures.EnglishCanadianCulture);
            content[Cultures.EnglishCanadianCulture][TestsConstants.ContentType.ReleaseDatePropertyName].SetValue(DateTime.MinValue);

            var loadedContent = SaveAndLoadContent(content);


            Assert.That(loadedContent[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.ReleaseDatePropertyName].GetValue<DateTime>(), Is.EqualTo(DateTime.MinValue));
        }

        private Content SaveAndLoadContent(Content content)
        {
            var contentManager = DependencyResolver.Current.GetService<IContentManager>();
            contentManager.Store(content);
            contentManager.SaveChanges();
            contentManager = DependencyResolver.Current.GetService<IContentManager>();
            return contentManager.LoadContent(content.Id);
        }

        [Test]
        public void WhenCultureVariantContentPropertyValueIsModifiedInASpecificCultureItShouldNotBeModifiedInOtherCultures()
        {
            //Arrange
            var content = new Content(Module[TestsConstants.ContentType.FolderTypeName], Cultures.FrenchCanadianCulture);

            //Act
            content.Translate(Cultures.EnglishCanadianCulture);
            content[Cultures.EnglishCanadianCulture][TestsConstants.ContentType.NamePropertyName].SetValue(TestsConstants.ContentType.NamePropertyModifiedValue);

            var loadedContent = SaveAndLoadContent(content);

            //Assert
            Assert.That(loadedContent[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(TestsConstants.ContentType.NamePropertyDefaultValue));
        }

        [Test]
        public void WhenContentIsLoadedItShouldHaveTheSameIdAsWhenCreated()
        {
            var folder = new Content(Module[TestsConstants.ContentType.FolderTypeName], Cultures.FrenchCanadianCulture);

            var loadedFolder = SaveAndLoadContent(folder);

            Assert.That(loadedFolder.Id, Is.EqualTo(folder.Id));
        }

        [Test]
        public void CreateTranslationVersionsWithPublications()
        {
            var folder = new Content(Module[TestsConstants.ContentType.FolderTypeName], Cultures.FrenchCanadianCulture);
            const string newName = "Super Folder";
            folder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].SetValue(newName); 

            var loadedFolder = SaveAndLoadContent(folder);

            Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, published: true), Is.Null);

            var dateTimeBeforesFirstPublication = DateTime.Now;
            Thread.Sleep(50);
            var firstPublication = new Publication();
            loadedFolder.CreateTranslationVersions(firstPublication);

            const string newName2 = "Super Folder 2";
            loadedFolder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].SetValue(newName2);

            loadedFolder = SaveAndLoadContent(loadedFolder);

            Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, published: true)[TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName));
            Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, published: true, dateTime: dateTimeBeforesFirstPublication), Is.Null);
            Assert.That(loadedFolder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName2));
            Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, published: true, dateTime: DateTime.MaxValue)[TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName));

            //Une publication précédente (CreationDate) ne devrait pas pouvoir s'étendre sur une plus longue période que la suivante...
            //Sinon comment viendrions nous annuler la publication d'un contenu ayant eu une publication infinie...

            const string newName3 = "Super Folder 3";
            loadedFolder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].SetValue(newName3);
            Thread.Sleep(50);
            var secondPublication = new Publication(endingDate: DateTime.Now.AddHours(2));
            loadedFolder.CreateTranslationVersions(secondPublication);

            loadedFolder = SaveAndLoadContent(loadedFolder);

            Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, published: true)[TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName3));
            Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, published: true, dateTime: DateTime.Now.AddHours(1))[TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName3));
            Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, published: true, dateTime: DateTime.Now.AddHours(3)), Is.Null);
        }

        [Test]
        public void CreateTranslationVersions()
        {
            var folder = new Content(Module[TestsConstants.ContentType.FolderTypeName], Cultures.FrenchCanadianCulture);
            const string newName = "Super Folder";
            folder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].SetValue(newName); 

            var loadedFolder = SaveAndLoadContent(folder);

            Assert.That(loadedFolder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName));


            var dateTimeBeforesFirstVersion = DateTime.Now;
            Thread.Sleep(50);
            loadedFolder.CreateTranslationVersions();


            loadedFolder = SaveAndLoadContent(loadedFolder);

            Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, dateTimeBeforesFirstVersion)[TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName));
            Assert.That(loadedFolder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName));

            const string newName2 = "Super Folder 2";
            loadedFolder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].SetValue(newName2);

            loadedFolder = SaveAndLoadContent(loadedFolder);

            Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, dateTimeBeforesFirstVersion)[TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName));
            Assert.That(loadedFolder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName2));

            var dateTimeBeforeSecondVersion = DateTime.Now;
            Thread.Sleep(50);
            loadedFolder.CreateTranslationVersions();


            loadedFolder = SaveAndLoadContent(loadedFolder);

            Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, dateTimeBeforeSecondVersion)[TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName2));
            Assert.That(loadedFolder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName2));



            loadedFolder = SaveAndLoadContent(loadedFolder);

            Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, dateTimeBeforesFirstVersion)[TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName));
            Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, dateTimeBeforeSecondVersion)[TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName2));
        }

        [Test]
        public void CreateManyVersions()
        {
            var folder = new Content(Module[TestsConstants.ContentType.FolderTypeName], Cultures.FrenchCanadianCulture);
            var name = Guid.NewGuid().ToString();
            folder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].SetValue(name); 
            var loadedFolder = SaveAndLoadContent(folder);

            Assert.That(loadedFolder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(name));

            var timestamp = DateTime.Now;
            Thread.Sleep(50);
            loadedFolder.CreateTranslationVersions();
            loadedFolder = SaveAndLoadContent(loadedFolder);

            Assert.That(loadedFolder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(name));
            Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, timestamp)[TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(name));

            for (var i = 0; i < 10; i++)
            {
                var newName = Guid.NewGuid().ToString();
                loadedFolder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].SetValue(newName);

                loadedFolder = SaveAndLoadContent(loadedFolder);

                Assert.That(loadedFolder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(newName));
                Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, timestamp)[TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(name));

                timestamp = DateTime.Now;
                Thread.Sleep(50);
                loadedFolder.CreateTranslationVersions();
                loadedFolder = SaveAndLoadContent(loadedFolder);

                name = newName;

                Assert.That(loadedFolder[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(name));
                Assert.That(loadedFolder.GetVersion(Cultures.FrenchCanadianCulture, timestamp)[TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(name));
            }
        }

        [Test]
        public void WhenContentIsCreatedItShouldContainThePropertiesOfItsContentType()
        {
            var content = new Content(Module[TestsConstants.ContentType.FolderTypeName], Cultures.FrenchCanadianCulture);

            var loadedContent = SaveAndLoadContent(content);

            foreach (var property in Module[TestsConstants.ContentType.FolderTypeName].Properties)
            {
                Assert.IsTrue(loadedContent.HasPropertyNamed(property.Name));
            }

            foreach (var property in loadedContent[Cultures.FrenchCanadianCulture].Properties)
            {
                Assert.IsTrue(Module[TestsConstants.ContentType.FolderTypeName].HasPropertyNamed(property.ContentTypeProperty.Name));
            }
        }

        [Test]
        public void WhenContentIsCreatedItsPropertiesShouldBeIniatilizedWithTheContentTypePropertiesDefaultValues()
        {
            var content = new Content(Module[TestsConstants.ContentType.FolderTypeName], Cultures.FrenchCanadianCulture);

            var loadedContent = SaveAndLoadContent(content);

            foreach (var propety in Module[TestsConstants.ContentType.FolderTypeName].Properties)
            {
                Assert.That(loadedContent[Cultures.FrenchCanadianCulture][propety].SerializedValue, Is.EqualTo(propety.SerializedDefaultValue));
            }
        }

        [Test]
        public void WhenContentIsTranslatedThePropertiesOfTheNewTranslationShouldBeInitializedWithTheDefaultValueOfThePropertyOfItsContentTypeExceptForCultureInvariantProperties()
        {
            var content = new Content(Module[TestsConstants.ContentType.FolderTypeName], Cultures.FrenchCanadianCulture);

            content.Translate(Cultures.EnglishCanadianCulture);

            content[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.NamePropertyName].SetValue("Rambo");
            content[Cultures.FrenchCanadianCulture][TestsConstants.ContentType.ReleaseDatePropertyName].SetValue(DateTime.MinValue); 

            var loadedContent = SaveAndLoadContent(content);

            Assert.That(loadedContent[Cultures.EnglishCanadianCulture][TestsConstants.ContentType.NamePropertyName].GetValue<string>(), Is.EqualTo(TestsConstants.ContentType.NamePropertyDefaultValue));
            Assert.That(TestsConstants.ContentType.NamePropertyDefaultValue, Is.Not.EqualTo("Rambo"));

            Assert.That(loadedContent[Cultures.EnglishCanadianCulture][TestsConstants.ContentType.ReleaseDatePropertyName].GetValue<DateTime>(), Is.EqualTo(DateTime.MinValue));
            Assert.That(TestsConstants.ContentType.ReleaseDatePropertyDefaultValue, Is.Not.EqualTo(DateTime.MinValue));
        }

        public Module BuildCompleteModule()
        {
            var module = new Module(TestsConstants.Module.FullName, TestsConstants.Module.Version);
            var folderType = module.AddContentType(TestsConstants.ContentType.FolderTypeName);
            folderType.AddProperty(TestsConstants.ContentType.NamePropertyName,
                TestsConstants.ContentType.NamePropertyDefaultValue, false);
            folderType.AddProperty(TestsConstants.ContentType.ReleaseDatePropertyName,
                TestsConstants.ContentType.ReleaseDatePropertyDefaultValue, true);

            var subFolderType = module.AddContentType(TestsConstants.ContentType.SubFolderTypeName, folderType);
            subFolderType.AddRelationProperty(TestsConstants.ContentType.ParentFolderPropertyName, true);
            subFolderType.AddRelationsProperty(TestsConstants.ContentType.RelatedFoldersPropertyName, true);
            subFolderType.AddAggregationProperty(TestsConstants.ContentType.AggregatedFolderPropertyName, true);
            subFolderType.AddAggregationsProperty(TestsConstants.ContentType.AggregatedFoldersPropertyName, true);

            return module;
        }
    }
}
