using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Mvc;
using System.Web.Routing;

namespace ToileDeFond.Modularity.Web
{
    [PrioritisedExport(typeof(IViewEngine))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MvcViewEngine : RazorViewEngine
    {
        public MvcViewEngine()
        {
            FileExtensions = new[] { "cshtml" };

            PartialViewLocationFormats = new[]
                                    {
                                        "~/Views/{1}/{0}.cshtml",
                                        "~/Views/Shared/{0}.cshtml",
                                    };

            MasterLocationFormats = new[]
                                    {
                                        "~/Views/{1}/{0}.cshtml",
                                        "~/Views/Shared/{0}.cshtml",
                                    };

            ViewLocationFormats = new[]
                                    {
                                        "~/Views/{1}/{0}.cshtml",
                                        "~/Views/Shared/{0}.cshtml",
                                    };


            AreaPartialViewLocationFormats = new[]
                                    {
                                        "~/Areas/{2}/Views/{1}/{0}.cshtml",
										"~/Areas/{2}/Views/Shared/{0}.cshtml",
                                    };

            AreaViewLocationFormats = new[]
                                    {
                                        "~/Areas/{2}/Views/{1}/{0}.cshtml",
										 "~/Areas/{2}/Views/Shared/{0}.cshtml",
                                    };

            AreaMasterLocationFormats = new[]
                                    {
                                        "~/Areas/{2}/Views/{1}/{0}.cshtml",
										"~/Areas/{2}/Views/Shared/{0}.cshtml",
                                    };
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if(IsModule(controllerContext))
            {
                var validsPath = GetValidsVirtualPath(viewName, controllerContext.RouteData.DataTokens, controllerContext.RouteData.Values);

                foreach (var path in validsPath)
                {
                    if (VirtualPathProvider.FileExists(path))
                        return new ViewEngineResult(CreateView(controllerContext, path, string.Empty), this);
                }
            }

            return base.FindView(controllerContext, viewName, masterName, useCache);
        }

        //TODO: Pas certain d'aimer ca
        private bool IsModule(ControllerContext controllerContext)
        {
            return controllerContext.RouteData.Values.ContainsKey("module");
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if(IsModule(controllerContext))
            {
                var validsPath = GetValidsVirtualPath(partialViewName, controllerContext.RouteData.DataTokens, controllerContext.RouteData.Values/*, moduleControllerAssembly*/);

                foreach (var path in validsPath)
                {
                    if (VirtualPathProvider.FileExists(path))
                        return new ViewEngineResult(CreatePartialView(controllerContext, path), this);
                }
            }

            return base.FindPartialView(controllerContext, partialViewName, useCache);
        }

        private List<string> GetValidsVirtualPath(string viewName, RouteValueDictionary customRoute, RouteValueDictionary defaultRoute /*, Module module*/)
        {
            var validsPath = new List<string>();

            var controller = defaultRoute["controller"];
            var area = customRoute.ContainsKey("area") ? customRoute["area"].ToString() : String.Empty;

            // Physical Files
            foreach (var format in PartialViewLocationFormats)
                validsPath.Add(string.Format(format, viewName, controller));

            foreach (var format in AreaPartialViewLocationFormats)
                validsPath.Add(string.Format(format, viewName, controller, area));

            // Virtual Files
            foreach (var format in PartialViewLocationFormats)
                validsPath.Add(VirtualFileHelper.GetVirtualModulePath(String.Format(format, viewName, controller), area));

            foreach (var format in AreaPartialViewLocationFormats)
                validsPath.Add(VirtualFileHelper.GetVirtualModulePath(String.Format(format, viewName, controller, area), area));

            return validsPath;
        }

        //TODO: Faire que les _viewstart fonctionnent dans les modules!
        //http://stackoverflow.com/questions/9191498/mvc3-compiled-razor-view-cant-find-viewstart


        //On retourne l'assembly si le controlleur fait parti d'un module pour qu'on puisse faire du virtual path...
        //private Module GetControllerModule(ControllerContext controllerContext)
        //{
        //    var controllerType = controllerContext.Controller.GetType();
        //    return DependencyResolver.Current.GetService<IReflectionContentManager>().GetModuleByType(controllerType);
        //}
    }
}
