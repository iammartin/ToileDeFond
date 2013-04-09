using System.ComponentModel.Composition;
using System.Reflection;
using MefContrib.Hosting.Conventions.Configuration;

namespace ToileDeFond.Modularity.Web
{
    [PrioritisedExport(typeof(IPartRegistryRetriever))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MvcControllerPartRegistryRetriever : IPartRegistryRetriever
    {
        public PartRegistry GetPartRegistryForAssembly(Assembly assembly)
        {
            return new MvcControllerPartRegistry(assembly);
        }

        //public void RegisterHttpModule(Assembly assembly)
        //{
        //    var type = typeof(IHttpModule);

        //    var moduleTypes = assembly.GetTypes().Where(type.IsAssignableFrom).ToList();

        //    moduleTypes.ForEach(DynamicModuleUtility.RegisterModule);
        //}
    }
}