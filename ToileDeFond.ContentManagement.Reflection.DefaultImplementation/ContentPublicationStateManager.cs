using System.ComponentModel.Composition;
using System.Web;
using ToileDeFond.ContentManagement;
using ToileDeFond.Modularity;

namespace ToileDeFond.Metadata.ContentManagement
{
    //TODO: Revoir l'implementation
    [PrioritisedExport(typeof(IContentPublicationStateManager), 1)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ContentPublicationStateManager : IContentPublicationStateManager
    {
        public bool ContentPublicationStateIsDraft()
        {
            if (HttpContext.Current == null)
                return false;

            var strContentPublicationStateIsDraft = HttpContext.Current.Request["ContentPublicationStateIsDraft"];

            bool contentPublicationStateIsDraft;
            if (strContentPublicationStateIsDraft == null || !bool.TryParse(strContentPublicationStateIsDraft, out contentPublicationStateIsDraft))
                return false;

            return contentPublicationStateIsDraft;
        }
    }
}
