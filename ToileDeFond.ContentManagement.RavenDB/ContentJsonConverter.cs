using System;
using Raven.Imports.Newtonsoft.Json;
using Raven.Json.Linq;

namespace ToileDeFond.ContentManagement.RavenDB
{
    //http://blog.maskalik.com/asp-net/json-net-implement-custom-serialization
    public class ContentJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            RavenJToken.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(value)).WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = RavenJObject.Load(reader).ToString();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Content>(json);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Content) == objectType;
        }
    }
}
