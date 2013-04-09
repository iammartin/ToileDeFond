using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ToileDeFond.Modularity.Web
{
    public interface IModelValidatorProviderRetriever
    {
        IEnumerable<ModelValidatorProvider> GetModelValidatorProviders();
    }
}
