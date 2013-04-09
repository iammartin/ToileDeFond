using System;
using Newtonsoft.Json.Linq;

namespace ToileDeFond.ContentManagement
{
    public partial class ContentProperty
    {
        public class PublishedContentPropertyValue : Entity<PublishedContentPropertyValue>, IComparable<PublishedContentPropertyValue>
        {
            #region Data

            private dynamic _value;
            public object Value
            {
                get { return _value; }
                set
                {
                    _value = value is JToken ? value : JToken.FromObject(value);
                }
            }

            //Pas certain d'aimer ça de serialiser plusieurs fois le meme object (toutes les PublishedContentPropertyValue qui ont la même publication)
            public  Publication Publication { get; protected internal set; }

            protected internal ContentProperty ContentProperty { get;  set; }

            #endregion

            #region ctors

            protected internal PublishedContentPropertyValue(ContentProperty property, object value, Publication publication)
            {
                ContentProperty = property;
                Publication = publication;
                Value = value;
            }

            public PublishedContentPropertyValue(object value, Publication publication, ContentProperty contentProperty)
            {
                ContentProperty = contentProperty;
                Value = value;
                Publication = publication;
            }

            #endregion

            #region Public

            public int CompareTo(PublishedContentPropertyValue other)
            {
                return Publication.CompareTo(other.Publication);
            }

            public T GetValue<T>()
            {
                return _value.ToObject<T>();
            }

            #endregion
        }
    }
}