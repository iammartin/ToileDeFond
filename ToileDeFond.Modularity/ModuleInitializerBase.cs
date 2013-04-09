namespace ToileDeFond.Modularity
{
    public abstract class ModuleInitializerBase : IModuleInitializer
    {
        public virtual void OnImportsSatisfied()
        {
        }

        public virtual void PreInitialize()
        {
        }

        public virtual void Initialize(IDependencyResolver dependencyResolver)
        {
        }
    }
}