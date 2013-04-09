using System.Web.Http;
using System.Web.Mvc;

namespace ToileDeFond.Web.App_Start
{
    /* http://blogs.infosupport.com/asp-net-mvc-4-rc-getting-webapi-and-areas-to-play-nicely/ */
    public class ModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "api"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            var moduleName = AreaName;

            context.Routes.MapHttpRoute(
                moduleName + "default-route",
                moduleName + "/{controller}/{id}",
               new { id = RouteParameter.Optional }/*,
               new[] { string.Format("{0}.API.*", GetType().Assembly.GetName().Name) }*/
            );
        }
    }
}