using System;
using System.Collections.Generic;
using System.Linq;

namespace ToileDeFond.ContentManagement.DataTransfer
{
    public class ModuleDto
    {
        public Guid Id { get; set; }

        public string Version { get; set; }

        public string Name { get;  set; }

        public List<ContentTypeDto> ContentTypes { get; set; }

        public ContentTypeDto this[string name] { get { return ContentTypes.FirstOrDefault(t => t.Name.Equals(name)); } }
    }
}