using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToileDeFond.ContentManagement.DataTransfer;
using Raven.Imports.Newtonsoft.Json;

namespace ToileDeFond.ContentManagement.RavenDB
{
    public class PublicationJsonConverter : JsonConverter
    {
        public PublicationJsonConverter()
        {

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType() == typeof(PublicationDto))
            {
                serializer.Converters.Remove(this);

                serializer.Serialize(writer, value);

                serializer.Converters.Add(this);
            }
            else
                serializer.Serialize(writer, ((Publication)value).ToDto());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(Publication))
                throw new NotImplementedException();

            serializer.Converters.Remove(this);

            var m = new Publication(serializer.Deserialize<PublicationDto>(reader));

            serializer.Converters.Add(this);

            return m;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Publication) == objectType || typeof(PublicationDto) == objectType;
        }
    }
}
