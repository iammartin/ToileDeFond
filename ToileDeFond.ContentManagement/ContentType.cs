using System;
using System.Collections.Generic;
using System.Linq;
using ToileDeFond.ContentManagement.DataTransfer;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement
{
    public partial class ContentType : EntityWithMetadata<ContentType>
    {
        #region Data

        public ContentType BaseContentType { get; protected internal set; }

        public Module Module { get; private set; }

        public string Name { get; private set; }

        protected internal readonly Dictionary<string, IContentTypeProperty> _properties;

        #endregion

        #region ctors

        protected internal ContentType(string name, Module module, ContentType baseContentType = null)
        {
            if (name.IsNullOrEmpty() || name.Contains("."))
                throw new NotImplementedException();

            Name = name;
            Module = module;
            BaseContentType = baseContentType;
            _properties = new Dictionary<string, IContentTypeProperty>();

            if (baseContentType != null)
            {
                foreach (var contentTypeProperty in baseContentType.Properties)
                {
                    _properties.Add(contentTypeProperty.Name, new InheritedContentTypeProperty(contentTypeProperty, this));
                }
            }
        }

        protected internal ContentType(ContentTypeDto contentTypeDto, Module module)
            : base(contentTypeDto.Id, contentTypeDto.Metadata)
        {
            Name = contentTypeDto.Name;
            Module = module;
            _properties = new Dictionary<string, IContentTypeProperty>();

            var ownProperties = contentTypeDto.OwnProperties.Select(p => new ContentTypeProperty(p, this)).ToList();
            ownProperties.ForEach(p => _properties.Add(p.Name, p));
        }

        #endregion

        #region Private 

        private void GuardAgainstDuplicateProperty(ContentTypeProperty property)
        {
            if (HasPropertyNamed(property.Name))
                throw new ArgumentException(String.Format("The content type {0} already contains a property named {1}", Name, property.Name));
        }

        private void GuardAgainstDuplicatePropertyInInheritingContentTypes(ContentTypeProperty property)
        {
            if (InheritingContentTypes.Any(t => t.Name.Equals(property.Name)))
                throw new ArgumentException(String.Format("The content type {0} has inheriting content types that already contains a property named {1}", Name, property.Name));
        }

        protected internal IEnumerable<IContentTypeProperty> CultureInvariantProperties
        {
            get { return _properties.Select(p => p.Value).Where(p => p.IsCultureInvariant); }
        }

        protected internal IEnumerable<IContentTypeProperty> CultureVariantProperties
        {
            get { return _properties.Select(p => p.Value).Where(p => !p.IsCultureInvariant); }
        }

        private ContentTypeProperty AddProperty(ContentTypeProperty property)
        {
            GuardAgainstDuplicateProperty(property);
            GuardAgainstDuplicatePropertyInInheritingContentTypes(property);

            _properties.Add(property.Name, property);

            //Tous les type enfants doivent avoir une InheritedContentTypeProperty
            //Il se peut qu'un type enfant ait déjà une propriété du même nom

            var inheritingContentTypes = InheritingContentTypes.ToList();

            while (inheritingContentTypes.Any())
            {
                inheritingContentTypes.ForEach(c => c.AddInheritedProperty(c.BaseContentType[property.Name]));
                inheritingContentTypes = inheritingContentTypes.SelectMany(t => t.InheritingContentTypes).ToList();
            }

            return property;
        }

        private void AddInheritedProperty(IContentTypeProperty property)
        {
            _properties.Add(property.Name, new InheritedContentTypeProperty(property, this));
        }

        #endregion

        #region Public

        public ContentTypeDto ToDto()
        {
            return new ContentTypeDto
            {
                BaseContentType = BaseContentType == null ? default(Guid?) : BaseContentType.Id,
                Id = Id,
                Metadata = new Dictionary<string, string>(Metadata),
                Name = Name,
                OwnProperties = OwnProperties.Select(p => p.ToDto()).ToList(),
                InheritedProperties = InheritedProperties.Select(p => p.ToDto()).ToList()
            };
        }

        public IEnumerable<ContentTypeProperty> OwnProperties
        {
            get
            {
                return _properties.Select(p => p.Value).Where(p => p.IsOwnProperty).Cast<ContentTypeProperty>();
            }
        }

        public ContentTypeProperty GetContentTypeProperty(string name)
        {
            name = name.SubstringAfterLastIndexOf('.');
            IContentTypeProperty property;

            if (TryGetProperty(name, out property) && property.IsOwnProperty)
            {
                return property as ContentTypeProperty;
            }

            return null;
        }

        public IEnumerable<InheritedContentTypeProperty> InheritedProperties
        {
            get { return _properties.Select(p => p.Value).Where(p => !p.IsOwnProperty).Cast<InheritedContentTypeProperty>(); }
        }

        public string FullName { get { return Module.Name + "." + Name; } }

        public IContentTypeProperty this[string name]
        {
            get
            {
                return _properties[name];
            }
        }

        public bool TryGetProperty(string name, out IContentTypeProperty contentTypeProperty)
        {
            name = name.SubstringAfterLastIndexOf('.');

            return _properties.TryGetValue(name, out contentTypeProperty);
        }

        public bool HasPropertyNamed(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("The parameter name cannot be empty.");

            return _properties.ContainsKey(name);
        }

        public bool CultureInvariant
        {
            get { return CultureVariantProperties.Any(); }
        }

        public ContentTypeProperty AddProperty(string name, object defaultValue, bool isCultureInvariant)
        {
            return AddProperty(new ContentTypeProperty(name, defaultValue, isCultureInvariant, this));
        }

        public ContentTypeProperty AddRelationProperty(string name, bool isCultureInvariant)
        {
           return AddProperty(new ContentTypeProperty(name, null, isCultureInvariant, this));
        }

        public ContentTypeProperty AddRelationsProperty(string name, bool isCultureInvariant)
        {
            return AddProperty(new ContentTypeProperty(name, new List<Guid>(), isCultureInvariant, this));
        }

        public ContentTypeProperty AddAggregationProperty(string name, bool isCultureInvariant)
        {
            return AddProperty(new ContentTypeProperty(name, null, isCultureInvariant, this));
        }

        public ContentTypeProperty AddAggregationsProperty(string name, bool isCultureInvariant)
        {
            return AddProperty(new ContentTypeProperty(name, new List<Content>(), isCultureInvariant, this));
        }

        public IEnumerable<ContentType> InheritingContentTypes
        {
            get { return Module.ContentTypes.Where(t => Equals(t.BaseContentType, this)); }
        }

        public IEnumerable<IContentTypeProperty> Properties { get { return _properties.Select(p => p.Value); } }

        public override bool TryGetMetadata<TType>(string name, out TType metadata)
        {
            var found = TryGetOwnMetadata(name, out metadata);

            if (!found && BaseContentType != null)
            {
                found = BaseContentType.TryGetMetadata(name, out metadata);
            }

            return found;
        }

        public bool RemoveProperty(string name)
        {
            name = name.SubstringAfterLastIndexOf('.');

            var contentTypeProperty = OwnProperties.FirstOrDefault(p => p.Name.Equals(name));

            return contentTypeProperty != null && _properties.Remove(name);
        }

        #endregion
    }
}
