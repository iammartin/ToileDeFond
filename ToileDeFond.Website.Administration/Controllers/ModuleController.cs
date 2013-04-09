using System;
using System.Globalization;
using System.Web.Mvc;
using ToileDeFond.ContentManagement;
using ToileDeFond.ContentManagement.Reflection;
using ToileDeFond.Website.Administration.Models;

namespace ToileDeFond.Website.Administration.Controllers
{
    [Authorize]
    public class ModuleController : Controller
    {
        private readonly IReflectionContentManager _reflectionContentManager;

        public static readonly Type[] BuiltInTypes = new[] { typeof(bool), typeof(byte), typeof(char), typeof(DateTime), typeof(decimal), typeof(double), typeof(Int16), typeof(Int32), typeof(Int64),
                typeof(sbyte), typeof(Single), typeof(string), typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(CultureInfo), typeof(Guid), typeof(Content)};

        public ModuleController(IReflectionContentManager moduleManager)
        {
            _reflectionContentManager = moduleManager;
        }

        //[GET("admin/modules")]
        [HttpGet]
        public ActionResult Modules()
        {
            var viewModel = new ModulesViewModel
                                {
                                    ModuleInfos = _reflectionContentManager.GetModuleInfos()
                                };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Modules(string moduleName) //install a module
        {
            var success = false;

            CreateOrUpdateReport<Module> moduleInstallOrUpdateReport = null;

            try
            {
                moduleInstallOrUpdateReport = _reflectionContentManager.GetNewOrUpdatedModule(moduleName);
                _reflectionContentManager.Store(moduleInstallOrUpdateReport.Item);
                _reflectionContentManager.SaveChanges();
                success = true;
            }
            catch (Exception exception)
            {
                //TODO: Log

            }

            //TODO: Trouver une solution au UrlRewriting pour que les ActionLink et RedirectToAction etc fonctionne vers les url réécrites

            var viewModel = new ModulesViewModel
                                {
                                    ModuleInfos = _reflectionContentManager.GetModuleInfos(),
                                    Message = success
                                                  ? String.Format("Module {0} has been {1}.",
                                                                  moduleName,
                                                                  (moduleInstallOrUpdateReport.Action ==
                                                                   CreateOrUpdateActions.Updated
                                                                       ? "updated"
                                                                       : "installed"))
                                                  : "An error occured."
                                };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Edit(string moduleName)
        {
            var module = _reflectionContentManager.LoadModule(moduleName);

            var viewModel = new EditModuleViewModel();

            if (module != null)
            {
                viewModel.Name = module.Name;
                viewModel.Version = module.Version;
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EditModuleViewModel viewModel)
        {
            //TODO: Validation, etc.

            var module = new Module(viewModel.Name, viewModel.Version);

            _reflectionContentManager.Store(module);
            _reflectionContentManager.SaveChanges();

            return Redirect("/admin/modules");
        }

        //[GET("admin/module")]
        [HttpGet]
        public ActionResult Module(string moduleName)
        {
            var viewModel = new ModuleViewModel
                                {
                                    Module = _reflectionContentManager.LoadModule(moduleName)
                                };

            return View(viewModel);
        }
    }
}