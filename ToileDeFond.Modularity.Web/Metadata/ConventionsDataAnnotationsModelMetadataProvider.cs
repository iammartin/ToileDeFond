using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ToileDeFond.Modularity.Web.Metadata
{
    public class ConventionsDataAnnotationsModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
        {
            var metedatas = base.GetMetadataForProperties(container, containerType);

            return metedatas;
        }

        public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
        {
            var metedata = base.GetMetadataForProperty(modelAccessor, containerType, propertyName);

            //AddContainerInstanceToMetadata(modelAccessor, metedata);

            return metedata;
        }

        public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
        {
            var metedata = base.GetMetadataForType(modelAccessor, modelType);

            return metedata;
        }

        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var modelMetadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            AddContainerInstanceToMetadata(modelAccessor, modelMetadata);

            return modelMetadata;
        }

        private static void AddContainerInstanceToMetadata(Func<object> modelAccessor, ModelMetadata modelMetadata)
        {
            // add container instance
            if (modelAccessor != null && modelAccessor.Target != null)
            {
                var containerField = modelAccessor.Target.GetType().GetField("container");
                if (containerField != null)
                {
                    var containerValue = containerField.GetValue(modelAccessor.Target);
                    modelMetadata.SetContainerInstance(containerValue);
                }
            }
        }
    }
}