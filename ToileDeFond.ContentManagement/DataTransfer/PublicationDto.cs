using System;

namespace ToileDeFond.ContentManagement.DataTransfer
{
    public class PublicationDto : EntityDto
    {
        public string EndDate { get; set; }
        public string StartDate { get; set; }
        public string CreationDate { get; set; }
    }
}