using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ToileDeFond.Modularity
{
    //TODO: Déplacer? refactorer? - inquiétudes de performance aussi
    public static class ModuleUtilities
    {
        private static readonly string[] CoreModuleNames = new[] {typeof (StarterBase).Assembly.GetName().Name};
        private static readonly Type ModuleInitializerType = typeof (IModuleInitializer);

        public static List<Tuple<Assembly, Type>> GetLoadedModuleAssemblies(IList<Assembly> allLoadedAssemblies = null,
                                                                            string[] exceptModuleNames = null)
        {
            allLoadedAssemblies = allLoadedAssemblies ?? AppDomain.CurrentDomain.GetAssemblies();
            exceptModuleNames = exceptModuleNames ?? CoreModuleNames;

            var moduleAssemblies = new List<Tuple<Assembly, Type>>();

            foreach (Assembly assembly in allLoadedAssemblies.Where(a => !exceptModuleNames.Contains(a.GetName().Name)))
            {
                //TODO: Gérer les erreurs de SingleOrDefault
                Type moduleInitializerType = GetModuleInitializerFromAssembly(assembly);

                if (moduleInitializerType != null)
                {
                    moduleAssemblies.Add(new Tuple<Assembly, Type>(assembly, moduleInitializerType));
                }
            }

            return moduleAssemblies;
        }

        public static Type GetModuleInitializerFromAssembly(Assembly assembly)
        {
            return assembly.GetTypes().SingleOrDefault(t => ModuleInitializerType.IsAssignableFrom(t));
        }
    }
}