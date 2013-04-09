using System;
using System.ComponentModel.Composition;
using System.Web;

namespace ToileDeFond.Modularity.Web
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AutoRegisterHttpModuleAttribute : PrioritisedExportAttribute
    {
        public AutoRegisterHttpModuleAttribute()
            : base(typeof(IHttpModule))
        {
        }
    }
}
