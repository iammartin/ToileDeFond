using System.Threading;
using System.Web;
using ToileDeFond.Utilities;

namespace ToileDeFond.Routing
{
    public abstract class UrlRewriterBase : IUrlRewriter
    {
        protected abstract IRoute GetRoute(HttpApplication httpApplication);

        protected virtual bool BypassUrlRewriter(HttpApplication httpApplication)
        {
            if (!IsAcceptedHttpMethod(httpApplication.Request)) return true;

            //if (IsAjaxRequest(httpApplication.Request)) return true;

            var localPath = GetLocalPath(httpApplication);

            if (IsStaticContent(localPath)) return true;

            //if (IsMvcRoute(localPath)) return true;

            //if (IsAPI(localPath)) return true;

            return false;
        }

        protected virtual string GetLocalPath(HttpApplication httpApplication)
        {
            return httpApplication.Request.Url.LocalPath.ToLower();
        }

        protected virtual bool IsAcceptedHttpMethod(HttpRequest request)
        {
            return request.HttpMethod == "GET" || request.HttpMethod == "POST";
        }

        //protected virtual bool IsAPI(string localPath)
        //{
        //    return localPath.StartsWith("/api/");
        //}

        //protected virtual bool IsMvcRoute(string localPath)
        //{
        //    return localPath.StartsWith("/");
        //}

        protected virtual bool IsAjaxRequest(HttpRequest request)
        {
            return request.IsAjaxRequest();
        }

        protected virtual bool IsStaticContent(string localPath)
        {
            return localPath.StartsWith("/content/");
        }

        protected virtual void SetCurrentCulture(IRoute route)
        {
            Thread.CurrentThread.CurrentCulture = route.Culture;
            Thread.CurrentThread.CurrentUICulture = route.Culture;
        }

        public virtual void UrlRewrite(HttpApplication httpApplication)
        {


            //Console.WriteLine(httpApplication.Request.Url.Scheme);
            //Console.WriteLine(httpApplication.Request.Url.Host);
            //Console.WriteLine(httpApplication.Request.Url.LocalPath);
            //Console.WriteLine(httpApplication.Request.Url.Query);

            if (!BypassUrlRewriter(httpApplication))
            {
                var route = GetRoute(httpApplication);

                //TODO: HttpMethods.Contains
                if (route != null && (route.AnyHttpMethod /*|| route.HttpMethods.Contains(httpApplication.Request.HttpMethod)*/))
                {
                    httpApplication.Context.Items.Add(UrlRewriterHttpModule.UrlRewriterCurrentRouteKey, route);
                    SetCurrentCulture(route);

                    //http://blogs.thesitedoctor.co.uk/tim/2011/02/21/Beware+ContextRewritePath+Does+Not+End+The+Current+Execution+Path.aspx
                    httpApplication.Context.RewritePath(route.RewriteToUrl);
                }
            }

            ////try to get a CMS content page with alias matching current request
            //var contentPage = repo.ContentPages.Query
            //    .Where(x => x.Alias == app.Request.FilePath)
            //    .SingleOrDefault();

            ////if not found, request isn't for a CMS page. abort
            //if (contentPage == null) return;

            ////request is for a CMS page. Add content object to request
            ////and execute generic Controller Action
            //app.Context.Items.Add("contentPage", contentPage);
            //app.Context.RewritePath("~/Cms/ContentPage");
        }
    }
}