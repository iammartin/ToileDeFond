using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement.Reflection
{
    public static class Extensions
    {
        public static IEnumerable<Type> GetContentTypeTypes(this Assembly assembly)
        {
            return assembly.GetTypes().Where(t => t.IsContentTranslationVersion());
        }

        public static bool IsContentTranslationVersion(this Type type)
        {
            var x = typeof (ContentTranslationVersion);

            return type != x && (type.Inherits(typeof(ContentTranslationVersion)) || 
                !type.IsInterface && type.HasAttribute<ContentTypeAttribute>());
        }
    }
}
