using System;
using System.Collections.Generic;
using System.Web;

namespace ToileDeFond.Modularity.Web
{
    public interface IHttpModuleRetriever
    {
        IEnumerable<Lazy<IHttpModule, IPrioritisedMefMetaData>> Modules { get; }
    }
}