using System.Web.Mvc;

namespace ToileDeFond.Website.App_Start
{
    //TODO: A quoi ca sert exactement (ca vient avec mvc/webapi) ??
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}