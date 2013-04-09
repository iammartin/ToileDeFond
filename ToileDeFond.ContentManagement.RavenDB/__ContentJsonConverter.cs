using System;
using System.Linq;
using ToileDeFond.ContentManagement.DataTransfer;
using Raven.Imports.Newtonsoft.Json;

namespace ToileDeFond.ContentManagement.RavenDB
{
    public class __ContentJsonConverter : JsonConverter
    {
        private readonly IContentManager _contentManager;

        public __ContentJsonConverter(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType() == typeof(ContentDto))
            {
                serializer.Converters.Remove(this);

                serializer.Serialize(writer, value);

                serializer.Converters.Add(this);
            }
            else
            {
                serializer.Serialize(writer, ((Content)value).ToDto());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(Content))
                throw new NotImplementedException();

            serializer.Converters.Remove(this);

            var contentDto = serializer.Deserialize<ContentDto>(reader);


            var module = _contentManager.LoadModule(contentDto.ModuleId);
            var publicationIds = contentDto.Properties.SelectMany(p => p.PublishedValues.Select(y => y.PublicationId)).Distinct().ToList();
            var publications = _contentManager.LoadPublications(publicationIds);


            var m = new Content(contentDto, module, publications.ToArray());

            serializer.Converters.Add(this);

            return m;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Content) == objectType || typeof(ContentDto) == objectType;
        }
    }

    public class ModuleJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType() == typeof(ModuleDto))
            {
                serializer.Converters.Remove(this);
                serializer.Serialize(writer, value);
                serializer.Converters.Add(this);
            }
            else
            {
                serializer.Serialize(writer, ((Module)value).ToDto());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(Module))
                throw new NotImplementedException();

            serializer.Converters.Remove(this);

            var m = new Module(serializer.Deserialize<ModuleDto>(reader));

            serializer.Converters.Add(this);

            return m;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Module) == objectType || typeof(ModuleDto) == objectType;
        }
    }
}
