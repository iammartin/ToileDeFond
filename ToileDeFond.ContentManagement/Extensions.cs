using System;

namespace ToileDeFond.ContentManagement
{
    public static class Extensions
    {
        public static bool IsContent(this Type type)
        {
            return type != null && type == typeof(Content);
        }

        public static bool IsContentTranslation(this Type type)
        {
            return type != null && type == typeof(Content.ContentTranslation);
        }
    }
}