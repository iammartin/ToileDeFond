using System;
using System.Collections.Generic;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Linq;

namespace ToileDeFond.ContentManagement.RavenDB
{
    public class MetadataConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = value as Dictionary<string, dynamic>;
            writer.WriteStartObject();

            foreach (var x in dict)
            {
                writer.WritePropertyName(x.Key);

                if (x.Value == null)
                    writer.WriteUndefined();
                else
                    ((JToken) x.Value).ToString();
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var result = new Dictionary<string, dynamic>();

            foreach (var x in jsonObject)
            {
                result.Add(x.Key, x.Value);
            }

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Dictionary<string, dynamic>) == objectType;
        }
    }
}