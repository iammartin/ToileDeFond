using System;
using ToileDeFond.ContentManagement;
using ToileDeFond.ContentManagement.Reflection;

namespace ToileDeFond.Website.Administration
{
    [ContentType]
    public class PropertyEditor : ContentTranslationVersion, IPropertyEditor
    {
        [CultureInvariant]
        public string GetRoute { get; set; }

        [CultureInvariant]
        public string PostRoute { get; set; }

        public string Name { get; set; }
    }
}