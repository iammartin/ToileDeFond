using System;
using System.Collections.Generic;
using System.Linq;

namespace ToileDeFond.ContentManagement.DataTransfer
{
    public class ContentTypeDto
    {
        public Guid Id { get; set; }

        public Dictionary<string, string> Metadata { get;  set; }

        public Guid? BaseContentType { get;  set; }

        public string Name { get;  set; }

        public List<ContentTypePropertyDto> OwnProperties { get;  set; }

        public List<InheritedContentTypePropertyDto> InheritedProperties { get; set; }

        public ContentTypePropertyDto this[string name] { get { return OwnProperties.FirstOrDefault(t => t.Name.Equals(name)); } }

        public InheritedContentTypePropertyDto GetInheritedContentTypeProperty(Guid id) { return InheritedProperties.FirstOrDefault(t => t.Id.Equals(id)); }
    }
}