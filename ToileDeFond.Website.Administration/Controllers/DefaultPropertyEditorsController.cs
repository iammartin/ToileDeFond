using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Imports.Newtonsoft.Json;
using ToileDeFond.ContentManagement;
using ToileDeFond.ContentManagement.Reflection;
using ToileDeFond.Website.Administration.Models;

namespace ToileDeFond.Website.Administration.Controllers
{
    //TODO: Hériter d'un controller de base pour la logique partagée (ou peut-etre mieux utiliser un service) - exemple : GetPropertyEditorValues
    public class DefaultPropertyEditorsController : Controller
    {
        private readonly IReflectionContentManager _reflectionContentManager;
        private readonly IPropertyEditorRepository _propertyEditorRepository;

        public DefaultPropertyEditorsController(IReflectionContentManager reflectionContentManager, IPropertyEditorRepository propertyEditorRepository)
        {
            _reflectionContentManager = reflectionContentManager;
            _propertyEditorRepository = propertyEditorRepository;
        }

        [HttpPost]
        public ActionResult GetEmbeddedContent(PropertyEditorViewModelBase viewModel)
        {
            return PartialView("_EmbeddedContent", new EmbeddedContentPropertyEditorViewModel
            {
                ContentTypeFullName = viewModel.ContentTypeFullName,
                Name = viewModel.Name,
                PropertyEditor = viewModel.PropertyEditor,
                GetPropertyEditorValues = GetPropertyEditorValues()
            });
        }

        [HttpPost]
        public ActionResult PostEmbeddedContent(EmbeddedContentPropertyEditorViewModel viewModel)
        {
            //TODO: Validation, etc.

            var contentTypeProperty = CreateContentTypeProperty(viewModel, null);

            contentTypeProperty.SetOrOverrideMetadata("DisplayName", viewModel.DisplayName);
            contentTypeProperty.SetOrOverrideMetadata("Type", typeof(Content));
            contentTypeProperty.SetOrOverrideMetadata("IsRequired", viewModel.IsRequired);

            return SaveContentTypeProperty(contentTypeProperty);
        }

        [HttpPost]
        public ActionResult GetSingleLineText(PropertyEditorViewModelBase viewModel)
        {
            return PartialView("_SingleLineText", new SingleTextLinePropertyEditorViewModel
                {
                    ContentTypeFullName = viewModel.ContentTypeFullName,
                    Name = viewModel.Name,
                    PropertyEditor = viewModel.PropertyEditor,
                    GetPropertyEditorValues = GetPropertyEditorValues()
                });
        }

        [HttpPost]
        public ActionResult PostSingleLineText(SingleTextLinePropertyEditorViewModel viewModel)
        {
            //TODO: Validation, etc.

            var contentTypeProperty = CreateContentTypeProperty(viewModel, "");

            contentTypeProperty.SetOrOverrideMetadata("DisplayName", viewModel.DisplayName);
            contentTypeProperty.SetOrOverrideMetadata("Type", typeof(string));
            contentTypeProperty.SetOrOverrideMetadata("IsRequired", viewModel.IsRequired);

           return SaveContentTypeProperty(contentTypeProperty);
        }

        [HttpPost]
        public ActionResult GetInteger(PropertyEditorViewModelBase viewModel)
        {
            return PartialView("_Integer", new IntegerPropertyEditorViewModel()
            {
                ContentTypeFullName = viewModel.ContentTypeFullName,
                Name = viewModel.Name,
                PropertyEditor = viewModel.PropertyEditor,
                GetPropertyEditorValues = GetPropertyEditorValues()
            });
        }

        [HttpPost]
        public ActionResult PostInteger(IntegerPropertyEditorViewModel viewModel)
        {
            //TODO: Validation, etc.

            var contentTypeProperty = CreateContentTypeProperty(viewModel, 0);

            contentTypeProperty.SetOrOverrideMetadata("DisplayName", viewModel.DisplayName);
            contentTypeProperty.SetOrOverrideMetadata("Type", typeof(int));
            contentTypeProperty.SetOrOverrideMetadata("IsRequired", viewModel.IsRequired);

           return SaveContentTypeProperty(contentTypeProperty);
        }

        private RedirectResult SaveContentTypeProperty(ContentType.ContentTypeProperty contentTypeProperty)
        {
            _reflectionContentManager.Store(contentTypeProperty.ContentType.Module);
            _reflectionContentManager.SaveChanges();

            return Redirect("/admin/contenttype?contenttypefullname=" + contentTypeProperty.ContentType.FullName);
        }

        private ContentType.ContentTypeProperty CreateContentTypeProperty(PropertyEditorViewModelBase viewModel, object defaultValue)
        {
            //TODO: Validation, etc.
            var contentType = _reflectionContentManager.LoadContentType(viewModel.ContentTypeFullName);

           return contentType.AddProperty(viewModel.Name, defaultValue, viewModel.IsCultureInvariant);
        }

        private SelectListItem[] GetPropertyEditorValues()
        {
            return new[] { new SelectListItem { Text = "", Value = "" } }
                    .Union(_propertyEditorRepository.GetAll().Select(p =>
                        new SelectListItem { Text = p.Name, Value = p.GetRoute + ";" + p.PostRoute })).ToArray();
        } 
    }
}
