using System.Linq;
using Raven.Client.Indexes;

namespace ToileDeFond.ContentManagement.RavenDB
{
    public class ContentsByContentTypeNameIndex : AbstractIndexCreationTask<Content>
    {
        public ContentsByContentTypeNameIndex()
        {
            Map = contents => from content in contents
                           select new
                           {
                               content.ContentType
                           };
        }
    }
}
