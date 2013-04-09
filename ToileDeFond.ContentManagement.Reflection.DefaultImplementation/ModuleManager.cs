using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using ToileDeFond.Modularity;
using System.Linq;

namespace ToileDeFond.ContentManagement.Reflection.DefaultImplementation
{
    //TODO: Déplacer dans un autre module
    [PrioritisedExport(typeof(IModuleManager), 1)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ModuleManager : IModuleManager
    {
        private readonly IReflectionContentManager _reflectionContentManager;

        [ImportingConstructor]
        public ModuleManager(IReflectionContentManager reflectionContentManager)
        {
            _reflectionContentManager = reflectionContentManager;
        }

        public void InstallOrUpdateModules(IList<Assembly> moduleAssemblies)
        {
            var reports = _reflectionContentManager.GetNewOrUpdatedModules(moduleAssemblies);

            foreach (var report in reports.Where(r => r.Action != CreateOrUpdateActions.Unchanged))
            {
                _reflectionContentManager.Store(report.Item);
            }
            
            _reflectionContentManager.SaveChanges();
        }

        public void Dispose()
        {
            _reflectionContentManager.Dispose();
        }
    }
}
