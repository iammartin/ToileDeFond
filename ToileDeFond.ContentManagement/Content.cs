using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using ToileDeFond.Modularity;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement
{
    [JsonConverter(typeof(ContentJsonConverter))]
    public partial class Content : Entity<Content>
    {
        #region Data

        public ContentType ContentType { get; protected set; }

        private readonly IList<ContentProperty> _internalProperties;

        private readonly IDictionary<CultureInfo, ContentTranslation> _translations;

        protected readonly List<ContentTranslationVersion> PreviousVersions;

        public IEnumerable<ContentTranslationVersion> GetVersions()
        {
            return PreviousVersions.Union(GetCurrentContentTranslationVersions(creationDate: DateTime.MaxValue))
                .NotNull().Select(x => x);
        }

        #endregion

        #region ctors

        public Content(ContentType type, CultureInfo defaultCulture)
        {
            if (Equals(defaultCulture, CultureInfo.InvariantCulture))
                throw new ArgumentException("Cannot create content with invariant culture as default culture. Create culture invariant content instead.");

            PreviousVersions = new List<ContentTranslationVersion>();
            _translations = new Dictionary<CultureInfo, ContentTranslation>();
            _internalProperties = new List<ContentProperty>();
            ContentType = type;

            foreach (var property in type.CultureInvariantProperties)
            {
                _internalProperties.Add(new ContentProperty(this, property, CultureInfo.InvariantCulture));
            }

            Translate(defaultCulture);
        }

        public Content(Guid id, Guid contentTypeId, IEnumerable<ContentTranslationVersion> contentTranslationVersions, Module module)
            : base(id)
        {
            _internalProperties = new List<ContentProperty>();
            ContentType = module.ContentTypes.Single(t => t.Id.Equals(contentTypeId));

            var translationVersions = contentTranslationVersions as List<ContentTranslationVersion> ?? contentTranslationVersions.ToList();
            var currentVersions = translationVersions.Where(v => v.Publication == null && v.CreationDate == DateTime.MaxValue).ToList();
            translationVersions.RemoveAll(currentVersions.Contains);

            PreviousVersions = translationVersions;

            _translations = currentVersions.Select(t => new ContentTranslation(t.Culture, this)).ToDictionary(t => t.Culture, t => t);

            foreach (var property in ContentType.Properties)
            {
                if (property.IsCultureInvariant)
                {
                    var currentVersion = currentVersions.First();

                    AddContentProperty(currentVersion, property, CultureInfo.InvariantCulture);
                }
                else
                {
                    foreach (var translation in _translations)
                    {
                        var currentTranslationVersion = currentVersions.FirstOrDefault(v => v.Culture.Equals(translation.Key));

                        AddContentProperty(currentTranslationVersion, property, translation.Key);
                    }
                }
            }
        }

        #endregion

        #region Public

        public IEnumerable<Publication> Publications
        {
            get { return PreviousVersions.Select(pv => pv.Publication).NotNull().Distinct(); }
        }

        public Module Module
        {
            get { return ContentType.Module; }
        }

        public Guid ContentTypeId { get { return ContentType.Id; } }

        public ContentTranslation this[CultureInfo culture]
        {
            get
            {
                if (!HasTranslation(culture))
                    throw new InvalidOperationException("The " + culture.EnglishName + " translation of this content does not exist.");

                return _translations[culture];
            }
        }

        public virtual bool HasPublishedVersion
        {
            get { return PreviousVersions.Any(p => p.Publication != null); }
        }

        public bool TryGetTranslation(CultureInfo culture, out ContentTranslation contentTranslation)
        {
            return _translations.TryGetValue(culture, out contentTranslation);
        }

        public ContentTranslation Translate(CultureInfo culture)
        {
            ContentTranslation translation;
            if (!TryGetTranslation(culture, out translation))
            {
                translation = Translate(culture, ContentType.CultureVariantProperties.Select(property => new ContentProperty(this, property, culture)));
            }

            return translation;
        }

        public bool HasTranslation(CultureInfo culture)
        {
            return _translations.ContainsKey(culture);
        }

        public bool HasPropertyNamed(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("The parameter name cannot be empty.");

            return ContentType.HasPropertyNamed(name);
        }

        public ContentTranslation GetExistingOrNewTranslation(Content content, CultureInfo cultureInfo)
        {
            return content.HasTranslation(cultureInfo) ? content[cultureInfo] : content.Translate(cultureInfo);
        }

        public IEnumerable<ContentTranslation> Translations { get { return _translations.Select(t => t.Value); } }

        //Returns true if any modification detected
        public bool CreateTranslationVersions(Publication publication = null)
        {
            //On va chercher toutes les versions (pour chacune des langues) selon l'état actuel
            var contentTranslationVersions = GetCurrentContentTranslationVersions(publication);

            //TODO: Valider que la détection de changement fonctionne bien dans tous les cas
            //On ajoute les versions qui ont des changements 

            var changes = 0;

            foreach (var contentTranslationVersion in contentTranslationVersions)
            {
                if (!contentTranslationVersion.Equals(GetVersion(contentTranslationVersion.Culture,
                                                                published: publication != null)))
                {
                    PreviousVersions.Add(contentTranslationVersion);
                    changes++;
                }
            }

            return changes > 0;
        }

        public ContentTranslationVersion GetVersion(CultureInfo culture, DateTime? dateTime = null, bool published = false)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            dateTime = dateTime ?? DependencyResolver.Current.GetService<IDateTimeManager>().Now();
            ContentTranslationVersion contentTranslationVersion = null;

            if (published)
            {
                //if (!HasPublishedVersion)
                //    throw new InvalidOperationException("The content has not been published yet.");

                var publicatedVersions = PreviousVersions.Where(v => v.Publication != null).ToList();
                if (publicatedVersions.Any())
                {
                    var latestPublication = publicatedVersions.OrderByDescending(v => v.CreationDate)
                        .Select(v => v.Publication).First();

                    if (dateTime <= latestPublication.EndingDate)
                    {
                        contentTranslationVersion = PreviousVersions.OrderByDescending(p => p.CreationDate).FirstOrDefault(v =>
                            v.Culture.Equals(culture) && v.Publication != null &&
                            v.Publication.StartingDate <= dateTime && v.Publication.EndingDate >= dateTime);
                    }
                }
            }
            else
            {
                //Ici le CreationDate représente la fin de vie d'une version draft
                contentTranslationVersion = GetVersions().OrderBy(v => v.CreationDate).FirstOrDefault(v =>
                   v.Culture.Equals(culture) && v.Publication == null && v.CreationDate > dateTime.Value);
            }

            return contentTranslationVersion;
        }

        #region Pour accéder aux propriétés Culture Invariant

        public bool TryGetProperty(IContentTypeProperty property, out ContentProperty contentProperty)
        {
            return TryGetProperty(property.Name, out contentProperty);
        }

        public bool TryGetProperty(string name, out ContentProperty contentProperty)
        {
            contentProperty = this[name];

            return contentProperty == null;
        }

        public ContentProperty this[IContentTypeProperty property]
        {
            get { return this[property.Name]; }
        }

        public ContentProperty this[string name]
        {
            get
            {
                name = name.SubstringAfterLastIndexOf('.');

                return _internalProperties.Where(p => p.ContentTypeProperty.IsCultureInvariant).First(prop => prop.ContentTypeProperty.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }

        #endregion

        #endregion

        #region Private

        private ContentTranslation Translate(CultureInfo culture, IEnumerable<ContentProperty> cultureVariantProperties)
        {
            if (Equals(culture, CultureInfo.InvariantCulture))
                throw new ArgumentException("Cannot create translation for invariant culture.");

            if (HasTranslation(culture))
                throw new InvalidOperationException("The " + culture.EnglishName +
                                                    " translation of this content already exists.");

            var newTranslation = new ContentTranslation(this, culture, cultureVariantProperties);

            _translations.Add(culture, newTranslation);

            return newTranslation;
        }

        private List<ContentTranslationVersion> GetCurrentContentTranslationVersions(Publication publication = null,
           DateTime? creationDate = null)
        {
            var contentTranslationVersions = new List<ContentTranslationVersion>();
            creationDate = creationDate ?? DependencyResolver.Current.GetService<IDateTimeManager>().Now();

            foreach (var contentTranslation in Translations)
            {
                var contentTranslationVersion = CreateVersion(contentTranslation, publication, creationDate);

                if (contentTranslationVersion != null)
                {
                    contentTranslationVersions.Add(contentTranslationVersion);
                }
            }

            return contentTranslationVersions;
        }

        protected ContentTranslationVersion CreateVersion(ContentTranslation contentTranslation,
            Publication publication = null, DateTime? creationDate = null)
        {
            creationDate = creationDate ?? DependencyResolver.Current.GetService<IDateTimeManager>().Now();

            return new ContentTranslationVersion(Id, creationDate.Value, publication, ContentType, contentTranslation.Culture,
                contentTranslation.Properties.Select(p =>
                    new ContentTranslationVersionProperty(p.ContentTypeProperty.GetOriginalContentTypeProperty(), p.SerializedValue)));
        }

        private void AddContentProperty(ContentTranslationVersion currentVersion, IContentTypeProperty property, CultureInfo culture)
        {
            ContentTranslationVersionProperty currentVersionProperty;
            if (currentVersion.TryGetProperty(property.Name, out currentVersionProperty))
            {
                _internalProperties.Add(new ContentProperty(currentVersionProperty.SerializedValue, culture, this, property));
            }
            else
            {
                _internalProperties.Add(new ContentProperty(this, property, culture));
            }
        }

        #endregion
    }
}