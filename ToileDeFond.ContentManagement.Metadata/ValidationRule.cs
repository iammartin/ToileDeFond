using ToileDeFond.ContentManagement.Reflection;

namespace ToileDeFond.ContentManagement.Metadata
{
    public class ValidationRule
    {
        [CultureInvariant]
        public string ModelValidatorClassFullName { get; set; }

        [CultureInvariant]
        public string Regex { get; set; }

        public string ErrorMessage { get; set; }
    }
}
