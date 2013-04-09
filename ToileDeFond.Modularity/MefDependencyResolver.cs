using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ToileDeFond.Modularity
{
    //http://blogs.msdn.com/b/dsplaisted/archive/2010/07/13/how-to-debug-and-diagnose-mef-failures.aspx
    public class MefDependencyResolver : IDependencyResolver
    {
        private readonly IEnumerable<ComposablePartCatalog> _composablePartCatalogs;

        private CompositionContainer _container;

        public MefDependencyResolver(IEnumerable<ComposablePartCatalog> composablePartCatalogs)
        {
            _composablePartCatalogs = composablePartCatalogs;
        }

        public virtual CompositionContainer Container
        {
            get { return _container ?? (_container = GetContainer()); }
        }

        public IEnumerable<ComposablePartCatalog> ComposablePartCatalogs
        {
            get { return _composablePartCatalogs; }
        }

        public object GetService(Type serviceType)
        {
            List<Lazy<object, object>> exports = GetValidExports(serviceType).ToList();
            object obj = exports.Any() ? exports.First().Value : null;

            return obj;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            List<Lazy<object, object>> exports = GetValidExports(serviceType).ToList();
            IEnumerable<object> obj = exports.Any()
                                          ? exports.Select(e => e.Value).AsEnumerable()
                                          : Enumerable.Empty<object>();

            return obj;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        private CompositionContainer GetContainer()
        {
            var mainCatalog = new AggregateCatalog(ComposablePartCatalogs);

            //TODO: Enlever CompositionOptions.DisableSilentRejection
            var container = new CompositionContainer(mainCatalog, CompositionOptions.DisableSilentRejection);
            //TODO: ThreadSafe - var container = new CompositionContainer(mainCatalog, true);

            //TODO: Filtrer les parts pas rapport - exemple tout ce qui vient de RavenDB!!

            container.ComposeParts();

            return container;
        }

        private IEnumerable<Lazy<object, object>> GetValidExports(Type serviceType)
        {
            IEnumerable<Lazy<object, object>> x;

            try
            {
                x = Container.GetExports(serviceType, null, null);
            }
            catch (ReflectionTypeLoadException ex)
            {
                var lines = new List<string>();

                foreach (Exception l in ex.LoaderExceptions)
                {
                    lines.Add(l.Message);
                }

                File.WriteAllLines(@"C:\loaderexceptions.txt", lines);

                throw;
            }

            return x;
        }

        public virtual void DoDispose()
        {
            _container.Dispose();
        }
    }
}