using System.Collections.Generic;
using System.Linq;
using ToileDeFond.Modularity;

namespace ToileDeFond.Tests.Common
{
    public class Starter : StarterBase
    {
        protected override IList<string> GetBinDllFileNames(string binPath)
        {
            var binDllFileNames = base.GetBinDllFileNames(binPath);

            var result = binDllFileNames.Where(f => f.StartsWith("ToileDeFond.")).ToList();

            return result;
        }
    }
}