using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement
{
    public partial class Content
    {
        //TODO: Extraire une classe de base ou une interface entre Content & ContentTranslation
        public class ContentTranslation : Entity<ContentTranslation>
        {
            #region Data

            public Content Content { get; protected internal set; }

            public CultureInfo Culture { get; private set; }

            #endregion

            #region ctors

            protected internal ContentTranslation(Content content, CultureInfo culture, IEnumerable<ContentProperty> properties)
            {
                var enumeratedProperties = properties.ToList();

                foreach (var property in enumeratedProperties)
                {
                    property.Content = content;
                }

                Content = content;
                Culture = culture;

                for (var i = 0; i < enumeratedProperties.Count(); i++)
                {
                    Content._internalProperties.Add(enumeratedProperties.ElementAt(i));
                }
            }

            public ContentTranslation(CultureInfo culture, Content content)
            {
                Culture = culture;
                Content = content;
            }

            #endregion

            #region Public

            public ContentType ContentType { get { return Content.ContentType; } }

            public string ContentTypeFullName { get { return Content.ContentType.FullName; } }

            public IEnumerable<ContentProperty> Properties
            {
                get { return Content._internalProperties.Where(p => p.Culture.Equals(CultureInfo.InvariantCulture) || p.Culture.Equals(Culture)); }
            }

            public bool TryGetProperty(IContentTypeProperty property, out ContentProperty contentProperty)
            {
                return TryGetProperty(property.Name, out contentProperty);
            }

            public bool TryGetProperty(string name, out ContentProperty contentProperty)
            {
                contentProperty = this[name];

                return contentProperty == null;
            }

            public ContentProperty this[IContentTypeProperty property]
            {
                get { return this[property.Name]; }
            }

            public ContentProperty this[string name]
            {
                get
                {
                    name = name.SubstringAfterLastIndexOf('.');

                    return Properties.First(prop => prop.ContentTypeProperty.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }
            }

            public bool CreateVersion(Publication publication = null)
            {
                var contentTranslationVersion = Content.CreateVersion(this, publication);

                return contentTranslationVersion != null;
            }

            #endregion
        }
    }
}