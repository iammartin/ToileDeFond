using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using ToileDeFond.Modularity.Web.Metadata;
using MefContrib.Web.Mvc;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

namespace ToileDeFond.Modularity.Web
{
    public abstract class WebStarterBase : StarterBase
    {
        //This must be called before AppStart (WebActivator)
        public override List<Tuple<Assembly, Type>> Start()
        {
            var loadedModuleAssemblies = base.Start();

            if (System.Web.Mvc.DependencyResolver.Current == DependencyResolver.Current)
                return loadedModuleAssemblies;

            // Tell MVC3 to use MEF as its dependency resolver.
            System.Web.Mvc.DependencyResolver.SetResolver(DependencyResolver.Current);

            // Tell Web API to use MEF as its dependency resolver.
            GlobalConfiguration.Configuration.DependencyResolver = new WebApiDependencyResolverConverter(DependencyResolver.Current);

            DynamicModuleUtility.RegisterModule(typeof(ContainerHttpModule));

            //var moduleRetriever = dependencyResolver.GetService<IHttpModuleRetriever>();
            //moduleRetriever.RegisterModules();

            // Tell MVC3 to resolve dependencies in controllers
            //ControllerBuilder.Current.SetControllerFactory(new ModuleControllerFactory(new CompositionControllerActivator(DependencyResolver as IDependencyBuilder)));

            // Tell MVC3 to resolve dependencies in filters
            //FilterProviders.Providers.Remove(FilterProviders.Providers.Single(f => f is FilterAttributeFilterProvider));
            //FilterProviders.Providers.Add(new CompositionFilterAttributeFilterProvider(_compositionManager));

            // Tell MVC3 to resolve dependencies in model validators
            //ModelValidatorProviders.Providers.Remove(ModelValidatorProviders.Providers.OfType<DataAnnotationsModelValidatorProvider>().Single());
            //ModelValidatorProviders.Providers.Add(new CompositionDataAnnotationsModelValidatorProvider(_compositionManager));
            //ModelValidatorProviders.Providers.Remove(ModelValidatorProviders.Providers.OfType<DataAnnotationsModelValidatorProvider>().Single());

            //TODO: Revoir...

            //Rendu a enlever ca -  ca empeche tuutes les validations normales
            //Je crois qu'il faudrait genre hériter des default ModelValidatorProviders (3) et si c'Est un content - skip en plus d'ajouter des ModelValidatorProviders pour les Content
            //Ensuite tester le login et s'assurer que la validation fonctionne avec bootstrap et aussi coté client
            #region a-enlever

            //for (var i = 0; i < ModelValidatorProviders.Providers.Count; i++)
            //{
            //    ModelValidatorProviders.Providers.RemoveAt(i);
            //}

            ModelValidatorProviders.Providers.Clear();

            var modelValidatorProviderRetrievers = DependencyResolver.Current.GetServices<IModelValidatorProviderRetriever>();

            foreach (var modelValidatorProviderRetriever in modelValidatorProviderRetrievers)
            {
                var providers = modelValidatorProviderRetriever.GetModelValidatorProviders();

                foreach (var modelValidatorProvider in providers)
                {
                    ModelValidatorProviders.Providers.Add(modelValidatorProvider);
                }
            }

            //ModelValidatorProviders.Providers.Add(new ExtendedModelValidatorProvider());

            #endregion

            // Tell MVC3 to resolve model binders through MEF. Note that a model binder should be decorated
            // with [ModelBinderExport].
            ModelBinderProviders.BinderProviders.Add(new CompositionModelBinderProvider(DependencyResolver.Current as ICompositionContainerProvider));

            //Tell MVC3 to use our custom ModelMetadataProvider
            ModelMetadataProviders.Current = new ExtendedModelMetadataProvider();

            // ADD MV3 View Engine
            //ViewEngines.Engines.Clear();
            //ViewEngines.Engines.Add(new MvcViewEngine());
            //TODO: ViewEngines.Engines.Add(new WebFormViewEngine());

            // Add path provider for module
            HostingEnvironment.RegisterVirtualPathProvider(new ModuleVirtualPathProvider());

            // Razor views assemblies injections
            //TODO:
            //RazorBuildProvider.CodeGenerationStarted += RazorBuildProviderCodeGenerationStarted;

            // Register MEF HttpModule
            DynamicModuleUtility.RegisterModule(typeof(CompositionContainerLifetimeHttpModule));

            return loadedModuleAssemblies;
        }

        protected override IDependencyResolver GetDependencyResolver(IEnumerable<ComposablePartCatalog> composablePartCatalogs)
        {
            return new MefWebDependencyResolver(composablePartCatalogs);
        }

        //Le Build Manager est utilisé par asp.net pour compiler les bon vieux .aspx (je crois)
        protected override void InstallOrUpdateModules(List<Tuple<Assembly, Type>> moduleAssemblies, IDependencyResolver dependencyResolver)
        {
            base.InstallOrUpdateModules(moduleAssemblies, dependencyResolver);

            foreach (var moduleAssembly in moduleAssemblies)
            {
                AddAssemblyToBuildManager(moduleAssembly.Item1);
            }
        }

        protected virtual void AddAssemblyToBuildManager(Assembly assembly)
        {
            try
            {
                BuildManager.AddReferencedAssembly(assembly);
            }
            catch (Exception ex) { }
        }

        //protected override void CurrentModuleInstalled(object sender, AssemblyEventArgs e)
        //{
        //    ModuleMvcArea.AreaRefresh();
        //}

        //protected virtual void RazorBuildProviderCodeGenerationStarted(object sender, EventArgs e)
        //{
        //    var provider = (RazorBuildProvider)sender;

        //    .Current.GetService<IReflectionContentManager>().GetAllAssemblies().ForEach(provider.AssemblyBuilder.AddAssemblyReference);
        //}
    }
}