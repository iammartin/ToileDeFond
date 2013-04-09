using System;
using System.Collections.Generic;

namespace ToileDeFond.ContentManagement.DataTransfer
{
    public class InheritedContentTypePropertyDto
    {
        public Guid Id { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}