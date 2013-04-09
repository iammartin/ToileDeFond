using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ToileDeFond.ContentManagement.Metadata
{
    public class CustomValidatorProvider : ModelValidatorProvider
    {
        private readonly DataAnnotationsModelValidatorProvider _provider = new DataAnnotationsModelValidatorProvider();

        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            if (metadata.ModelType == typeof(Content) || metadata.ContainerType == typeof(Content))
            {
                return Enumerable.Empty<ModelValidator>();
            }

            return _provider.GetValidators(metadata, context);
        }
    }
}