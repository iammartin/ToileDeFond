using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using ToileDeFond.Utilities;
using MefContrib.Web.Mvc;
using Newtonsoft.Json;

namespace ToileDeFond.ContentManagement.Metadata
{
    //Note importante : ca peut pas etre un ContentModelBinder puisqu'on ne veut pas reconstruire un Content mais plutot un genre de DTO représentant une nouvelle version d'un contenu
    //Ici on ne devrait pas faire de query a la bd pour récupérer un Content???
    //Tout ca rammene la question du versionning des sous-contenus!!! comment ca va fonctionner?


    [ModelBinderExport(typeof(Content.ContentTranslation))]
    public class ContentModelBinder : IModelBinder
    {
        private readonly IContentManager _contentManager;
        readonly string[] _corePropertiesName = new[] { "ContentTypeFullName", "Culture", "Id" };

        [ImportingConstructor]
        public ContentModelBinder(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var data = GetData(controllerContext);
            var corePropertyValues = RemoveAndGetValuesForCoreProperties(data);

            CultureInfo culture;
            if (!TryGetCulture(corePropertyValues, out culture))
            {
                throw new Exception("ContentModelBinder did not found a culture.");
            }

            Guid? contentId = null;
            if (!TryGetContentId(corePropertyValues, ref contentId))
            {
                throw new Exception("ContentModelBinder did not found a content ID.");
            }

            var content = _contentManager.LoadContent(contentId.Value);

            if (content == null)
            {
                ContentType contentType;
                if (!TryGetContentType(corePropertyValues, out contentType))
                {
                    throw new Exception("ContentModelBinder did not found a content type.");
                }

                content = new Content(contentType, culture);
            }

            //TODO: Remplacer toutes les constantes par des constantes... ex. "Id"

            content = UpdateContent(content, data, culture);

            return content[culture];
        }

        private Dictionary<string, string> RemoveAndGetValuesForCoreProperties(IDictionary<string, string> data)
        {
            var corePropertyValues = new Dictionary<string, string>();

            foreach (var corePropertyName in _corePropertiesName)
            {
                KeyValuePair<string, string> item;
                var name = corePropertyName;

                if (data.TryGetFirst(d => d.Key.SubstringAfterLastIndexOf('.') == name, out item))
                {
                    corePropertyValues.Add(corePropertyName, item.Value);
                    data.Remove(item);
                }
            }

            return corePropertyValues;
        }

        private bool TryGetContentType(IDictionary<string, string> data, out ContentType contentType)
        {
            contentType = null;
            string contentTypeFullName;

            if (data.TryGetValue("ContentTypeFullName", out contentTypeFullName) && !contentTypeFullName.IsNullOrEmpty())
            {
                contentType = _contentManager.LoadContentType(contentTypeFullName);
            }

            return contentType != null;
        }

        private bool TryGetContentId(IDictionary<string, string> data, ref Guid? contentId)
        {
            string stringContentId;

            if (data.TryGetValue("Id", out  stringContentId))
            {
                stringContentId.IsGuid(ref contentId);
            }

            return contentId != null;
        }

        private Content UpdateContent(Content content, IDictionary<string, string> data, CultureInfo culture)
        {
            //TODO: Rendu ici - décortiquer le contenu et ses sous objets le cas échéant...

            var translation = content.GetExistingOrNewTranslation(content, culture);

            foreach (var property in data)
            {
                IContentTypeProperty contentTypeProperty;
                if(translation.ContentType.TryGetProperty(property.Key.SubstringAfterLastIndexOf('.'), out contentTypeProperty))
                {
                   var type = contentTypeProperty.GetMetadataOrDefault<Type>("Type") ?? typeof(string);

                    //TODO: Ouach!
                    if (type == typeof (string) || type == typeof (DateTime) || type == typeof (DateTime?))
                    {
                        translation[contentTypeProperty].SerializedValue = "\"" +  property.Value + "\"";
                    }
                    else
                    {
                        translation[contentTypeProperty].SerializedValue = property.Value;
                    }
                }
            }

            return content;
        }

        private bool TryGetCulture(IDictionary<string, string> data, out CultureInfo culture)
        {
            culture = null;
            string stringContentCulture;

            if (data.TryGetValue("Culture", out stringContentCulture) && !stringContentCulture.IsNullOrEmpty())
            {
                try
                {
                    culture = CultureInfo.GetCultureInfo(stringContentCulture);
                }
                catch
                {
                }
            }

            return culture != null;
        }

        private IDictionary<String, String> GetData(ControllerContext controllerContext)
        {
            IDictionary<String, String> data = null;
            var contentType = controllerContext.HttpContext.Request.ContentType;

            if (contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                string json;

                using (var stream = controllerContext.HttpContext.Request.InputStream)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(stream))
                        json = reader.ReadToEnd();
                }

                if (!json.IsNullOrEmpty())
                {
                    data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json, new Newtonsoft.Json.JsonSerializerSettings { DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc });
                }
            }
            else
            {
                data = controllerContext.HttpContext.Request.Form.ToDictionary();
            }

            if (data != null)
            {
                //http://stackoverflow.com/questions/220020/how-to-handle-checkboxes-in-asp-net-mvc-forms
                var keys = new HashSet<string>();

                foreach (var item in data.Where(item => item.Value == "true,false"))
                {
                    keys.Add(item.Key);
                }

                foreach (var key in keys)
                {
                    data[key] = "true";
                }
            }

            return data;
        }
    }
}