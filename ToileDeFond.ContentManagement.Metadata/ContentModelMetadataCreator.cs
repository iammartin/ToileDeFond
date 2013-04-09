using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToileDeFond.Localization;
using ToileDeFond.Modularity;
using ToileDeFond.Modularity.Web.Metadata;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement.Metadata
{
    //TODO: Merger les dictionaries de Metadata des ContentType pour ensuite créer un ModelMetadataItem à partir du data à l'intérieur du dictionnaire
    //A voir: Comment gérer les valeurs qui sont des collections (ajout/suppression par overridde) Ex. règles de validations...
    //Le best serait de faire comme pour le web.config avec les version debug et release (du xml avec de l'overriding facile) - voir vidéo de hanselman pour comprend le fonctionnement...

    [PrioritisedExport(typeof(IModelMetadataItemCreator))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ContentModelMetadataCreator : IModelMetadataItemCreator
    {
        private readonly IContentManager _contentManager;
        private readonly ICultureManager _cultureManager;

        [ImportingConstructor]
        public ContentModelMetadataCreator(IContentManager contentManager, ICultureManager cultureManager)
        {
            _contentManager = contentManager;
            _cultureManager = cultureManager;
        }

        public bool IsKnownType(Type modelType)
        {
            return modelType.IsContent();
        }

        public ModelMetadata GetModelMetadata(ModelMetadataProvider provider, Func<object> modelAccessor, Type modelType)
        {
            ValidateContainerTypeIsContent(modelType);

            var contentType = GetContentType(modelAccessor == null ? null : modelAccessor());

            return GetContentModelMetadata(provider, contentType, null, modelAccessor, modelType, null);
        }

        private ContentType GetContentType(object model)
        {
            return model == null ? _contentManager.LoadContentType(HttpContext.Current.Request.QueryString["ContentTypeFullName"]) : ((Content)model).ContentType;
        }

        //Ici le modelAccessor est celui de la propriété
        public ModelMetadata GetModelPropertyMetadata(ModelMetadataProvider provider, Func<object> modelAccessor, Type containerType, string propertyName)
        {
            ValidateContainerTypeIsContent(containerType);

            if (propertyName == "Id")
            {
                return GetContentContentIdModelMetadata(provider, containerType, modelAccessor: modelAccessor);
            }

             if (propertyName == "Culture")
            {
                return GetContentCultureModelMetadata(provider, containerType, modelAccessor: modelAccessor);
            }

             if (propertyName == "ContentTypeFullName")
             {
                 return GetContentContentTypeFullNameModelMetadata(provider, containerType, modelAccessor: modelAccessor);
             }

            var contentType = _contentManager.LoadContentType(_contentManager.GetContentTypeFullNameFromPropertyFullName(propertyName));

            if (containerType == null)
                throw new InvalidOperationException("Cannot create metadata for unknown content type of property: " + propertyName);

            IContentTypeProperty contentTypeProperty;

            return contentType.TryGetProperty(propertyName, out contentTypeProperty) ?
                GetContentModelMetadata(provider, contentTypeProperty, containerType, modelAccessor, GetTypeOrDefaultType(contentTypeProperty), contentTypeProperty.FullName) : null;
        }

        public IEnumerable<ModelMetadata> GetModelPropertiesMetadata(ModelMetadataProvider provider, object container, Type containerType)
        {
            ValidateContainerTypeIsContent(containerType);

            var contentType = GetContentType(container);

            //TODO: La bonne culture HttpContext.Current.Request.QueryString["CultureName"]

            //TODO: N+1 avec GetPropertyEditor
            var propertiesModelMetadata = contentType.Properties.Select(contentTypeProperty =>
                {
                    //TODO: Revoir?
                    var type = GetTypeOrDefaultType(contentTypeProperty);


                    return GetContentModelMetadata(provider, contentTypeProperty, containerType, () => (container == null ?
                       contentTypeProperty.GetDefaultValue(type) : 
                       ((Content) container)[_cultureManager.GetDefaultCulture()][contentTypeProperty].GetValue(type)),
                                            type, contentTypeProperty.FullName);
                }).ToList();


            //Ajouter les propriétés "core" Ex. Culture, ContentTypeFullName

            propertiesModelMetadata.Add(GetContentCultureModelMetadata(provider, containerType, container));
            propertiesModelMetadata.Add(GetContentContentIdModelMetadata(provider, containerType,  container));
            propertiesModelMetadata.Add(GetContentContentTypeFullNameModelMetadata(provider, containerType, contentType));

            return propertiesModelMetadata;
        }

        private static Type GetTypeOrDefaultType(IContentTypeProperty contentTypeProperty)
        {
            Type type;
            if (!contentTypeProperty.TryGetMetadata("Type", out type))
            {
                type = typeof (string);
            }
            return type;
        }

        private ContentModelMetadata GetContentContentTypeFullNameModelMetadata(ModelMetadataProvider provider, Type containerType, ContentType contentType = null, Func<object> modelAccessor = null)
        {
            if (modelAccessor == null)
            {
                modelAccessor = () => contentType.FullName;
            }

            var modelMetadata = new ContentModelMetadata(provider, containerType, modelAccessor, typeof(string), "ContentTypeFullName");

            modelMetadata.TemplateHint = "HiddenInput";
            modelMetadata.HideSurroundingHtml = true;

            return modelMetadata;
        }

        private ContentModelMetadata GetContentContentIdModelMetadata(ModelMetadataProvider provider, Type containerType, object container = null, Func<object> modelAccessor = null)
        {
            if (modelAccessor == null)
            {
                if (container == null)
                {
                    modelAccessor = () => Guid.NewGuid();
                }
                else
                {
                    modelAccessor = () => ((Content)container).Id;
                }
            }

            var modelMetadata = new ContentModelMetadata(provider, containerType, modelAccessor, typeof(Guid), "Id");

            modelMetadata.TemplateHint = "HiddenInput";
            modelMetadata.HideSurroundingHtml = true;

            return modelMetadata;
        }

        private ContentModelMetadata GetContentCultureModelMetadata(ModelMetadataProvider provider, Type containerType, object container = null, Func<object> modelAccessor = null)
        {
            if (modelAccessor == null)
            {
                
                    modelAccessor = () => _cultureManager.GetDefaultCulture();
            }

            var modelMetadata = new ContentModelMetadata(provider, containerType, modelAccessor, typeof(CultureInfo), "Culture");

            modelMetadata.TemplateHint = "HiddenInput";
            modelMetadata.HideSurroundingHtml = true;

            return modelMetadata;
        }

        private ContentModelMetadata GetContentModelMetadata(ModelMetadataProvider provider, IEntityWithMetadata entityWithMetadata, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var metadata = new ContentModelMetadata(provider, containerType, modelAccessor, modelType, propertyName);

            metadata.ShowForDisplay = entityWithMetadata.GetMetadataOrDefault<bool>("ShowForDisplay");

            string displayName;
            if (entityWithMetadata.TryGetMetadata("DisplayName", out displayName))
            {
                metadata.DisplayName = displayName;
            }

            string shortDisplayName;
            if (entityWithMetadata.TryGetMetadata("ShortDisplayName", out shortDisplayName))
            {
                metadata.ShortDisplayName = shortDisplayName;
            }

            string templateHint;
            if (entityWithMetadata.TryGetMetadata("TemplateHint", out templateHint))
            {
                metadata.TemplateHint = templateHint;
            }
            else
            {
                metadata.TemplateHint = GetDefaultTemplateHint(provider, entityWithMetadata, containerType, modelAccessor, modelType, propertyName);
            }

            string description;
            if (entityWithMetadata.TryGetMetadata("Description", out description))
            {
                metadata.Description = description;
            }

            string nullDisplayText;
            if (entityWithMetadata.TryGetMetadata("NullDisplayText", out nullDisplayText))
            {
                metadata.NullDisplayText = nullDisplayText;
            }

            string watermark;
            if (entityWithMetadata.TryGetMetadata("Watermark", out watermark))
            {
                metadata.Watermark = watermark;
            }

            bool hideSurroundingHtml;
            if (entityWithMetadata.TryGetMetadata("HideSurroundingHtml", out hideSurroundingHtml))
            {
                metadata.HideSurroundingHtml = hideSurroundingHtml;
            }

            bool requestValidationEnabled;
            if (entityWithMetadata.TryGetMetadata("RequestValidationEnabled", out requestValidationEnabled))
            {
                metadata.RequestValidationEnabled = requestValidationEnabled;
            }

            bool isReadOnly;
            if (entityWithMetadata.TryGetMetadata("IsReadOnly", out isReadOnly))
            {
                metadata.IsReadOnly = isReadOnly;
            }

            bool isRequired;
            if (entityWithMetadata.TryGetMetadata("IsRequired", out isRequired))
            {
                metadata.IsRequired = isRequired;
            }

            bool showForEdit;
            if (entityWithMetadata.TryGetMetadata("ShowForEdit", out showForEdit))
            {
                metadata.ShowForEdit = showForEdit;
            }
            else
            {
                metadata.ShowForEdit = !metadata.IsReadOnly;
            }

            int order;
            if (entityWithMetadata.TryGetMetadata("Order", out order))
            {
                metadata.Order = order;
            }

            string displayFormatString;
            if (entityWithMetadata.TryGetMetadata("DisplayFormatString", out displayFormatString))
            {
                metadata.DisplayFormatString = displayFormatString;
            }


            string editFormatString;
            if (entityWithMetadata.GetMetadataOrDefault<bool>("ApplyFormatInEditMode") && metadata.ShowForEdit && entityWithMetadata.TryGetMetadata("EditFormatString", out editFormatString))
            {
                metadata.EditFormatString = editFormatString;
            }

            bool convertEmptyStringToNull;
            if (entityWithMetadata.TryGetMetadata("ConvertEmptyStringToNull", out convertEmptyStringToNull))
            {
                metadata.ConvertEmptyStringToNull = convertEmptyStringToNull;
            }

            return metadata;
        }

        private string GetDefaultTemplateHint(ModelMetadataProvider provider, IEntityWithMetadata entityWithMetadata, 
            Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            string result = null;

            if (modelType == typeof (List<Guid>))
            {
                result = "_ContentListPicker";
            }
            else if (modelType == typeof(Guid))
            {
                result = "_ContentPicker";
            }
            //else if (modelType == typeof(Content))
            //{
            //    result = propertyName == null ? "_Content" : "_ChildContentBuilder";
            //}
            //else if (modelType == typeof(List<Content>))
            //{
            //    result = "_ChildContentListBuilder";
            //}

            //switch (modelType --)
            //{
            //    case "System.Collections.Generic.List`1[[System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]":
            //        return "_MultiSelectList";
            //}

            return result;
        }

        protected virtual void ValidateContainerTypeIsContent(Type modelType)
        {
            ExceptionHandling.IsNotNull(modelType, "modelType");

            if (!modelType.IsContent())
                throw new ArgumentException("ContentModelMetadataItemCreator can only create ModelMetadataItem for Content type. ContentModelMetadataItemCreator cannot create ModelMetadataItem for type: " + modelType.FullName, "modelType");
        }

        //protected virtual ModelMetadataItem FromDictionary(IDictionary<string, string> dictionary)
        //{
        //    return JsonConvert.DeserializeObject<ModelMetadataItem>(JsonConvert.SerializeObject(dictionary));
        //}
    }
}
