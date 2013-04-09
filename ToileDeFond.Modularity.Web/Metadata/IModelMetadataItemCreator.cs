using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ToileDeFond.Modularity.Web.Metadata
{
    public interface IModelMetadataItemCreator
    {
        bool IsKnownType(Type type);

        /// <summary>
        /// Gets the model metadata.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <returns></returns>
        ModelMetadata GetModelMetadata(ModelMetadataProvider provider, Func<object> modelAccessor, Type modelType);

        /// <summary>
        /// Gets the model property metadata.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        ModelMetadata GetModelPropertyMetadata(ModelMetadataProvider provider, Func<object> modelAccessor, Type modelType, string propertyName);

        /// <summary>
        /// Gets the model properties metadata.
        /// </summary>
        /// <param name="container"> </param>
        /// <param name="modelType">Type of the model.</param>
        /// <returns></returns>
        IEnumerable<ModelMetadata> GetModelPropertiesMetadata(ModelMetadataProvider provider, object container, Type modelType);
    }
}
