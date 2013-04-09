using System;
using System.Globalization;

namespace ToileDeFond.ContentManagement.Reflection
{
    public interface IContentBuilder
    {
        Guid? GetContentId(object objectContent);
        Content UpdateContent(object objectContent, Content content, CultureInfo cultureInfo);
        CultureInfo GetCultureInfo(object objectContent);
    }
}