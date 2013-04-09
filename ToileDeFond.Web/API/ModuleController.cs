using System;
using System.ComponentModel.Composition;
using System.Web.Http;
using ToileDeFond.ContentManagement;

namespace ToileDeFond.Web.API
{
    /*    http://codebetter.com/glennblock/2012/02/28/why-are-there-2-controllers-in-the-asp-net-web-api-contactmanager-example-rest-has-nothing-to-with-it-2/     */

    public class ModuleController : ApiController
    {

        private readonly IContentManager _contentManager;

        [ImportingConstructor]
        public ModuleController(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public Module Get(Guid id)
        {
            return _contentManager.LoadModule(id);
        }
    }
}
