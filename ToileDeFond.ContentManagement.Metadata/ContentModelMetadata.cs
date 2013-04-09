using System;
using System.Web.Mvc;

namespace ToileDeFond.ContentManagement.Metadata
{
    public class  ContentModelMetadata : ModelMetadata
    {
        public ContentModelMetadata(ModelMetadataProvider provider, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName) : base(provider, containerType, modelAccessor, modelType, propertyName)
        {
        }
    }
}