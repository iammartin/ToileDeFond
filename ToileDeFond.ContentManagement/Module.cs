using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToileDeFond.ContentManagement.DataTransfer;

namespace ToileDeFond.ContentManagement
{
    [JsonConverter(typeof(ModuleJsonConverter))]
    public class Module : Entity<Module>
    {
        #region Data

        public string Version { get; set; }

        public string Name { get; private set; }

        private readonly Dictionary<string, ContentType> _contentTypes;

        #endregion

        #region Public

        public IEnumerable<ContentType> ContentTypes
        {
            get { return _contentTypes.Select(c => c.Value); }
        }

        //TODO: Faire un tryget pour ne pas lancer d'exception - comme la collection interne Dictionary
        public ContentType this[string name]
        {
            get
            {
                return _contentTypes[name];
            }
        }

        public bool TryGetContentType(string name, out ContentType contentType)
        {
           return _contentTypes.TryGetValue(name, out contentType);
        }

        public ContentType AddContentType(string name, ContentType baseContentType = null)
        {
            GuardAgainstDuplicateContentType(name);

            var contentType = new ContentType(name, this, baseContentType /*TODO: ?? Default BaseContentType*/);

            _contentTypes.Add(contentType.Name, contentType);

            return contentType;
        }

        //public ContentType AddContentType(ContentType contentType)
        //{
        //    GuardAgainstDuplicateContentType(contentType.Name);

        //    contentType.Module = this;

        //    _contentTypes.Add(contentType.Name, contentType);

        //    return contentType;
        //}

        public bool HasContentTypeNamed(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("The parameter name cannot be empty.");

            return _contentTypes.ContainsKey(name);
        }

        public ModuleDto ToDto()
        {
            return new ModuleDto
            {
                ContentTypes = ContentTypes.Select(ct => ct.ToDto()).ToList(),
                Id = Id,
                Name = Name,
                Version = Version
            };
        }

        #endregion

        #region ctors

        public Module(string name, string version, Guid? id = null) : base(id)
        {
            Name = name;
            _contentTypes = new Dictionary<string, ContentType>();
            Version = version;
        }

        public Module(ModuleDto moduleDto)
            : base(moduleDto.Id)
        {
            Name = moduleDto.Name;

            var contentTypes = moduleDto.ContentTypes.Select(c => new ContentType(c, this)).ToList();

            foreach (var contentTypeDto in moduleDto.ContentTypes.Where(x => x.BaseContentType.HasValue))
            {
                var contentType = contentTypes.Single(c => c.Id.Equals(contentTypeDto.Id));
                contentType.BaseContentType = contentTypes.Single(c => c.Id.Equals(contentTypeDto.BaseContentType));
            }

            foreach (var contentType in contentTypes.Where(x => x.BaseContentType != null))
            {
                foreach (var ownProperty in contentType.BaseContentType.OwnProperties)
                {
                    contentType._properties.Add(ownProperty.Name, new ContentType.InheritedContentTypeProperty(moduleDto[contentType.Name]
                        .GetInheritedContentTypeProperty(ownProperty.Id), ownProperty, contentType));
                }
            }

            _contentTypes = contentTypes.ToDictionary(c => c.Name, c => c);

            Version = moduleDto.Version;
        }

        #endregion

        #region Private

        private void GuardAgainstDuplicateContentType(string contentTypeName)
        {
            if (HasContentTypeNamed(contentTypeName))
                throw new ArgumentException(String.Format("The module {0} already contains a content type named {1}", Name, contentTypeName));
        }

        #endregion
    }
}