using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ToileDeFond.Modularity.Web.Metadata
{
    public class ExtendedModelValidatorProvider : ModelValidatorProvider
    {
        private IModelValidatorsCreator GetModelValidatorsCreator(Type containerType)
        {
            var modelValidatorsCreator = DependencyResolver.Current.GetServices<IModelValidatorsCreator>();

            return modelValidatorsCreator.FirstOrDefault(c => c.IsKnownType(containerType));
        }

        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            var validators = Enumerable.Empty<ModelValidator>();

            if (metadata.ContainerType != null)
            {
                var modelValidatorsCreator = GetModelValidatorsCreator(metadata.ContainerType);

                if (modelValidatorsCreator != null)
                {
                    var modelValidators = modelValidatorsCreator.GetModelValidators(metadata, context);

                    if (modelValidators != null)
                    {
                        return modelValidators;
                    }
                }
            }

            return validators;
        }
    }
}