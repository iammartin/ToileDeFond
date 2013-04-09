using System;
using System.Globalization;
using System.Text;
using System.Web.Mvc;
using Raven.Imports.Newtonsoft.Json;
using ToileDeFond.ContentManagement;
using ToileDeFond.ContentManagement.Reflection;
using ToileDeFond.Localization;
using ToileDeFond.Website.Administration.Models;
using ToileDeFond.Utilities;
using System.Linq;

namespace ToileDeFond.Website.Administration.Controllers
{
    [Authorize]
    public class ContentController : Controller
    {
        private readonly IReflectionContentManager _reflectionContentManager;
        private readonly ICultureManager _cultureManager;

        public static readonly Type[] BuiltInTypes = new[] { typeof(bool), typeof(byte), typeof(char), typeof(DateTime), typeof(decimal), typeof(double), typeof(Int16), typeof(Int32), typeof(Int64),
                typeof(sbyte), typeof(Single), typeof(string), typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(CultureInfo), typeof(Guid), typeof(Content)};

        public ContentController(IReflectionContentManager moduleManager, ICultureManager cultureManager)
        {
            _reflectionContentManager = moduleManager;
            _cultureManager = cultureManager;
        }

        //TODO: Paging
        [HttpGet]
        public ActionResult Contents(string contentTypeFullName, CultureInfo culture = null)
        {
            var viewModel = new ContentsViewModel();

            if (!contentTypeFullName.IsNullOrEmpty())
            {
                viewModel.Contents = _reflectionContentManager.LoadContentsByContentType(contentTypeFullName, 10);
                viewModel.ContentType = _reflectionContentManager.LoadContentType(contentTypeFullName);
                viewModel.Culture = culture ?? _cultureManager.GetDefaultCulture(); //TODO : Faire la différence entre ContentManagementCulture & UICulture!!!
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Content(string contentTypeFullName, Guid? contentId = null, CultureInfo culture = null)
        {
            culture = culture ?? _cultureManager.GetDefaultCulture();

            Content content = null;

            if (contentId.HasValue)
            {
                content = _reflectionContentManager.LoadContent(contentId.Value);
            }

            if (content == null)
            {
                if (contentTypeFullName.IsNullOrEmpty())
                    throw new NotImplementedException();

                var contentType = _reflectionContentManager.LoadContentType(contentTypeFullName);

                if (contentType != null)
                {
                    content = new Content(contentType, culture);
                }
            }
            else
            {
                content = content.Translate(culture).Content;
            }

            return View(content);
        }

        [HttpPost]
        public ActionResult Content(Content.ContentTranslation contentTranslation)
        {
            _reflectionContentManager.Store(contentTranslation.Content);
            _reflectionContentManager.SaveChanges();

            //var viewModel = new ContentViewModel
            //                    {
            //                        Content = contentTranslation
            //                    };

            return Redirect(new StringBuilder("/admin/contents")
                .AddQueryParam("contenttypefullname", contentTranslation.ContentType.FullName)
                .AddQueryParamIfAlreadyThere("culture").ToString());
        }

        [HttpPost]
        public ActionResult Delete(Guid id, string contentTypeFullName, CultureInfo culture = null)
        {
            _reflectionContentManager.DeleteContentById(id);
            _reflectionContentManager.SaveChanges();

            return Redirect(new StringBuilder("/admin/contents")
                .AddQueryParam("contenttypefullname", contentTypeFullName)
                .AddQueryParamIfAlreadyThere("culture").ToString());
        }
    }
}