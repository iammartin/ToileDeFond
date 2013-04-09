using System.ComponentModel.Composition;
using ToileDeFond.Modularity;
using ToileDeFond.Tests.FakeModules.First;

namespace ToileDeFond.Tests.FakeModules.Second
{
    [PrioritisedExport(typeof(IService), 1)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Service1 : IService
    {
    }
}
