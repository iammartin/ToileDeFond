using System.ComponentModel.Composition;
using System.Globalization;
using System.Threading;
using ToileDeFond.Modularity;

namespace ToileDeFond.Localization
{
    [PrioritisedExport(typeof(ICultureManager))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CultureManager : ICultureManager
    {
        public CultureInfo GetDefaultCulture()
        {
            return Thread.CurrentThread.CurrentCulture;
        }
    }
}
