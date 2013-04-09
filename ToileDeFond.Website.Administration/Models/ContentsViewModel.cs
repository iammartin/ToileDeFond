using System.Globalization;
using ToileDeFond.ContentManagement;
using ToileDeFond.Utilities;

namespace ToileDeFond.Website.Administration.Models
{
    public class ContentsViewModel
    {
        public ContentType ContentType { get; set; }
        public PagedCollection<Content> Contents { get; set; }
        public CultureInfo Culture { get; set; }
    }
}