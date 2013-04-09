using System;
using System.Globalization;
using ToileDeFond.ContentManagement;
using Raven.Client;

namespace ToileDeFond.Localization.RavenDB
{
    public class Localizer : ILocalizer
    {
        private readonly IContentManager _contentManager;
        private readonly IDocumentStore _documentStore;

        public Localizer(IContentManager contentManager, IDocumentStore documentStore)
        {
            _contentManager = contentManager;
            _documentStore = documentStore;
        }

        public string GetLocalizedString(string resourneName, string groupeName = "global", CultureInfo culture = null)
        {
            //Content content;

            //culture = culture ?? CultureInfo.GetCultureInfo("fr-ca");

            //using (var session = _documentStore.OpenSession())
            //{
            //    content = session.Advanced.LuceneQuery<Content, LocalizationResourceIndex>()
            //        .WhereEquals("Name", resourneName)
            //        .WhereEquals("GroupName", groupeName).SingleOrDefault();
            //}

            //if (content == null)
            //    return null;

            //return content.HasTranslation(culture) ? content[culture]["SerializedValue"].GetPublishedValue<string>() : null;

            throw new NotImplementedException();
        }
    }
}
