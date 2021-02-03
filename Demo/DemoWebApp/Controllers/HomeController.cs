using System.Web.Mvc;

namespace DemoWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.HtmlString = "";
            ViewBag.JsonText = "";
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            ViewBag.JsonText = formCollection.Get("txtArea");
            ViewBag.HtmlString = new Json2HtmlTable.Json2Html().GetJson2Html(ViewBag.JsonText);
            return View();
        }
    }
}