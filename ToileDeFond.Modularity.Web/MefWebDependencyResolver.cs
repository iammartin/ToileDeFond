using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Web;
using MefContrib.Hosting.Filter;
using MefContrib.Web.Mvc;
using System.ComponentModel.Composition;

namespace ToileDeFond.Modularity.Web
{
    public class MefWebDependencyResolver : MefDependencyResolver, IDependencyBuilder, IServiceProvider, ICompositionContainerProvider
    {
        private const string HttpContextKey = "__CompositionDependencyResolver_Container";

        public MefWebDependencyResolver(IEnumerable<ComposablePartCatalog> composablePartCatalogs)
            : base(composablePartCatalogs)
        {

        }

        public override CompositionContainer Container
        {
            get
            {
                if (HttpContext.Current == null)
                    return base.Container;

                if (!CurrentRequestContext.Items.Contains(HttpContextKey))
                {
                    CurrentRequestContext.Items.Add(HttpContextKey, GetContainer());
                }

                return (CompositionContainer)CurrentRequestContext.Items[HttpContextKey];
            }
        }

        protected virtual CompositionContainer GetContainer()
        {
            var mainCatalog = new AggregateCatalog(ComposablePartCatalogs);
            //var mainProvider = new CatalogExportProvider(mainCatalog);
            var globalMainCatalog = new FilteringCatalog(mainCatalog, new WebFiltering(PartCreationScope.Global));
            var globalMainProvider = new CatalogExportProvider(globalMainCatalog);
            var perRequestMainCatalog = new FilteringCatalog(mainCatalog, new WebFiltering(PartCreationScope.PerRequest));
            var perRequestMainProvider = new CatalogExportProvider(perRequestMainCatalog);

            //var priorityCatalog = new PriorityCatalog(mainCatalog);
            //_priorityProvider = new CatalogExportProvider(priorityCatalog);
            //_perRequestPriorityCatalog = new FilteringCatalog(priorityCatalog, new WebFiltering(PartCreationScope.PerRequest));
            //_perRequestPriorityProvider = new CatalogExportProvider(_perRequestPriorityCatalog);

            var container = new CompositionContainer(null, true, perRequestMainProvider, globalMainProvider);
            globalMainProvider.SourceProvider = container;
            perRequestMainProvider.SourceProvider = container;

            container.ComposeParts();

            return container;
        }

        //TODO: A quoi ca sert ???
        public T Build<T>(T service)
        {
            Container.SatisfyImportsOnce(service);
            return service;
        }

        public override void DoDispose()
        {
            base.DoDispose();

            if (CurrentRequestContext.Items.Contains(HttpContextKey))
            {
                var x = (CompositionContainer)CurrentRequestContext.Items[HttpContextKey];

                if (x != null)
                    x.Dispose();
            }
        }

        //Nécessaire pour que System.Web.Mvc.DependencyResolver.SetResolver(dependencyResolver) fonctionner.... weid
        #region

        //TODO: Rempacer getservice par getinstance dans pas web..
        public object GetInstance(Type serviceType)
        {
            return GetService(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return GetServices(serviceType);
        }

        public TService GetInstance<TService>()
        {
            return System.Web.Mvc.DependencyResolverExtensions.GetService<TService>(this);
        }

        public TService GetInstance<TService>(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return System.Web.Mvc.DependencyResolverExtensions.GetServices<TService>(this);
        }

        #endregion
    }
}