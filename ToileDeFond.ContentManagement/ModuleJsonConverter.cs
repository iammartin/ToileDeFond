using System;
using ToileDeFond.ContentManagement.DataTransfer;
using Newtonsoft.Json;

namespace ToileDeFond.ContentManagement
{
    //http://blog.maskalik.com/asp-net/json-net-implement-custom-serialization
    public class ModuleJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var module = value as Module;

            serializer.Serialize(writer, module.ToDto());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //var jsonObject = JObject.Load(reader);
            var moduleDto = serializer.Deserialize<ModuleDto>(reader);

            return new Module(moduleDto);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Module) == objectType;
        }
    }
}