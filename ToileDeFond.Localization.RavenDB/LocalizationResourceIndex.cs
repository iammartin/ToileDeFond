using ToileDeFond.ContentManagement;
using Raven.Client.Indexes;

namespace ToileDeFond.Localization.RavenDB
{
    //TODO: Se fier a l'index pour les routes
    //public class LocalizationResourceIndex : AbstractIndexCreationTask<Content>
    //{
    //    public LocalizationResourceIndex()
    //    {
    //        Map = contents => from content in contents
    //                          from version in content.PreviousVersions
    //                          from translation in version.Translations
    //                          from property in translation.Properties
    //                          where version.PublicationDate == null //Draft
    //                          where property.Name == "Name"
    //                          select new { Id = content.Id, CultureName = translation.CultureName, Name = property.SerializedValue };
    //    }
    //}
}