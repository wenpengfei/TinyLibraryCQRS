using System.Web.Mvc;

namespace TinyLibraryCQRS.Presentations.Mvc3.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to Tiny Library CQRS - A Demo Application for CQRS Architecture!";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
