using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ToileDeFond.Modularity
{
    //TODO: Copy de celui de MVC... (à revoir...)
    //Agit comme proxy (enrobeur) pour pouvoir mettre n'importe quelle classe qui a les méthodes GetInstance & GetAllInstances en tant que vrai DependencyResolver
    public class DependencyResolver : IDisposable
    {
        // Static accessors 

        private static readonly DependencyResolver _instance = new DependencyResolver();
        private IDependencyResolver _current; /* = new DefaultDependencyResolver();*/

        public static IDependencyResolver Current
        {
            get { return _instance.InnerCurrent; }
        }

        public static bool IsSet
        {
            get { return _instance._current != null; }
        }

        public IDependencyResolver InnerCurrent
        {
            get
            {
                if (_current == null)
                    throw new NotImplementedException("Dependency resolver not set.");

                return _current;
            }
        }

        public void Dispose()
        {
            InnerCurrent.Dispose();
        }

        public static void SetResolver(IDependencyResolver resolver)
        {
            _instance.InnerSetResolver(resolver);
        }

        public static void SetResolver(object commonServiceLocator)
        {
            _instance.InnerSetResolver(commonServiceLocator);
        }

        public static void SetResolver(Func<Type, object> getService, Func<Type, IEnumerable<object>> getServices)
        {
            _instance.InnerSetResolver(getService, getServices);
        }

        // Instance implementation (for testing purposes)

        public void InnerSetResolver(IDependencyResolver resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException("resolver");
            }

            _current = resolver;
        }

        public void InnerSetResolver(object commonServiceLocator)
        {
            if (commonServiceLocator == null)
            {
                throw new ArgumentNullException("commonServiceLocator");
            }

            Type locatorType = commonServiceLocator.GetType();
            MethodInfo getInstance = locatorType.GetMethod("GetInstance", new[] {typeof (Type)});
            MethodInfo getInstances = locatorType.GetMethod("GetAllInstances", new[] {typeof (Type)});

            if (getInstance == null ||
                getInstance.ReturnType != typeof (object) ||
                getInstances == null ||
                getInstances.ReturnType != typeof (IEnumerable<object>))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                                                          "Dependency Resolver Does Not Implement ICommonServiceLocator: {0}",
                                                          locatorType.FullName), "commonServiceLocator");
            }

            var getService =
                (Func<Type, object>)
                Delegate.CreateDelegate(typeof (Func<Type, object>), commonServiceLocator, getInstance);
            var getServices =
                (Func<Type, IEnumerable<object>>)
                Delegate.CreateDelegate(typeof (Func<Type, IEnumerable<object>>), commonServiceLocator, getInstances);

            _current = new DelegateBasedDependencyResolver(getService, getServices);
        }

        public void InnerSetResolver(Func<Type, object> getService, Func<Type, IEnumerable<object>> getServices)
        {
            if (getService == null)
            {
                throw new ArgumentNullException("getService");
            }
            if (getServices == null)
            {
                throw new ArgumentNullException("getServices");
            }

            _current = new DelegateBasedDependencyResolver(getService, getServices);
        }

        // Helper classes

        private class DefaultDependencyResolver : IDependencyResolver
        {
            public object GetService(Type serviceType)
            {
                try
                {
                    return Activator.CreateInstance(serviceType);
                }
                catch
                {
                    return null;
                }
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                return Enumerable.Empty<object>();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
            }
        }

        private class DelegateBasedDependencyResolver : IDependencyResolver
        {
            private readonly Func<Type, object> _getService;
            private readonly Func<Type, IEnumerable<object>> _getServices;

            public DelegateBasedDependencyResolver(Func<Type, object> getService,
                                                   Func<Type, IEnumerable<object>> getServices)
            {
                _getService = getService;
                _getServices = getServices;
            }

            public object GetService(Type type)
            {
                try
                {
                    return _getService.Invoke(type);
                }
                catch
                {
                    return null;
                }
            }

            public IEnumerable<object> GetServices(Type type)
            {
                return _getServices(type);
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
            }
        }
    }
}