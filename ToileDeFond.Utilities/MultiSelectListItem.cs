using System.Web.Mvc;

namespace ToileDeFond.Utilities
{
    public class MultiSelectListItem : SelectListItem
    {
        public string GroupKey { get; set; }
        public string GroupName { get; set; }
    }
}