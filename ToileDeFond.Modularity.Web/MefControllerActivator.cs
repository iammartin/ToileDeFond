using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace ToileDeFond.Modularity.Web
{
    //TODO: MAgie MEFContrib ??? est-ce utiliser - a tester (enlever et voir si tout fonctionne encore)
    [PrioritisedExport(typeof(IControllerActivator))]
    public class MefControllerActivator : IControllerActivator
    {
        public IController Create(RequestContext requestContext, Type controllerType)
        {
            return DependencyResolver.Current.GetService(controllerType) as IController;
        }
    }
}
