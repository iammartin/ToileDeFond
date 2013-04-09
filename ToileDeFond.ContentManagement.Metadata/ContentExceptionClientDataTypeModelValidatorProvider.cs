using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ToileDeFond.ContentManagement.Metadata
{
    public class ContentExceptionClientDataTypeModelValidatorProvider : ClientDataTypeModelValidatorProvider
    {
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            if (metadata.ContainerType != null && (metadata.ContainerType == typeof(Content) || metadata.ContainerType == typeof(Content.ContentTranslation)))
            {
                return Enumerable.Empty<ModelValidator>();
            }

            return base.GetValidators(metadata, context);
        }
    }
}