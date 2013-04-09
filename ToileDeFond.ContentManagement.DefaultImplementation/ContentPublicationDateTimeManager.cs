using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ToileDeFond.Modularity;

namespace ToileDeFond.ContentManagement.DefaultImplementation
{
    //TODO: Revoir l'implementation
    [PrioritisedExport(typeof(IContentPublicationDateTimeManager), 1)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ContentPublicationDateTimeManager : IContentPublicationDateTimeManager
    {
        private readonly IContentPublicationStateManager _contentPublicationStateManager;
        private readonly IDateTimeManager _dateTimeManager;

   
        public ContentPublicationDateTimeManager(IContentPublicationStateManager contentPublicationStateManager, 
            IDateTimeManager dateTimeManager)
        {
            _contentPublicationStateManager = contentPublicationStateManager;
            _dateTimeManager = dateTimeManager;
        }

        [ImportingConstructor]
        public ContentPublicationDateTimeManager([ImportMany]IEnumerable<Lazy<IContentPublicationStateManager, IPrioritisedMefMetaData>> contentPublicationStateManagers,
            IDateTimeManager dateTimeManager)
            : this(contentPublicationStateManagers.OrderByDescending(x => x.Metadata.Priority).First().Value, dateTimeManager)
        {
            
        }

        public DateTime? GetContentPublicationDateTime()
        {
            if (_contentPublicationStateManager.ContentPublicationStateIsDraft())
                return null;

            return _dateTimeManager.Now()/*.ToUniversalTime()*/;
        }
    }
}