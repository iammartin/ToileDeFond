using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web;

namespace ToileDeFond.Modularity.Web
{
    [PrioritisedExport(typeof(IHttpModuleRetriever))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HttpModuleRetriever : IHttpModuleRetriever
    {
        [ImportMany(typeof(IHttpModule))]
        public IEnumerable<Lazy<IHttpModule, IPrioritisedMefMetaData>> Modules
        {
            get;
            set;
        }
    }
}
