using System;

namespace ToileDeFond.ContentManagement.Reflection
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CultureAttribute : Attribute
    {
    }
}