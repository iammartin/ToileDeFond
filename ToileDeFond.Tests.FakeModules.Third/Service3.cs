using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;
using ToileDeFond.Modularity;
using ToileDeFond.Tests.FakeModules.First;

namespace ToileDeFond.Tests.FakeModules.Third
{
    [PrioritisedExport(typeof(IService), 3)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Service3 : IService
    {
    }
}
