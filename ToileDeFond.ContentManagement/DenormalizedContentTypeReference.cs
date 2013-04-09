using System;

namespace ToileDeFond.ContentManagement
{
    public class DenormalizedContentTypeReference
    {
        protected DenormalizedContentTypeReference()
        {

        }

        public DenormalizedContentTypeReference(Guid id, string name, string fullName, Module module)
        {
            Id = id;
            Name = name;
            FullName = fullName;
            Module = module;
        }

        public static implicit operator DenormalizedContentTypeReference(ContentType contentType)
        {
            return new DenormalizedContentTypeReference(contentType.Id,
                                                        contentType.Name,
                                                        contentType.FullName,
                                                        contentType.Module
                );
        }

        public Guid Id { get; protected set; }

        public string Name { get; protected set; }

        public string FullName { get; protected set; }

        public DenormalizedModuleReference Module { get; protected set; }
    }
}