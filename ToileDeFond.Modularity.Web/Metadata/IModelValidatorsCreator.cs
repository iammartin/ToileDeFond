using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ToileDeFond.Modularity.Web.Metadata
{
    public interface IModelValidatorsCreator
    {
        bool IsKnownType(Type containerType);
        IEnumerable<ModelValidator> GetModelValidators(ModelMetadata metadata, ControllerContext context);
    }
}