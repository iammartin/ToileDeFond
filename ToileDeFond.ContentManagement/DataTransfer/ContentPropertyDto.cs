using System;

namespace ToileDeFond.ContentManagement.DataTransfer
{
    public class ContentPropertyDto : EntityDto
    {
        public object Value { get; set; }

        public Guid ContentTypePropertyId { get; set; }

        public string CultureName { get; set; }
    }
}
