using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Newtonsoft.Json;
using ToileDeFond.Modularity.Web;
using ToileDeFond.Website;
using ToileDeFond.Website.App_Start;
using System.Linq;
using DependencyResolver = System.Web.Mvc.DependencyResolver;

[assembly: WebActivator.PreApplicationStartMethod(typeof(MvcApplication), "Start")]

namespace ToileDeFond.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void Start()
        {
            new Starter().Start();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteTable.Routes.IgnoreRoute("Content/");
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            //TODO: Déplacer... c'est pas beau ici ;-)
            foreach (var bundleConfig in DependencyResolver.Current.GetServices<IBundleConfig>())
            {
                bundleConfig.RegisterBundles(BundleTable.Bundles);
            }

            RemoveWebApiXmlFormatter();
        }

        private static void RemoveWebApiXmlFormatter()
        {
            var formatters = GlobalConfiguration.Configuration.Formatters;
            formatters.JsonFormatter.SerializerSettings.Converters.Add(new ContentJsonConverter());
            formatters.JsonFormatter.SerializerSettings.Converters.Add(new ModuleJsonConverter());

            formatters.Remove(formatters.XmlFormatter);
        }
    }

    //TODO
    internal class ModuleJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }

    //TODO
    internal class ContentJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }

    public class Starter : WebStarterBase
    {
        //TODO: Trouver une meilleure solution
        protected override IList<string> GetBinDllFileNames(string binPath)
        {
            var binDllFileNames = base.GetBinDllFileNames(binPath);

            var result = binDllFileNames.Where(f => f.StartsWith("ToileDeFond.")).ToList();

            return result;
        }
    }
}