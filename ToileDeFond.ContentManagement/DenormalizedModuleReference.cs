using System;

namespace ToileDeFond.ContentManagement
{
    public class DenormalizedModuleReference
    {
        protected DenormalizedModuleReference()
        {

        }

        public DenormalizedModuleReference(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static implicit operator DenormalizedModuleReference(Module module)
        {
            return new DenormalizedModuleReference(module.Id,
                                                   module.Name
                );
        }

        public Guid Id { get; protected set; }

        public string Name { get; protected set; }
    }
}