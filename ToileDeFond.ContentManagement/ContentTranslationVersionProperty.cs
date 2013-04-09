using System;
using Newtonsoft.Json;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement
{
    public class ContentTranslationVersionProperty : IEquatable<ContentTranslationVersionProperty>
    {
        private readonly string _serializedValue;

        public bool Equals(ContentTranslationVersionProperty other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ContentTypeProperty.Id, other.ContentTypeProperty.Id) && Equals(SerializedValue, other.SerializedValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ContentTranslationVersionProperty)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ContentTypeProperty != null ? ContentTypeProperty.Id.GetHashCode() : 0) * 397) ^
                       (SerializedValue != null ? SerializedValue.GetHashCode() : 0);
            }
        }

        protected ContentTranslationVersionProperty()
        {

        }

        //public ContentTranslationVersionProperty(ContentType.ContentTypeProperty contentTypeProperty, object value)
        //{
        //    ContentTypeProperty = contentTypeProperty;
        //    SerializedValue = value;
        //}

        public ContentTranslationVersionProperty(object value, DenormalizedContentTypePropertyReference contentTypeProperty)
        {
            if (value == null)
                _serializedValue = null;
            else
                _serializedValue = JsonConvert.SerializeObject(value, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc });

            ContentTypeProperty = contentTypeProperty;
        }

        public ContentTranslationVersionProperty(DenormalizedContentTypePropertyReference contentTypeProperty, string serializedValue)
        {
            _serializedValue = serializedValue;
            ContentTypeProperty = contentTypeProperty;
        }

        //public DenormalizedContentTypePropertyReference ContentTypeProperty { get; set; }
        //public object SerializedValue { get; set; } //TODO: Tester rapidement - Cette value est boxed - est-ce que ravendb va bien la storée ???
        public DenormalizedContentTypePropertyReference ContentTypeProperty { get; protected set; }

        public string SerializedValue
        {
            get { return _serializedValue; }
        }

        public T GetValue<T>()
        {
            if (_serializedValue == null)
            {
                return (T)typeof(T).GetDefaultValue();
            }

            return JsonConvert.DeserializeObject<T>(_serializedValue, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc });
        }

        public object GetValue(Type type)
        {
            if (_serializedValue == null)
            {
                return type.GetDefaultValue();
            }

            return JsonConvert.DeserializeObject(_serializedValue, type);
        }
    }
}