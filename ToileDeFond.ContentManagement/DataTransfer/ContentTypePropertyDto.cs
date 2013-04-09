using System;
using System.Collections.Generic;

namespace ToileDeFond.ContentManagement.DataTransfer
{
    public class ContentTypePropertyDto
    {
        public Guid Id { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public string Name { get; set; }
    }
}