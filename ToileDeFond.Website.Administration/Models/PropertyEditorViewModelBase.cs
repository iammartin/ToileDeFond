using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ToileDeFond.Website.Administration.Models
{
    public class PropertyEditorViewModelBase
    {
         [Required]
        public string Name { get; set; }

        [UIHint("_DropDownList")]
        [Required]
        public string[] PropertyEditor { get; set; }
        public SelectListItem[] GetPropertyEditorValues { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ContentTypeFullName { get; set; }

        [Required]
        [UIHint("Boolean")]
        public bool IsCultureInvariant { get; set; }
    }
}