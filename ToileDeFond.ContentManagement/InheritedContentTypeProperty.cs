using System;
using System.Collections.Generic;
using ToileDeFond.ContentManagement.DataTransfer;

namespace ToileDeFond.ContentManagement
{
    public partial class ContentType
    {
        public class InheritedContentTypeProperty : ContentTypePropertyBase<InheritedContentTypeProperty>, IContentTypeProperty
        {
            private readonly IContentTypeProperty _parentProperty;

            protected internal InheritedContentTypeProperty(IContentTypeProperty parentProperty, ContentType contentType)
                : base(parentProperty.Id, contentType)
            {
                _parentProperty = parentProperty;
            }

            protected internal InheritedContentTypeProperty(InheritedContentTypePropertyDto inheritedContentTypePropertyDto, IContentTypeProperty parentProperty, ContentType contentType)
                : base(parentProperty.Id, contentType, inheritedContentTypePropertyDto.Metadata)
            {
                _parentProperty = parentProperty;
            }

            public override string Name
            {
                get { return ParentProperty.Name; }
            }

            public override bool IsOwnProperty
            {
                get { return false; }
            }

            public ContentTypeProperty GetOriginalContentTypeProperty()
            {
                var originalContentTypeProperty = _parentProperty;

                while (originalContentTypeProperty.ParentProperty != null)
                {
                    originalContentTypeProperty = originalContentTypeProperty.ParentProperty;
                }

                return (ContentTypeProperty)originalContentTypeProperty;
            }

            public IContentTypeProperty ParentProperty
            {
                get { return _parentProperty; }
            }

            public override bool TryGetMetadata<TType>(string name, out TType metadata)
            {
                var found = TryGetOwnMetadata(name, out metadata);

                if (!found && ParentProperty != null)
                {
                    found = ParentProperty.TryGetMetadata(name, out metadata);
                }

                return found;
            }

            public override bool TryGetMetadata(string name, Type type, out object metadata)
            {
                var found = TryGetOwnMetadata(name, type, out metadata);

                if (!found && ParentProperty != null)
                {
                    found = ParentProperty.TryGetMetadata(name, type, out metadata);
                }

                return found;
            }

            public override bool TryGetSerializedMetadata(string name, out string metadata)
            {
                var found = TryGetOwnSerializedMetadata(name, out metadata);

                if (!found && ParentProperty != null)
                {
                    found = ParentProperty.TryGetSerializedMetadata(name, out metadata);
                }

                return found;
            }

            public InheritedContentTypePropertyDto ToDto()
            {
                return new InheritedContentTypePropertyDto
                {
                    Id = ParentProperty.Id,
                    Metadata = new Dictionary<string, string>(Metadata)
                };
            }
        }
    }
}