using System;

namespace ToileDeFond.ContentManagement.DataTransfer
{
    public class PublishedContentPropertyValueDto
    {
        public Guid Id { get; set; }
        public object Value { get; set; }
        public Guid PublicationId { get; set; }
    }
}
