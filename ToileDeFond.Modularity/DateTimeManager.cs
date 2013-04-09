using System;
using System.ComponentModel.Composition;

namespace ToileDeFond.Modularity
{
    [PrioritisedExport(typeof (IDateTimeManager))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DateTimeManager : IDateTimeManager
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}