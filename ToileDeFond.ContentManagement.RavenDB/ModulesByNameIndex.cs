using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Indexes;

namespace ToileDeFond.ContentManagement.RavenDB
{
    public class ModulesByNameIndex : AbstractIndexCreationTask<Module>
    {
        public ModulesByNameIndex()
        {
            Map = modules => from module in modules
                             select new { module.Name };
        }
    }
}
