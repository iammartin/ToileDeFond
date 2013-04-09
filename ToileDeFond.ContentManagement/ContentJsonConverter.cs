using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ToileDeFond.Modularity;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement
{
    //http://blog.maskalik.com/asp-net/json-net-implement-custom-serialization
    public class ContentJsonConverter : JsonConverter
    {
        //Est-ce qu'on conserve les names (contenttype, module) ou seulement les id
        //si on conserve les names ca veut dire qu'il faudra corriger/updater l'ensemble des noeuds si jamais ils changent...
        //Mais si quelqu'un veut faire une query, c'est pas mal plus facile sur le nom de la propriété (par exemple) que sur son id...
        //Décision finale : on sauvegarde telqu'un objet json normal serait sauvegarder (avec les names) puisque le changement des names est trop important... 
        //on fera les updates nécessaire si la struture d'un type change
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var content = value as Content;
            writer.WriteStartObject();
            WriteJsonContentInfo(writer, value, serializer, content);
            writer.WritePropertyName("GetVersions");
            writer.WriteStartArray();

            foreach (var contentTranslationVersion in content.GetVersions())
            {
                WriteJsonContentTranslationVersion(writer, value, serializer, content, contentTranslationVersion);
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        protected virtual void WriteJsonContentTranslationVersion(JsonWriter writer, object value, JsonSerializer serializer, Content content, Content.ContentTranslationVersion contentTranslationVersion)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("CreationDate");
            serializer.Serialize(writer, contentTranslationVersion.CreationDate);
            WriteJsonContentInfo(writer, value, serializer, content);

            WriteJsonPublicationInfo(writer, value, serializer, contentTranslationVersion.Publication);

            writer.WritePropertyName("CultureName");
            serializer.Serialize(writer, contentTranslationVersion.Culture.Name);
            var propertyDictionary = contentTranslationVersion.Properties.Select(contentTranslationVersionProperty =>
                WriteJsonContentTranslationVersionProperty(writer, value, serializer, content, contentTranslationVersion,
                contentTranslationVersionProperty)).ToDictionary(propertyKeyValue =>
                    propertyKeyValue.Key, propertyKeyValue => propertyKeyValue.Value);


            writer.WritePropertyName("PropertyDictionary");

            writer.WriteStartObject();

            foreach (var propertyKeyValue in propertyDictionary)
            {
                writer.WritePropertyName(propertyKeyValue.Key);
                serializer.Serialize(writer, propertyKeyValue.Value);
            }

            writer.WriteEndObject();

            writer.WriteEndObject();
        }

        protected virtual void WriteJsonPublicationInfo(JsonWriter writer, object value, JsonSerializer serializer, Publication publication)
        {
            writer.WritePropertyName("PublicationCreationDate");
            if (publication == null)
                serializer.Serialize(writer, null);
            else
                serializer.Serialize(writer, publication.CreationDate);

            writer.WritePropertyName("PublicationStartingDate");
            if (publication == null)
                serializer.Serialize(writer, null);
            else
                serializer.Serialize(writer, publication.StartingDate);

            writer.WritePropertyName("PublicationEndingDate");
            if (publication == null)
                serializer.Serialize(writer, null);
            else
                serializer.Serialize(writer, publication.EndingDate);
        }

        //Tres important !!!
        //TODO: http://ayende.com/blog/66563/ravendb-migrations-rolling-updates
        protected virtual KeyValuePair<string, Guid> WriteJsonContentTranslationVersionProperty(JsonWriter writer, object value,
            JsonSerializer serializer, Content content, Content.ContentTranslationVersion contentTranslationVersion,
            ContentTranslationVersionProperty contentTranslationVersionProperty)
        {
            writer.WritePropertyName(contentTranslationVersionProperty.ContentTypeProperty.Name);
            
            if (contentTranslationVersionProperty.SerializedValue == null)
            {
                writer.WriteNull();
            }
            else
            {
                var json = contentTranslationVersionProperty.SerializedValue;
                writer.WriteRawValue(json);
            }

            return new KeyValuePair<string, Guid>(contentTranslationVersionProperty.ContentTypeProperty.Name, contentTranslationVersionProperty.ContentTypeProperty.Id);
        }

        protected virtual void WriteJsonContentInfo(JsonWriter writer, object value, JsonSerializer serializer, Content content)
        {
            //TODO: Ne pas générer l'id nous meme - (Dealing with custom ID for high number of documents) http://ravendb.net/docs/2.0/client-api/basic-operations/saving-new-document
            writer.WritePropertyName("Id");
            serializer.Serialize(writer, content.Id);
            writer.WritePropertyName("ContentType");
            serializer.Serialize(writer, content.ContentType.FullName);
            writer.WritePropertyName("ContentTypeId");
            serializer.Serialize(writer, content.ContentTypeId);
            writer.WritePropertyName("ModuleId");
            serializer.Serialize(writer, content.Module.Id);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            // _documentStore.OpenSession().Advanced.GetMetadataFor()

            JToken serializedContentTypeId;
            if (!jsonObject.TryGetValue("ContentTypeId", out serializedContentTypeId))
            {
                throw new NotImplementedException();
            }

            var contentTypeId = serializedContentTypeId.ToObject<Guid>();

            JToken serializedModuleId;
            if (!jsonObject.TryGetValue("ModuleId", out serializedModuleId))
            {
                throw new NotImplementedException();
            }

            var moduleId = serializedModuleId.ToObject<Guid>();

            JToken serializedVersions;
            if (!jsonObject.TryGetValue("GetVersions", out serializedVersions))
            {
                throw new NotImplementedException();
            }

            var module = DependencyResolver.Current.GetService<IContentManager>().LoadModule(moduleId);
            var versions = serializedVersions.Select(serializedVersion =>
                GetContentTranslationVersion(serializedVersion, module.ContentTypes.First(t =>
                    t.Id.Equals(contentTypeId)))).ToList();

            return new Content(versions.First().Id, contentTypeId, versions, module);
        }

        //TODO: Convertir cette méthode en ContentTranslationVersionJsonConverter (aurait l'avantage de pourvoir etre réutilisé) ?
        private Content.ContentTranslationVersion GetContentTranslationVersion(JToken serializedVersion, ContentType contentType)
        {
            var jsonObject = JObject.Load(serializedVersion.CreateReader());

            Guid? id = null;
            var propertyDictionary = new Dictionary<string, Guid>();
            DateTime? creationDate = null;
            string cultureName = null;
            DateTime? publicationCreationDate = null;
            DateTime? publicationStartingDate = null;
            DateTime? publicationEndingDate = null;
            var properties = new Dictionary<string, string>();

            foreach (var contentTranslationVersionSerializedProperty in jsonObject.Properties())
            {
                switch (contentTranslationVersionSerializedProperty.Name)
                {
                    case "Id":
                        id = contentTranslationVersionSerializedProperty.Value.ToObject<Guid>();
                        break;
                    case "PropertyDictionary":
                        var jsonPropertyDictionaryObject = JObject.Load(contentTranslationVersionSerializedProperty.Value.CreateReader());

                        foreach (var property in jsonPropertyDictionaryObject.Properties())
                        {
                            propertyDictionary.Add(property.Name, property.Value.ToObject<Guid>());
                        }
                        break;
                    case "CreationDate":
                        creationDate = contentTranslationVersionSerializedProperty.Value.ToObject<DateTime>();
                        break;
                    case "CultureName":
                        cultureName = contentTranslationVersionSerializedProperty.Value.ToObject<string>();
                        break;
                    case "PublicationCreationDate":
                        publicationCreationDate = contentTranslationVersionSerializedProperty.Value.ToObject<DateTime?>();
                        break;
                    case "PublicationStartingDate":
                        publicationStartingDate = contentTranslationVersionSerializedProperty.Value.ToObject<DateTime?>();
                        break;
                    case "PublicationEndingDate":
                        publicationEndingDate = contentTranslationVersionSerializedProperty.Value.ToObject<DateTime?>();
                        break;
                    case "ContentType":
                        break;
                    case "ContentTypeId":
                        break;
                    case "ModuleId":
                        break;
                    default:
                        //Pas de deserialisation pour les sous objets (Content)... il sont deserialisés a la demande (GetValue<T> - T étant un Content) comme toutes autres propriétés du contenu
                        var serializedValue = contentTranslationVersionSerializedProperty.ToString().ReplaceFirst("\"" + contentTranslationVersionSerializedProperty.Name + "\": ", "");
                        properties.Add(contentTranslationVersionSerializedProperty.Name, serializedValue);
                        break;
                }
            }

            var contentTranslationVersionProperties = properties.Select(property => new ContentTranslationVersionProperty(new DenormalizedContentTypePropertyReference(propertyDictionary[property.Key], property.Key), property.Value)).ToList();

            return new Content.ContentTranslationVersion(id.Value, creationDate.Value, publicationCreationDate.HasValue ?
                new Publication(publicationStartingDate, publicationEndingDate, publicationCreationDate) :
                null, contentType, new CultureInfo(cultureName), contentTranslationVersionProperties);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Content) == objectType;
        }
    }
}