using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ToileDeFond.ContentManagement
{
    public partial class Content
    {
        public class ContentTranslationVersion : IEquatable<ContentTranslationVersion>
        {
            private Dictionary<string, ContentTranslationVersionProperty> _propertiesDictionary;
            private List<ContentTranslationVersionProperty> _properties;

            public ContentTranslationVersion()
            {

            }

            public ContentTranslationVersion(Guid contentId, DateTime creationDate, Publication publication,
                DenormalizedContentTypeReference contentType, CultureInfo culture,
                IEnumerable<ContentTranslationVersionProperty> properties)
            {
                Id = contentId;
                CreationDate = creationDate;
                Publication = publication;
                ContentType = contentType;
                Culture = culture;
                Properties = properties.ToList();
            }

            public bool TryGetProperty(string name, out ContentTranslationVersionProperty prop)
            {
                return _propertiesDictionary.TryGetValue(name, out prop);
            }

            public ContentTranslationVersionProperty this[string name]
            {
                get
                {
                    ContentTranslationVersionProperty prop;

                    if (!TryGetProperty(name, out prop))
                        throw new InvalidOperationException("No property named " + name);

                    return prop;
                }
            }

            public Guid Id { get; protected set; }

            public DateTime CreationDate { get; protected set; }

            public Publication Publication { get; protected set; }

            public DenormalizedContentTypeReference ContentType { get; protected set; }

            public CultureInfo Culture { get; set; }

            public List<ContentTranslationVersionProperty> Properties
            {
                get { return _properties; }
                protected set
                {
                    _properties = value;
                    _propertiesDictionary = value.ToDictionary(x => x.ContentTypeProperty.Name, x => x);
                }
            }

            public bool Equals(ContentTranslationVersion other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Id.Equals(other.Id) && Equals(Culture, other.Culture) && Equals(Publication, other.Publication) && Equals(Properties, other.Properties);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ContentTranslationVersion)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = Id.GetHashCode();
                    hashCode = (hashCode * 397) ^ (Culture != null ? Culture.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Publication != null ? Publication.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Properties != null ? Properties.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }
    }
    //TODO: Tous les denormalized reference doivent être mis à jour quand l'objet référencé est modifier
    //Exemple, lorsque le ContentType est renommé

    //TODO: Faire un opt-in pour la serialisation (2 collections de properties, entre autres)
}