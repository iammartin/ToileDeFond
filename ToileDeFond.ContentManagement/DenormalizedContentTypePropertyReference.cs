using System;

namespace ToileDeFond.ContentManagement
{
    public class DenormalizedContentTypePropertyReference
    {
        protected DenormalizedContentTypePropertyReference()
        {

        }

        public DenormalizedContentTypePropertyReference(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static implicit operator DenormalizedContentTypePropertyReference(ContentType.ContentTypeProperty contentTypeProperty)
        {
            return new DenormalizedContentTypePropertyReference
                (
                contentTypeProperty.Id,
                contentTypeProperty.Name
                );
        }

        public string Name { get; protected set; }

        public Guid Id { get; protected set; }
    }
}