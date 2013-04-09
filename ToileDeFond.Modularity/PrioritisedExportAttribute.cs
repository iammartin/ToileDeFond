using System;
using System.ComponentModel.Composition;

namespace ToileDeFond.Modularity
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class PrioritisedExportAttribute : ExportAttribute, IPrioritisedMefMetaData
    {
        public PrioritisedExportAttribute(Type type, int priority = 0)
            : base(type)
        {
            Priority = priority;
        }

        public int Priority { get; set; }
    }
}