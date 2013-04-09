using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using ToileDeFond.Modularity;
using ToileDeFond.Tests.FakeModules.First;

namespace ToileDeFond.Tests.FakeModules.Second
{
    [PrioritisedExport(typeof(IServiceWithDependencies))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ServiceWithDependencies : IServiceWithDependencies
    {
        private readonly List<IService> _services;

        [ImportingConstructor]
        public ServiceWithDependencies([ImportMany]IEnumerable<Lazy<IService, IPrioritisedMefMetaData>> services)
        {
            _services = services.OrderBy(x => x.Metadata.Priority).Select(x => x.Value).ToList();
        }


        [ImportMany(typeof(IService))]
        public IEnumerable<Lazy<IService, IPrioritisedMefMetaData>> Test3 { get; set; }

        public ReadOnlyCollection<IService> Services { get { return _services.AsReadOnly(); } }
    }
}