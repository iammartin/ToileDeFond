using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToileDeFond.Modularity;

namespace ToileDeFond.Tools
{
    public class Starter : StarterBase
    {
        //TODO: Trouver une meilleure solution
        protected override IList<string> GetBinDllFileNames(string binPath)
        {
            var binDllFileNames = base.GetBinDllFileNames(binPath);

            var result = binDllFileNames.Where(f => f.StartsWith("ToileDeFond.")).ToList();

            return result;
        }

        protected override bool IsFilteredOut(KeyValuePair<string, Tuple<FileInfo[], string[]>> keyValuePair)
        {
            return keyValuePair.Key.ToLower().Contains("metadata")
                || keyValuePair.Key.ToLower().Contains("web")
                || keyValuePair.Key.ToLower().Contains("security");
        }
    }
}
