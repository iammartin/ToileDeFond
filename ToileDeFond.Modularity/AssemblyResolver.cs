using System;
using System.Linq;
using System.Reflection;

namespace ToileDeFond.Modularity
{
    public class AssemblyResolver
    {
        public Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string name = args.Name.Contains(",") ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name;
            Assembly assembly =
                AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Equals(name));

            return assembly;
        }
    }
}