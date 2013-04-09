using ToileDeFond.ContentManagement.Reflection;

namespace ToileDeFond.Localization
{
    //[ContentType(ContentTypeId)]
    [ContentType]
    public class LocalizationResource
    {
        //const string ContentTypeId = "bb496e69-0938-49fb-8a8e-1cd7c4ea922d";

        [CultureInvariant]
        public string Name { get; set; }

        [CultureInvariant]
        public string GroupName { get; set; }

        public string Value { get; set; }
    }
}
