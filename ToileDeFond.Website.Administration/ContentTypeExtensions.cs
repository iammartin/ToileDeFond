using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToileDeFond.ContentManagement;

namespace ToileDeFond.Website.Administration
{
    public static class ContentTypeExtensions
    {
        public static IContentTypeProperty[] GetGridProperties(this ContentType contentType)
        {
            if(!contentType.Properties.Any())
                return new IContentTypeProperty[]{};

            var gridProperties = contentType.Properties.Where(p => p.Metadata.ContainsKey("DisplayInGrid")).ToList();

            if (gridProperties.Any())
            {
                return gridProperties.ToArray();
            }

            var nameProperty = contentType.Properties.FirstOrDefault(p => p.Name.Equals("Name"));

            if (nameProperty != null)
                 return new[] {nameProperty};

            return new[] {contentType.Properties.First()};
        }
    }
}