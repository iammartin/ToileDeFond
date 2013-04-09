using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Http;
using ToileDeFond.ContentManagement;

namespace ToileDeFond.Web.API
{
    public class ModulesController : ApiController
    {
        private readonly IContentManager _contentManager;

        [ImportingConstructor]
        public ModulesController(IContentManager contentManager )
        {
            _contentManager = contentManager;
        }

        public IEnumerable<Module> GetAll()
        {
            return _contentManager.LoadAllModules();
        }
    }
}