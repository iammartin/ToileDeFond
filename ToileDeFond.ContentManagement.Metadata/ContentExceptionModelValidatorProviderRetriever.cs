using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Mvc;
using ToileDeFond.Modularity;
using ToileDeFond.Modularity.Web;

namespace ToileDeFond.ContentManagement.Metadata
{
    [PrioritisedExport(typeof(IModelValidatorProviderRetriever))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ContentExceptionModelValidatorProviderRetriever : IModelValidatorProviderRetriever
    {
        public IEnumerable<ModelValidatorProvider> GetModelValidatorProviders()
        {
            //yield return new ContentExceptionDataAnnotationsModelValidatorProvider();
            yield return new CustomValidatorProvider();
            yield return new ContentExceptionDataErrorInfoModelValidatorProvider();
            yield return new ContentExceptionClientDataTypeModelValidatorProvider();
        }
    }
}