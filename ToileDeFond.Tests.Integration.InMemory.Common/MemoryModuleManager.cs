using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using ToileDeFond.Modularity;

namespace ToileDeFond.Tests.Integration.InMemory.Common
{
    [PrioritisedExport(typeof (IModuleManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MemoryModuleManager : IModuleManager
    {
        public void Dispose()
        {
        }

        public void InstallOrUpdateModules(IList<Assembly> moduleAssemblies)
        {
        }
    }
}