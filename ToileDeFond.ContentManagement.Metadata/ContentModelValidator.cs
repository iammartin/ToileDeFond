using System.Web.Mvc;

namespace ToileDeFond.ContentManagement.Metadata
{
    public abstract class ContentModelValidator : ModelValidator
    {
        protected ContentModelValidator(ModelMetadata metadata, ControllerContext controllerContext) : base(metadata, controllerContext)
        {
        }

        public abstract void Initialize(Content validationRule);
    }
}