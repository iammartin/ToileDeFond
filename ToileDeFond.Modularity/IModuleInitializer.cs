using System.ComponentModel.Composition;

namespace ToileDeFond.Modularity
{
    [InheritedExport]
    public interface IModuleInitializer : IPartImportsSatisfiedNotification
    {
        void Initialize(IDependencyResolver dependencyResolver);
        void PreInitialize();
    }
}