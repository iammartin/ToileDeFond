using System;
using System.Globalization;
using System.Web.Mvc;
using ToileDeFond.ContentManagement;
using ToileDeFond.ContentManagement.Reflection;
using ToileDeFond.Website.Administration.Models;
using ToileDeFond.Utilities;

namespace ToileDeFond.Website.Administration.Controllers
{
    [Authorize]
    public class ContentTypeController : Controller
    {
        private readonly IReflectionContentManager _reflectionContentManager;

        public static readonly Type[] BuiltInTypes = new[] { typeof(bool), typeof(byte), typeof(char), typeof(DateTime), typeof(decimal), typeof(double), typeof(Int16), typeof(Int32), typeof(Int64),
                typeof(sbyte), typeof(Single), typeof(string), typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(CultureInfo), typeof(Guid), typeof(Content)};

        public ContentTypeController(IReflectionContentManager moduleManager)
        {
            _reflectionContentManager = moduleManager;
        }

        [HttpGet]
        public ActionResult ContentType(string contentTypeFullName)
        {
            return View(new ContentTypeViewModel { ContentType = _reflectionContentManager.LoadContentType(contentTypeFullName) });

            //TODO: Rediriger au module s'il n'est pas null
            //return Redirect("/admin");
        }

        [HttpGet]
        public ActionResult Edit(string moduleName, string contentTypeFullName = null, Guid? baseContentTypeId = null)
        {
            //TODO: Validation, etc.
            //TODO: BaseContentType

            ContentType contentType = null;

            if (!contentTypeFullName.IsNullOrEmpty())
            {
                contentType = _reflectionContentManager.LoadContentType(contentTypeFullName);
            }

            if (contentType == null && moduleName.IsNullOrEmpty())
            {
                throw new NotImplementedException();
            }


            /* _reflectionContentManager.GetModuleNameFromContentTypeFullName(contentTypeFullName);*/

            var viewModel = new EditContentTypeViewModel();

            if (contentType != null)
            {
                viewModel.Name = contentType.Name;
                viewModel.ModuleName = contentType.Module.Name;
            }
            else
            {
                viewModel.ModuleName = moduleName;
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EditContentTypeViewModel viewModel)
        {
            //TODO: Validation, etc.
            //TODO: BaseContentType

            var module = _reflectionContentManager.LoadModule(viewModel.ModuleName);

            module.AddContentType(viewModel.Name);

            _reflectionContentManager.Store(module);
            _reflectionContentManager.SaveChanges();

            return Redirect("/admin/module?modulename=" + viewModel.ModuleName);
        }
    }
}