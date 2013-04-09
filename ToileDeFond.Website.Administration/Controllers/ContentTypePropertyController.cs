using System;
using System.Globalization;
using System.Text;
using System.Web.Mvc;
using ToileDeFond.ContentManagement;
using ToileDeFond.ContentManagement.Reflection;
using ToileDeFond.Website.Administration.Models;
using ToileDeFond.Utilities;
using System.Linq;

namespace ToileDeFond.Website.Administration.Controllers
{
    [Authorize]
    public class ContentTypePropertyController : Controller
    {
        private readonly IReflectionContentManager _reflectionContentManager;
        private readonly IPropertyEditorRepository _propertyEditorRepository;

        public static readonly Type[] BuiltInTypes = new[] { typeof(bool), typeof(byte), typeof(char), typeof(DateTime), typeof(decimal), typeof(double), typeof(Int16), typeof(Int32), typeof(Int64),
                typeof(sbyte), typeof(Single), typeof(string), typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(CultureInfo), typeof(Guid), typeof(Content)};

        public ContentTypePropertyController(IReflectionContentManager moduleManager, IPropertyEditorRepository propertyEditorRepository)
        {
            _reflectionContentManager = moduleManager;
            _propertyEditorRepository = propertyEditorRepository;
        }

        [HttpGet]
        public ActionResult Edit(string contentTypeFullName, string propertyFullName = null)
        {
            //TODO: Validation, etc.
            //TODO: BaseContentType
            contentTypeFullName = _reflectionContentManager.GetContentTypeFullNameFromPropertyFullName(propertyFullName) ?? contentTypeFullName;

            ContentType contentType = null;

            if (!contentTypeFullName.IsNullOrEmpty())
            {
                contentType = _reflectionContentManager.LoadContentType(contentTypeFullName);
            }

            if (contentType == null)
            {
                throw new NotImplementedException();
            }

            IContentTypeProperty contentTypeProperty = null;
            if (!propertyFullName.IsNullOrEmpty())
            {
                contentType.TryGetProperty(propertyFullName, out contentTypeProperty);
            }

            var viewModel = new PropertyEditorViewModelBase();

            if (contentTypeProperty != null)
            {
                viewModel.Name = contentTypeProperty.Name;
            }

            //viewModel.GetPropertyEditorValues = () => contentType.Module.ContentTypes.Select(ct => new MultiSelectListItem
            //        {
            //            GroupKey = ct.Module.Id.ToString(),
            //            GroupName = ct.Module.Name,
            //            Text = ct.Name,
            //            Value = ct.Id.ToString()
            //        });

            viewModel.GetPropertyEditorValues = new[] { new SelectListItem { Text = "", Value = "" } }
                .Union(_propertyEditorRepository.GetAll().Select(p => 
                    new SelectListItem { Text = p.Name, Value = p.GetRoute + ";" + p.PostRoute })).ToArray();

            viewModel.ContentTypeFullName = contentType.FullName;

            if (Request.IsAjaxRequest())
            {
                return PartialView("Edit", viewModel);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            throw new NotImplementedException();

            //TODO: Validation, etc.
            //TODO: BaseContentType

            //TODO:C'est pu type de donné mais un controle qu'on veut selectionner... le type ne sert plus a rien

            //var contentType = _reflectionContentManager.LoadContentType(viewModel.ContentTypeFullName);
            //Type type;

            //if (viewModel.Type == "ToileDeFond.ContentManagement.Content")
            //{
            //    type = typeof(Content);
            //    viewModel.CultureInvariant = true;
            //}
            //else
            //{
            //    type = Type.GetType(viewModel.Type);
            //}

            //var finalType = type;

            //if (viewModel.IsList)
            //{
            //    finalType = typeof(List<>).MakeGenericType(type);
            //}

            //IContentTypeProperty contentTypeProperty;
            //if (!contentType.TryGetProperty(viewModel.Name, out contentTypeProperty))
            //{
            //    contentTypeProperty = contentType.AddProperty(viewModel.Name, viewModel.IsList ? Activator.CreateInstance(finalType) :
            //          (viewModel.IsNullable ? null : finalType.GetDefaultValue()), viewModel.CultureInvariant);
            //}

            //if (viewModel.IsList && type == typeof(Guid))
            //{
            //    contentTypeProperty.SetOrOverrideMetadata("RelatedContentTypeWhiteList", viewModel.RelatedContentTypeWhiteList);
            //}
            //else
            //{
            //    contentTypeProperty.StopOverridingMetadata("RelatedContentTypeWhiteList");
            //}

            ////TODO: Update

            //_reflectionContentManager.Store(contentType.Module);
            //_reflectionContentManager.SaveChanges();

            //return Redirect(new StringBuilder("/admin/contenttype")
            //             .AddQueryParam("contenttypefullname", contentType.FullName).ToString());
        }

        [HttpPost]
        public ActionResult Delete(string propertyFullName)
        {
            var contentTypeFullName = _reflectionContentManager.GetContentTypeFullNameFromPropertyFullName(propertyFullName);
            var contentType = _reflectionContentManager.LoadContentType(contentTypeFullName);

            if (contentType != null)
            {
                if (contentType.RemoveProperty(propertyFullName))
                {
                    _reflectionContentManager.Store(contentType.Module);
                    _reflectionContentManager.SaveChanges();
                }

                return Redirect(new StringBuilder("/admin/contenttype")
                .AddQueryParam("contenttypefullname", contentTypeFullName).ToString());
            }

            return Redirect("/admin/modules");
        }
    }
}