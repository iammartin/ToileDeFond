using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using NUnit.Framework;
using ToileDeFond.ContentManagement;
using ToileDeFond.ContentManagement.Reflection;
using ToileDeFond.ContentManagement.Reflection.DefaultImplementation;

namespace ToileDeFond.Tests.Unit.ContentManagement.Reflection
{
    public class ReflectionBuilderTests
    {
        [Test]
        public void ReflectionBuilderTest()
        {
            var reflectionBuilder = new ReflectionContentManagementBuilder();

            var module = reflectionBuilder.BuildModuleFromAssembly(typeof (ContactInputViewModel).Assembly);

            Assert.That(module.Name, Is.EqualTo("ToileDeFond.Tests.Unit.ContentManagement.Reflection"));
            Assert.That(module.ContentTypes.Count(), Is.EqualTo(1));

            var contentType = module.ContentTypes.First();

            Assert.That(contentType.Name, Is.EqualTo("ContactInputViewModel"));
            Assert.That(contentType.Properties.Count(), Is.EqualTo(5));

            IContentTypeProperty nameProperty;

            contentType.TryGetProperty("Name", out nameProperty);
            Assert.That(nameProperty, Is.Not.Null);

            var dataAnnotationsModelMetadataProvider = new DataAnnotationsModelMetadataProvider();

            var contentModelmetadataBasedOnDataAnnotationsModelMetadataProvider = dataAnnotationsModelMetadataProvider
                .GetMetadataForType(() => new ContactInputViewModel(), typeof(ContactInputViewModel));

            var nameMedata = contentModelmetadataBasedOnDataAnnotationsModelMetadataProvider.Properties.Single( p => p.DisplayName == "Nom");

            //TODO: Tester l'overriding du metadata
            //TODO: Tester l'ensemble des propriétés metadata

            Assert.That(nameMedata, Is.Not.Null);
            //Assert.That(nameMedata.DisplayName, Is.EqualTo(nameProperty.DisplayName));
        }

        [ContentType]
        class ContactInputViewModel
        {
            [Required(ErrorMessage = "Le nom est requis")]
            [DisplayName("Nom")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Le courriel est requis")]
            [DisplayName("Courriel")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Le sujet est requis")]
            [DisplayName("Sujet")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Le message est requis")]
            [DisplayName("Message")]
            public string Message { get; set; }

            [DisplayName("If you're human leave this blank.")]
            public string SpamProtection { get; set; }
        }
    }
}
