using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using MefContrib.Hosting.Conventions;
using MefContrib.Hosting.Conventions.Configuration;
using MefContrib.Web.Mvc;

namespace ToileDeFond.Modularity.Web
{
    public class WebApiControllerPartRegistry : PartRegistry
    {
        public WebApiControllerPartRegistry(Assembly assembly)
        {
            Scan(x => x.Assembly(assembly));

            Part()
                .ForTypesAssignableFrom<IHttpController>()
                .MakeNonShared()
                .AddMetadata(new PartCreationScopeAttribute(PartCreationScope.Default))
                .ExportAs<IHttpController>()
                .Export()
                .Imports(x =>
                    {
                        x.Import().Members(
                            m => new[] { m.GetConstructors().FirstOrDefault(c => c.GetCustomAttributes(typeof(ImportingConstructorAttribute), false).Length > 0) ?? m.GetGreediestConstructor() });
                        x.Import().Members(
                            m => m.GetMembers().Where(mbr => mbr.GetCustomAttributes(typeof(ImportAttribute), false).Length > 0).ToArray());
                    });
        }
    }
}