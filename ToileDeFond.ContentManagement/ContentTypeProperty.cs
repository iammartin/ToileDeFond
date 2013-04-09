using System;
using System.Collections.Generic;
using ToileDeFond.ContentManagement.DataTransfer;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement
{
    public partial class ContentType
    {
        public class ContentTypeProperty : ContentTypePropertyBase<ContentTypeProperty>, IContentTypeProperty
        {
            #region Data

            private readonly string _name;

            #endregion

            #region ctors

            protected internal ContentTypeProperty(string name, object defaultValue, bool isCultureInvariant, ContentType contentType)
                : base(contentType)
            { 
                if(name.IsNullOrEmpty() || name.Contains("."))
                    throw new NotImplementedException();

                _name = name;
                SetOrOverrideMetadata("DefaultValue", defaultValue);
                IsCultureInvariant = isCultureInvariant;
            }

            protected internal ContentTypeProperty(ContentTypePropertyDto contentTypePropertyDto, ContentType contentType)
                : base(contentTypePropertyDto.Id, contentType, contentTypePropertyDto.Metadata)
            {
                _name = contentTypePropertyDto.Name;
            }

            #endregion

            #region Public

            public override string Name
            {
                get { return _name; }
            }

            public ContentTypePropertyDto ToDto()
            {
                return new ContentTypePropertyDto
                {
                    Id = Id,
                    Name = Name,
                    Metadata = new Dictionary<string, string>(Metadata)
                };
            }

            public override bool IsOwnProperty
            {
                get { return true; }
            }

            public IContentTypeProperty ParentProperty { get { return null; } }

            public ContentTypeProperty GetOriginalContentTypeProperty()
            {
                return this;
            }

            #endregion
        }
    }
}