using System;

namespace ToileDeFond.ContentManagement.Reflection
{
   public class ContentTranslationVersion
    {
       public string Id { get; set; }
       public string ContentType { get; set; }
       public string ContentTypeId { get; set; }
       public string ModuleId { get; set; }
       public string CultureName { get; set; }
       public DateTime? PublicationCreationDate { get; set; }
       public DateTime? PublicationStartingDate { get; set; }
       public DateTime? PublicationEndingDate { get; set; }
    }
}
