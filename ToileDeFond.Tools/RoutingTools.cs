using System;
using System.Collections.Generic;
using System.Globalization;
using ToileDeFond.ContentManagement;
using ToileDeFond.Modularity;
using ToileDeFond.Routing;
using ToileDeFond.Routing.FirstImplementation;
using NUnit.Framework;

namespace ToileDeFond.Tools
{
    public class RoutingTools
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            new Starter().Start();
        }

        [Test]
        [Ignore]
        public void AddDefaultRoutes()
        {
            DeleteAllRoutes();

            var routes = new List<Route>{
                new Route{
                    RewriteFromUrl = "/",
                    RewriteToUrl = "/ToileDeFond.Website.CurriculumVitae/CurriculumVitae/Index",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "/contactez-moi",
                    RewriteToUrl = "/ToileDeFond.Website.Contact/Contact/Contact",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "/login",
                    RewriteToUrl = "/ToileDeFond.Website.Security/Security/Login",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "register",
                    RewriteToUrl = "/ToileDeFond.Website.Security/Security/Register",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "/admin",
                    RewriteToUrl = "/ToileDeFond.Website.Administration/Administration/Index",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "/admin/modules",
                    RewriteToUrl = "/ToileDeFond.Website.Administration/Module/Modules",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "/admin/module",
                    RewriteToUrl = "/ToileDeFond.Website.Administration/Module/Module",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "/admin/contenttype",
                    RewriteToUrl = "/ToileDeFond.Website.Administration/ContentType/ContentType",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "/admin/contents",
                    RewriteToUrl = "/ToileDeFond.Website.Administration/Content/Contents",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "/admin/content",
                    RewriteToUrl = "/ToileDeFond.Website.Administration/Content/Content",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "/admin/edit-module",
                    RewriteToUrl = "/ToileDeFond.Website.Administration/Module/Edit",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "/admin/edit-content-type",
                    RewriteToUrl = "/ToileDeFond.Website.Administration/ContentType/Edit",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "/admin/edit-content-type-property",
                    RewriteToUrl = "/ToileDeFond.Website.Administration/ContentTypeProperty/Edit",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "/admin/delete-content",
                    RewriteToUrl = "/ToileDeFond.Website.Administration/Content/Delete",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                },
                new Route{
                    RewriteFromUrl = "/admin/delete-content-type-property",
                    RewriteToUrl = "/ToileDeFond.Website.Administration/ContentTypeProperty/Delete",
                    Culture = CultureInfo.GetCultureInfo("fr-CA")
                }
            };
            

            var publication =  new Publication();
            using (var routeRepository = DependencyResolver.Current.GetService<IRouteRepository>())
            {
                foreach (var route in routes)
                {
                    routeRepository.AddRoute(route, publication);
                }

                routeRepository.SaveChanges();
            }
        }

        [Test]
        [Ignore]
        public void DeleteAllRoutes()
        {
            using (var routeRepository = DependencyResolver.Current.GetService<IRouteRepository>())
            {
                routeRepository.DeleteAllRoutes();
            }
        }
    }
}


//namespace ToileDeFond.Tools
//{
//    public class RoutingTools
//    {
//        private IDocumentStore _documentStore;

//        [TestFixtureSetUp]
//        public void TestFixtureSetUp()
//        {
//            _documentStore = RavenDBUtilities.CreateNewDocumentStoreInitializeAndCreateUtilIndexes("http://localhost:8081", "ToileDeFond");

//            using (var moduleManager = new ModuleManager(GetReflectionContentManager()))
//            {
//                moduleManager.InstallOrUpdateModules(new List<Assembly> { typeof(Route).Assembly });
//            }
//        }

//        [Test]
//        [Ignore]
//        public void AddRoute()
//        {
//            var route = new Route
//                            {
//                                RewriteFromUrl = "/",
//                                RewriteToUrl = "/ToileDeFond.Website.CurriculumVitae/CurriculumVitae/Index",
//                                Culture = CultureInfo.GetCultureInfo("fr-CA")
//                            };

