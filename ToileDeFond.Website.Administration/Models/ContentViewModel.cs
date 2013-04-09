using System.Globalization;
using ToileDeFond.ContentManagement;
using ToileDeFond.Utilities;

namespace ToileDeFond.Website.Administration.Models
{
    public class ContentViewModel
    {
        public Content.ContentTranslation Content { get; set; }
        public CultureInfo Culture { get; set; }
    }
}