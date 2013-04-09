using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

namespace ToileDeFond.Routing
{
    //TODO: Supporter plus de types sur les ContentType (au pire utiliser la serialization si type complexe)
    public interface IRoute
    {
        string RewriteToUrl { get; }
        CultureInfo Culture { get; }
        //List<string> HttpMethods { get; }
        bool AnyHttpMethod { get; }
        string RewriteFromUrl { get; }
    }
}