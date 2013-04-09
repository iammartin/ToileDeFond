using System;
using System.Globalization;
using Newtonsoft.Json;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement
{
    public class ContentProperty
    {
        #region Data

        public IContentTypeProperty ContentTypeProperty { get; private set; }

        public Content Content { get; protected internal set; }

        public CultureInfo Culture { get; private set; }

        public string SerializedValue { get; set; }

        #endregion

        #region ctors

        protected internal ContentProperty(Content content, IContentTypeProperty property, CultureInfo culture)
        {
            ContentTypeProperty = property;
            Culture = culture;
            Content = content;
            SerializedValue = property.SerializedDefaultValue;
        }

        public ContentProperty(CultureInfo culture, object value, Content content, IContentTypeProperty contentTypeProperty)
        {
            Content = content;
            SetValue(value);
            ContentTypeProperty = contentTypeProperty;
            Culture = culture;
        }

        public ContentProperty( string serializedValue,CultureInfo culture, Content content, IContentTypeProperty contentTypeProperty)
        {
            Content = content;
            SerializedValue = serializedValue;
            ContentTypeProperty = contentTypeProperty;
            Culture = culture;
        }

        #endregion

        #region Public

        public T GetValue<T>()
        {
            return (T) GetValue(typeof (T));
        }

        public object GetValue(Type type)
        {
            if (SerializedValue == null)
            {
                return type.GetDefaultValue();
            }

            object value;

            try
            {
                value = JsonConvert.DeserializeObject(SerializedValue, type, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc });
            }
            catch 
            {
                value = type.GetDefaultValue();
            }

            return value;
        }

        public void SetValue(object value)
        {
            SerializedValue = JsonConvert.SerializeObject(value, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc });
        }

        public string Name { get { return ContentTypeProperty.Name; } }

        #endregion
    }
}