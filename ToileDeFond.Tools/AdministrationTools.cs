using System.Collections.Generic;
using NUnit.Framework;
using ToileDeFond.ContentManagement;
using ToileDeFond.Modularity;
using ToileDeFond.Website.Administration;

namespace ToileDeFond.Tools
{
   public class AdministrationTools
    {
       [TestFixtureSetUp]
       public void TestFixtureSetUp()
       {
           new Starter().Start();
       }

       [Test]
       [Ignore]
       public void AddDefaultPropertyEditors()
       {
           DeleteAllPropertyEditors();

           var propertyEditors = new List<PropertyEditor>{
                new PropertyEditor{
                    Name = "Single Text Line",
                    GetRoute = "/ToileDeFond.Website.Administration/DefaultPropertyEditors/GetSingleLineText",
                    PostRoute = "/ToileDeFond.Website.Administration/DefaultPropertyEditors/PostSingleLineText"
                },
                new PropertyEditor{
                    Name = "Integer",
                    GetRoute = "/ToileDeFond.Website.Administration/DefaultPropertyEditors/GetInteger",
                    PostRoute = "/ToileDeFond.Website.Administration/DefaultPropertyEditors/PostInteger"
                },
                new PropertyEditor{
                    Name = "Embedded Content",
                    GetRoute = "/ToileDeFond.Website.Administration/DefaultPropertyEditors/GetEmbeddedContent",
                    PostRoute = "/ToileDeFond.Website.Administration/DefaultPropertyEditors/PostEmbeddedContent"
                }
            };


           var publication = new Publication();
           using (var propertyEditorRepository = DependencyResolver.Current.GetService<IPropertyEditorRepository>())
           {
               foreach (var propertyEditor in propertyEditors)
               {
                   propertyEditorRepository.AddPropertyEditor(propertyEditor, publication);
               }

               propertyEditorRepository.SaveChanges();
           }
       }

       [Test]
       [Ignore]
       public void DeleteAllPropertyEditors()
       {
           using (var propertyEditorRepository = DependencyResolver.Current.GetService<IPropertyEditorRepository>())
           {
               propertyEditorRepository.DeleteAllPropertyEditors();
           }
       }
    }
}
