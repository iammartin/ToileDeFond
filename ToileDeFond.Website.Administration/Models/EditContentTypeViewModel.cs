using System.Web.Mvc;

namespace ToileDeFond.Website.Administration.Models
{
    public class EditContentTypeViewModel
    {
        public string Name { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ModuleName { get; set; }
    }
}