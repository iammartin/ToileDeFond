using System;

namespace ToileDeFond.ContentManagement.ModuleBuilding
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class StrongAggregationPropertyAttribute : Attribute
    {
    }
}