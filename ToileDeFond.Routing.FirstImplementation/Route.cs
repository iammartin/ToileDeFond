using System;
using System.Globalization;
using ToileDeFond.ContentManagement;
using ToileDeFond.ContentManagement.Reflection;

namespace ToileDeFond.Routing.FirstImplementation
{
    [ContentType]
    public class Route : ContentTranslationVersion, IRoute
    {
        private string _rewriteFromUrl;

        public Route()
        {
            AnyHttpMethod = true;
        }

        public string RewriteToUrl { get;  set; }

        public bool AnyHttpMethod { get; set; }

        public string RewriteFromUrl
        {
            get { return _rewriteFromUrl; }
            set
            {
                if(value == null)
                    throw new Exception("Route should not have null RewriteFromUrl.");

                _rewriteFromUrl = value.TrimStart(new[] {'/'}).ToLower();
            }
        }

        public CultureInfo Culture
        {
            get { return CultureInfo.GetCultureInfo(CultureName); }
            set { CultureName = value.Name; }
        }
    }
}
