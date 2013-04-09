using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Mvc;
using ToileDeFond.Modularity;

namespace ToileDeFond.ContentManagement.Metadata
{
    [PrioritisedExport(typeof(ContentModelValidator))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RegexContentModelValidator : ContentModelValidator
    {
        [ImportingConstructor]
        public RegexContentModelValidator(ModelMetadata metadata, ControllerContext controllerContext)
            : base(metadata, controllerContext)
        {
        }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(Content validationRule)
        {
            throw new NotImplementedException();
        }
    }
}
