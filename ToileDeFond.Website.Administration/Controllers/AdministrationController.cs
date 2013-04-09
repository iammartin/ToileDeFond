using System.Web.Mvc;

namespace ToileDeFond.Website.Administration.Controllers
{
    [Authorize]
    public class AdministrationController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}