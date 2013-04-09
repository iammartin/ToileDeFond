using System.Collections.Generic;
using ToileDeFond.ContentManagement.Reflection;

namespace ToileDeFond.Website.Administration.Models
{
    public class ModulesViewModel
    {
        public List<ModuleInfo> ModuleInfos { get; set; }

        public string Message { get; set; }
    }
}