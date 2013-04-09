using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ToileDeFond.Modularity;

namespace ToileDeFond.Tests.FakeModules.First
{
    public interface IServiceWithDependencies
    {
        ReadOnlyCollection<IService> Services { get; }
        IEnumerable<Lazy<IService, IPrioritisedMefMetaData>> Test3 { get; set; }
    }
}