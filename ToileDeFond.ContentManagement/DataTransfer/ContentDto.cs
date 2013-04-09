using System;
using System.Collections.Generic;

namespace ToileDeFond.ContentManagement.DataTransfer
{
    public class ContentDto
    {
        public Guid Id { get; set; }
        public DenormalizedContentTypeReference ContentType { get; set; }
        public List<dynamic> Versions { get; set; }
    }
}
