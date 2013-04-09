using System;
using ToileDeFond.ContentManagement;
using ToileDeFond.ContentManagement.DataTransfer;
using NUnit.Framework;
using System.Linq;
using ToileDeFond.Tests.Common;

namespace ToileDeFond.Tests.Unit.ContentManagement
{
    [TestFixture]
    public class ModuleDataTransferTests
    {
        //TODO: Ajouter les tests pour les Inherited properties

        [Test]
        public void ModuleToDto()
        {
            var module = BuildCompleteModule();

            //var moduleDto = new ModuleMapper().ToDto(module);

            var moduleDto = module.ToDto();

            Assert.That(moduleDto.Id, Is.EqualTo(module.Id));
            Assert.That(moduleDto.Name, Is.EqualTo(module.Name));
            Assert.That(moduleDto.Version, Is.EqualTo(module.Version));
            Assert.That(moduleDto.ContentTypes.Count, Is.EqualTo(module.ContentTypes.Count()));

            var contentTypeAndContentTypeDto = AssertContentType(module, moduleDto, TestsConstants.ContentType.FolderTypeName);

            var contentTypePropertyAndContentTypePropertyDto = AssertContentTypeProperty(contentTypeAndContentTypeDto, TestsConstants.ContentType.NamePropertyName);

            var contentTypeAndContentTypeDto2 = AssertContentType(module, moduleDto, TestsConstants.ContentType.SubFolderTypeName);

            AssertContentTypeProperty(contentTypeAndContentTypeDto2, TestsConstants.ContentType.ParentFolderPropertyName);
            AssertInheritedContentTypeProperty(contentTypeAndContentTypeDto2, TestsConstants.ContentType.NamePropertyName, contentTypePropertyAndContentTypePropertyDto);
        }

        private Tuple<ContentType, ContentTypeDto> AssertContentType(Module module, ModuleDto moduleDto, string contentTypeName)
        {
            var folderTypeDto = moduleDto.ContentTypes.SingleOrDefault(c => c.Name.Equals(contentTypeName));

            Assert.That(folderTypeDto, Is.Not.Null);

            ContentType folderType;

            module.TryGetContentType(contentTypeName, out folderType);

            Assert.That(folderTypeDto.Id, Is.EqualTo(folderType.Id));
            Assert.That(folderTypeDto.Name, Is.EqualTo(folderType.Name));
            Assert.That(folderTypeDto.Metadata, Is.EqualTo(folderType.Metadata));

            if(folderType.BaseContentType != null)
            {
                Assert.That(folderTypeDto.BaseContentType, Is.EqualTo(folderType.BaseContentType.Id));
            }
            
            Assert.That(folderTypeDto.OwnProperties.Count, Is.EqualTo(folderType.OwnProperties.Count()));

            return new Tuple<ContentType, ContentTypeDto>(folderType, folderTypeDto);
        }

        private Tuple<IContentTypeProperty, ContentTypePropertyDto> AssertContentTypeProperty(Tuple<ContentType, ContentTypeDto> contentTypeAndContentTypeDto, string propertyName)
        {
            var namePropertyDto = contentTypeAndContentTypeDto.Item2.OwnProperties.SingleOrDefault(p => p.Name.Equals(propertyName));

            Assert.That(namePropertyDto, Is.Not.Null);

            IContentTypeProperty nameProperty;

            contentTypeAndContentTypeDto.Item1.TryGetProperty(propertyName, out nameProperty);
            Assert.That(namePropertyDto.Id, Is.EqualTo(nameProperty.Id));
            Assert.That(namePropertyDto.Name, Is.EqualTo(nameProperty.Name));
            Assert.That(namePropertyDto.Metadata, Is.EqualTo(nameProperty.Metadata));
            Assert.That(namePropertyDto.Type, Is.EqualTo(nameProperty.Type.FullName));

            return new Tuple<IContentTypeProperty, ContentTypePropertyDto>(nameProperty, namePropertyDto);
        }

        private Tuple<IContentTypeProperty, InheritedContentTypePropertyDto> AssertInheritedContentTypeProperty(Tuple<ContentType, ContentTypeDto> contentTypeAndContentTypeDto, string propertyName, 
            Tuple<IContentTypeProperty, ContentTypePropertyDto> contentTypePropertyAndContentTypePropertyDto)
        {
            var namePropertyDto = contentTypeAndContentTypeDto.Item2.InheritedProperties.SingleOrDefault(p => p.Id.Equals(contentTypePropertyAndContentTypePropertyDto.Item1.Id));

            Assert.That(namePropertyDto, Is.Not.Null);

            IContentTypeProperty nameProperty;

            contentTypeAndContentTypeDto.Item1.TryGetProperty(propertyName, out nameProperty);

            Assert.That(namePropertyDto.Id, Is.EqualTo(nameProperty.Id));
            Assert.That(namePropertyDto.Metadata, Is.EqualTo(nameProperty.Metadata));

            return new Tuple<IContentTypeProperty, InheritedContentTypePropertyDto>(nameProperty, namePropertyDto);
        }

        //TODO: [Test]
        //public void ModuleFromDto()
        //{

        //}

        public  Module BuildCompleteModule()
        {
            var module = new Module(TestsConstants.Module.FullName, TestsConstants.Module.Version);
            var folderType = module.AddContentType(TestsConstants.ContentType.FolderTypeName);
            folderType.AddProperty(TestsConstants.ContentType.NamePropertyName, typeof(string),
                TestsConstants.ContentType.NamePropertyDefaultValue, false);
            folderType.AddProperty(TestsConstants.ContentType.ReleaseDatePropertyName, typeof(DateTime?),
                TestsConstants.ContentType.ReleaseDatePropertyDefaultValue, true);

            var subFolderType = module.AddContentType(TestsConstants.ContentType.SubFolderTypeName, folderType);
            subFolderType.AddRelationProperty(TestsConstants.ContentType.ParentFolderPropertyName, true);

            return module;
        }
    }
}
