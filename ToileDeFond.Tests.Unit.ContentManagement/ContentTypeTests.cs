using System.Linq;
using NUnit.Framework;
using ToileDeFond.ContentManagement;
using ToileDeFond.Tests.Common;

namespace ToileDeFond.Tests.Unit.ContentManagement
{
    [TestFixture]
    public class ContentTypeTests
    {
        //TODO: Effacer un type parent d'autres types (effacer les enfant ou seulement retirer la relation parent et la replugger si parent plus haut)
        //TODO: Déplacer un type d'arborescence
        //TODO: Tester l'héritage du metadata des ContentType/ContentTypeProperty

        [Test]
        public void WhenContentTypeIsCreatedItsNameShouldBeTheOneSpecifiedInTheConstructor()
        {
            var folderType = new Module(TestsConstants.Module.FullName, TestsConstants.Module.Version)
                .AddContentType(TestsConstants.ContentType.FolderTypeName);

            Assert.That(TestsConstants.ContentType.FolderTypeName, Is.EqualTo(folderType.Name));
        }

        [Test]
        public void WhenContentTypeIsCreatedItsPropertyListShouldBeEmpty()
        {
            var folderType = new Module(TestsConstants.Module.FullName, TestsConstants.Module.Version)
                .AddContentType(TestsConstants.ContentType.FolderTypeName);

            Assert.That(folderType.Properties.ToList(), Is.Empty);
        }

        [Test]
        public void WhenCultureInvariantPropertyIsAddedToAContentTypeItShouldBeAccesibleByItsName()
        {
            var folderType = new Module(TestsConstants.Module.FullName, TestsConstants.Module.Version)
                .AddContentType(TestsConstants.ContentType.FolderTypeName);

            folderType.AddProperty(TestsConstants.ContentType.NamePropertyName, 
                TestsConstants.ContentType.NamePropertyDefaultValue, true);

            Assert.That(folderType[TestsConstants.ContentType.NamePropertyName], Is.Not.Null);
        }

        [Test]
        public void WhenCultureVariantPropertyIsAddedToAContentTypeItShouldBeAccesibleByItsName()
        {
            var folderType = new Module(TestsConstants.Module.FullName, TestsConstants.Module.Version)
                .AddContentType(TestsConstants.ContentType.FolderTypeName);

            folderType.AddProperty(TestsConstants.ContentType.NamePropertyName, 
                TestsConstants.ContentType.NamePropertyDefaultValue, false);

            Assert.That(folderType[TestsConstants.ContentType.NamePropertyName], Is.Not.Null);
        }

        [Test]
        public void WhenCultureInvariantPropertyIsAddedToAContentTypeItShouldContainTheAddedProperty()
        {
            var folderType = new Module(TestsConstants.Module.FullName, TestsConstants.Module.Version)
                .AddContentType(TestsConstants.ContentType.FolderTypeName);

            folderType.AddProperty(TestsConstants.ContentType.NamePropertyName, 
                TestsConstants.ContentType.NamePropertyDefaultValue, true);

            Assert.IsTrue(folderType.HasPropertyNamed(TestsConstants.ContentType.NamePropertyName));
        }

        [Test]
        public void WhenCultureVariantPropertyIsAddedToAContentTypeItShouldContainTheAddedProperty()
        {
            var folderType = new Module(TestsConstants.Module.FullName, TestsConstants.Module.Version)
                .AddContentType(TestsConstants.ContentType.FolderTypeName);

            folderType.AddProperty(TestsConstants.ContentType.NamePropertyName, 
                TestsConstants.ContentType.NamePropertyDefaultValue, false);

            Assert.IsTrue(folderType.HasPropertyNamed(TestsConstants.ContentType.NamePropertyName));
        }

        [Test]
        public void WhenPropertyIsAddedToAContentTypeItsDefaultValueShouldBeTheSameAsThePropertyDefaultValue()
        {
            var folderType = new Module(TestsConstants.Module.FullName, TestsConstants.Module.Version)
                .AddContentType(TestsConstants.ContentType.FolderTypeName);

            folderType.AddProperty(TestsConstants.ContentType.NamePropertyName, 
                TestsConstants.ContentType.NamePropertyDefaultValue, false);

            Assert.That(folderType[TestsConstants.ContentType.NamePropertyName].GetDefaultValue(TestsConstants.ContentType.NamePropertyDefaultValue.GetType()), 
                Is.EqualTo(TestsConstants.ContentType.NamePropertyDefaultValue));
        }
    }
}