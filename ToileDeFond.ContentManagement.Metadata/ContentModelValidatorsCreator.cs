using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Mvc;
using ToileDeFond.Modularity;
using ToileDeFond.Modularity.Web.Metadata;
using DependencyResolver = ToileDeFond.Modularity.DependencyResolver;

namespace ToileDeFond.ContentManagement.Metadata
{
    [PrioritisedExport(typeof(IModelValidatorsCreator))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ContentModelValidatorsCreator : IModelValidatorsCreator
    {
        private readonly IContentModelValidatorRepository _contentModelValidatorRepository;

        [ImportingConstructor]
        public ContentModelValidatorsCreator(IContentModelValidatorRepository contentModelValidatorRepository)
        {
            _contentModelValidatorRepository = contentModelValidatorRepository;
        }

        public bool IsKnownType(Type containerType)
        {
            return containerType == typeof(Content);
        }

        public IEnumerable<ModelValidator> GetModelValidators(ModelMetadata metadata, ControllerContext context)
        {
            var isPropertyValidation = metadata.ContainerType != null && !String.IsNullOrEmpty(metadata.PropertyName);

            var validationRules = isPropertyValidation ? _contentModelValidatorRepository.GetValidationRulesForContentTypeProperty(metadata.PropertyName) : _contentModelValidatorRepository.GetValidationRulesForContentType(metadata.ContainerType.FullName);

            foreach (var validationRule in validationRules)
            {
                var modelValidatorClassFullName = validationRule["ModelValidatorClassFullName"].GetValue<string>();

                var type =Type.GetType(modelValidatorClassFullName);

                var validator = (ContentModelValidator)DependencyResolver.Current.GetService(type);

                validator.Initialize(validationRule);

                yield return validator;
            }
        }
    }
}
