using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ToileDeFond.Modularity.Web.Metadata
{
    //This is a singleton (by MVC framework....) so we cannot inject dependencies
    //We have to do poor man dependency injection
    public class ExtendedModelMetadataProvider : ConventionsDataAnnotationsModelMetadataProvider
    {
        public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
        {
            if (modelType == null)
            {
                var result = base.GetMetadataForType(modelAccessor, modelType);

                return result;
            }

            var modelMetadataItemCreator = GetModelMetadataCreator(modelType);

            if (modelMetadataItemCreator == null)
            {
                var result = base.GetMetadataForType(modelAccessor, modelType);

                return result;
            }

            var metadataItem = modelMetadataItemCreator.GetModelMetadata(this, modelAccessor, modelType);

            return metadataItem ?? base.GetMetadataForType(modelAccessor, modelType) /*ModelMetadataFromModelMetadataItem(metadataItem, null, modelAccessor, modelType, null)*/;

            //ModelMetadata modelMetadata = new ExtendedModelMetadata(this, null, modelAccessor, modelType, null, metadataItem);
        }

        public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
        {
            if (containerType == null)
            {
                var result = base.GetMetadataForProperties(container, containerType);

                return result;
            }

            var modelMetadataItemCreator = GetModelMetadataCreator(containerType);

            if (modelMetadataItemCreator == null)
            {
                var result = base.GetMetadataForProperties(container, containerType);

                return result;
            }

            var metadataItems = modelMetadataItemCreator.GetModelPropertiesMetadata(this, container, containerType);

            if (metadataItems == null)
            {
                var result = base.GetMetadataForProperties(container, containerType);

                return result;
            }

            //var modelMetadata = ModelMetadataFromModelMetadataItem(metadataItem, containerType, () => tempDescriptor.GetValue(container), tempDescriptor.PropertyType, tempDescriptor.Name);

            return metadataItems;
        }

        public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
        {
            if (containerType == null)
            {
                var result = base.GetMetadataForProperty(modelAccessor, containerType, propertyName);

                return result;
            }

            var modelMetadataItemCreator = GetModelMetadataCreator(containerType);

            if (modelMetadataItemCreator == null)
            {
                var result = base.GetMetadataForProperty(modelAccessor, containerType, propertyName);

                return result;
            }

            var metadataItem = modelMetadataItemCreator.GetModelPropertyMetadata(this, modelAccessor, containerType, propertyName);

            if (metadataItem == null)
            {
                var result = base.GetMetadataForProperty(modelAccessor, containerType, propertyName);

                return result;
            }

            //var propertyDescriptor = TypeDescriptor.GetProperties(containerType).Cast<PropertyDescriptor>()
            //    .FirstOrDefault(property => property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

            //if (propertyDescriptor == null)
            //{
            //    throw new ArgumentException(string.Format(ExceptionMessages.ThePropertyNameOfTypeCouldNotBeFound, containerType.FullName, propertyName));
            //}

            //return ModelMetadataFromModelMetadataItem(metadataItem, containerType, modelAccessor, propertyDescriptor.PropertyType, propertyName);

            return metadataItem;

            //ModelMetadata modelMetadata = new ExtendedModelMetadata(this, containerType, modelAccessor, propertyType, propertyName, propertyMetadata);
        }

        #region private

        private IModelMetadataItemCreator GetModelMetadataCreator(Type containerType)
        {
            var modelMetadataItemCreators = DependencyResolver.Current.GetServices<IModelMetadataItemCreator>();

            return modelMetadataItemCreators.FirstOrDefault(c => c.IsKnownType(containerType));
        }

        //private ModelMetadata ModelMetadataFromModelMetadataItem(ModelMetadataItem metadataItem, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        // private ModelMetadata ModelMetadataFromModelMetadataItem(ModelMetadataItem metadataItem)
        //{
        //    var metadata = new ModelMetadata(this, metadataItem.ContainerType, metadataItem.ModelAccessor, metadataItem.ModelType, metadataItem.PropertyName);

        //    metadata.ShowForDisplay = metadataItem.ShowForDisplay;

        //    if (metadataItem.DisplayName != null)
        //    {
        //        metadata.DisplayName = metadataItem.DisplayName;
        //    }

        //    if (metadataItem.ShortDisplayName != null)
        //    {
        //        metadata.ShortDisplayName = metadataItem.ShortDisplayName;
        //    }

        //    if (!string.IsNullOrEmpty(metadataItem.TemplateHint))
        //    {
        //        metadata.TemplateHint = metadataItem.TemplateHint;
        //    }

        //    if (metadataItem.Description != null)
        //    {
        //        metadata.Description = metadataItem.Description;
        //    }

        //    if (metadataItem.NullDisplayText != null)
        //    {
        //        metadata.NullDisplayText = metadataItem.NullDisplayText;
        //    }

        //    if (metadataItem.Watermark != null)
        //    {
        //        metadata.Watermark = metadataItem.Watermark;
        //    }

        //    if (metadataItem.HideSurroundingHtml.HasValue)
        //    {
        //        metadata.HideSurroundingHtml = metadataItem.HideSurroundingHtml.Value;
        //    }

        //    if (metadataItem.RequestValidationEnabled.HasValue)
        //    {
        //        metadata.RequestValidationEnabled = metadataItem.RequestValidationEnabled.Value;
        //    }

        //    if (metadataItem.IsReadOnly.HasValue)
        //    {
        //        metadata.IsReadOnly = metadataItem.IsReadOnly.Value;
        //    }

        //    if (metadataItem.IsRequired.HasValue)
        //    {
        //        metadata.IsRequired = metadataItem.IsRequired.Value;
        //    }

        //    if (metadataItem.ShowForEdit.HasValue)
        //    {
        //        metadata.ShowForEdit = metadataItem.ShowForEdit.Value;
        //    }
        //    else
        //    {
        //        metadata.ShowForEdit = !metadata.IsReadOnly;
        //    }

        //    if (metadataItem.Order.HasValue)
        //    {
        //        metadata.Order = metadataItem.Order.Value;
        //    }

        //    if (metadataItem.DisplayFormatString != null)
        //    {
        //        metadata.DisplayFormatString = metadataItem.DisplayFormatString;
        //    }

        //    if (metadataItem.ApplyFormatInEditMode && metadata.ShowForEdit && metadataItem.EditFormatString != null)
        //    {
        //        metadata.EditFormatString = metadataItem.EditFormatString;
        //    }

        //    if (metadataItem.ConvertEmptyStringToNull.HasValue)
        //    {
        //        metadata.ConvertEmptyStringToNull = metadataItem.ConvertEmptyStringToNull.Value;
        //    }

        //    return metadata;
        //}

        #endregion
    }
}