//            using (var reflectionContentManager = GetReflectionContentManager())
//            {
//                var contentReport = reflectionContentManager.GetNewOrUpdatedContent(route);
//                reflectionContentManager.Store(contentReport.Item);
//                reflectionContentManager.SaveChanges();
//            }
//        }

//        [Test]
//        [Ignore]
//        public void AddDefaultRoutes()
//        {
//            DeleteAllRoutes();

//            var routes = new List<Route>{
//                new Route{
//                    RewriteFromUrl = "/",
//                    RewriteToUrl = "/ToileDeFond.Website.CurriculumVitae/CurriculumVitae/Index",
//                    Culture = CultureInfo.GetCultureInfo("fr-CA")
//                },
//                new Route{
//                    RewriteFromUrl = "/contactez-moi",
//                    RewriteToUrl = "/ToileDeFond.Website.Contact/Contact/Contact",
//                    Culture = CultureInfo.GetCultureInfo("fr-CA")
//                },
//                new Route{
//                    RewriteFromUrl = "/login",
//                    RewriteToUrl = "/ToileDeFond.Website.Security/Security/Login",
//                    Culture = CultureInfo.GetCultureInfo("fr-CA")
//                },
//                new Route{
//                    RewriteFromUrl = "register",
//                    RewriteToUrl = "/ToileDeFond.Website.Security/Security/Register",
//                    Culture = CultureInfo.GetCultureInfo("fr-CA")
//                },
//                new Route{
//                    RewriteFromUrl = "/admin",
//                    RewriteToUrl = "/ToileDeFond.Website.Administration/Administration/Index",
//                    Culture = CultureInfo.GetCultureInfo("fr-CA")
//                },
//                new Route{
//                    RewriteFromUrl = "/admin/modules",
//                    RewriteToUrl = "/ToileDeFond.Website.Administration/Administration/Modules",
//                    Culture = CultureInfo.GetCultureInfo("fr-CA")
//                },
//                new Route{
//                    RewriteFromUrl = "/admin/module",
//                    RewriteToUrl = "/ToileDeFond.Website.Administration/Administration/Module",
//                    Culture = CultureInfo.GetCultureInfo("fr-CA")
//                },
//                new Route{
//                    RewriteFromUrl = "/admin/contenttype",
//                    RewriteToUrl = "/ToileDeFond.Website.Administration/Administration/ContentType",
//                    Culture = CultureInfo.GetCultureInfo("fr-CA")
//                },
//                new Route{
//                    RewriteFromUrl = "/admin/contents",
//                    RewriteToUrl = "/ToileDeFond.Website.Administration/Administration/Contents",
//                    Culture = CultureInfo.GetCultureInfo("fr-CA")
//                },
//                new Route{
//                    RewriteFromUrl = "/admin/content",
//                    RewriteToUrl = "/ToileDeFond.Website.Administration/Administration/Content",
//                    Culture = CultureInfo.GetCultureInfo("fr-CA")
//                }
//            };

//            using (var reflectionContentManager = GetReflectionContentManager())
//            {
//                foreach (var route in routes)
//                {
//                    var contentReport = reflectionContentManager.GetNewOrUpdatedContent(route);
//                    reflectionContentManager.Store(contentReport.Item);
//                }

//                reflectionContentManager.SaveChanges();
//            }
//        }

//        [Test]
//        [Ignore]
//        public void DeleteAllRoutes()
//        {
//            RavenDBUtilities.WaitForStaleIndexes(_documentStore);

//            _documentStore.DatabaseCommands.DeleteByIndex("DraftRoutesByRewriteFromUrlIndex", new IndexQuery());

//            RavenDBUtilities.WaitForStaleIndexes(_documentStore);
//        }

//        private ReflectionContentManager GetReflectionContentManager()
//        {
//            return new ReflectionContentManager(new ContentManager(_documentStore.OpenSession()), new ReflectionContentManagementBuilder(), new ReflectionContentBuilder(), new CultureManager());
//        }

//        [TestFixtureTearDown]
//        public virtual void TestFixtureTearDown()
//        {
//            _documentStore.Dispose();
//            _documentStore = null;
//        }
//    }
//}
