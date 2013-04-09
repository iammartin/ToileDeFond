using System.ComponentModel.Composition;
using ToileDeFond.Modularity;
using ToileDeFond.Tests.FakeModules.First;

namespace ToileDeFond.Tests.FakeModules.Second
{
    [PrioritisedExport(typeof(IService), 2)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Service2 : IService
    {
    }
}