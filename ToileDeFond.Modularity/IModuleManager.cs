using System;
using System.Collections.Generic;
using System.Reflection;

namespace ToileDeFond.Modularity
{
    public interface IModuleManager : IDisposable
    {
        //TODO: Ajouter la focntion pour juste un module
        //Module(s) parce que c'est plus performant - évite de faire N+1
        void InstallOrUpdateModules(IList<Assembly> moduleAssemblies);
    }
}