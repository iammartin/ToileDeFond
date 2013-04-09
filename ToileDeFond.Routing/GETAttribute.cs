using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ToileDeFond.Routing
{
    public class RouteGETAttribute : AttributeRouting.Web.Mvc.GETAttribute
    {
        public RouteGETAttribute(string routeUrl) : base(routeUrl)
        {
        }
      
        public override bool IsValidForRequest(System.Web.Mvc.ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
        {
            var isValideForRequest = base.IsValidForRequest(controllerContext, methodInfo);

            //if(isValideForRequest && !HttpContext.Current.Items.Contains(UrlRewriter.UrlRewriterCurrentRouteKey))
            //    throw new HttpException(404, "HTTP/1.1 404 Not Found");

            //return isValideForRequest;

            return isValideForRequest && HttpContext.Current.Items.Contains(UrlRewriterHttpModule.UrlRewriterCurrentRouteKey);
        }
    }
}
