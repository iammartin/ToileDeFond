using System;
using System.Collections.Generic;

namespace ToileDeFond.ContentManagement
{
    //TODO: Tout mettre en metatdata et permettre de versionné le metadata afin de pouvoir publié tout comme un contenu (à la sitecore) ?
    public partial class ContentType
    {
        public abstract class ContentTypePropertyBase<T> : EntityWithMetadata<T> where T : Entity<T>, IContentTypeProperty
        {
            #region ctor

            protected ContentTypePropertyBase(ContentType contentType)
            {
                _contentType = contentType;
            }

            protected ContentTypePropertyBase(Guid id, ContentType contentType, IDictionary<string, string> metadata = null)
                : base(id, metadata)
            {
                _contentType = contentType;
            }

            #endregion

            private readonly ContentType _contentType;

            public ContentType ContentType { get { return _contentType; } }

            public string FullName { get { return ContentType.FullName + "." + Name; } }

            //public abstract Type Type { get; }

            public abstract bool IsOwnProperty { get; }

            public abstract string Name { get; }

            //Rendu a tout serializez en string dès l'entrée  - donc on remplace tous les dynamic par des string

            //TODO: Changing the DefaultValue has a lot of impact... peut-etre mieux de pas permettre de modifier aussi facilement (comme IsCultureInvariant) !?
            public string SerializedDefaultValue
            {
                get { return GetSerializedMetadata("DefaultValue"); }
                set { SetOrOverrideSerializedMetadata("DefaultValue", value); }
            }

            public bool IsCultureInvariant
            {
                get { return GetMetadataOrDefault<bool>("IsCultureInvariant"); }
                protected set { SetOrOverrideMetadata("IsCultureInvariant", value); }
            }

            public TType GetDefaultValue<TType>()
            {
                return GetMetadataOrDefault<TType>("DefaultValue");
            }

            public object GetDefaultValue(Type type)
            {
                return GetMetadataOrDefault("DefaultValue", type);
            }

            public void SetDefaultValue(object value)
            {
                SetOrOverrideMetadata("DefaultValue", value);
            }
        }
    }
}
