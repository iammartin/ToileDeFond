using System;

namespace ToileDeFond.ContentManagement.Reflection
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CultureInvariantAttribute : Attribute
    {
    }
}
