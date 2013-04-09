using System;
using System.Collections.Generic;

namespace ToileDeFond.Modularity
{
    public interface IDependencyResolver : IDisposable
    {
        object GetService(Type serviceType);
        IEnumerable<object> GetServices(Type serviceType);
        void Reset();
    }
}