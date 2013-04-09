using System.Web.Mvc;

namespace ToileDeFond.Website.Security.App_Start
{
    public class ModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return GetType().Assembly.GetName().Name; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            var moduleName = AreaName;

            context.MapRoute(
                moduleName + "default-route",
                moduleName + "/{controller}/{action}/{id}",
                new
                {
                    area = moduleName,
                    controller = "SecurityController",
                    action = "Login",
                    id = UrlParameter.Optional,
                    module = true
                },
                new[] { string.Format("{0}.Controllers.*", moduleName) }
            );
        }
    }
}