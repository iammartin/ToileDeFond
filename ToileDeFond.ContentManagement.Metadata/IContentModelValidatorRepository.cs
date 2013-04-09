using System.Collections.Generic;

namespace ToileDeFond.ContentManagement.Metadata
{
    public interface IContentModelValidatorRepository
    {
        IEnumerable<Content> GetValidationRulesForContentTypeProperty(string propertyName);
        IEnumerable<Content> GetValidationRulesForContentType(string fullName);
    }
}