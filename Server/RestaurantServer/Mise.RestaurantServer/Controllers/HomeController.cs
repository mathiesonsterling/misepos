using System.Web.Mvc;

namespace Mise.RestaurantServer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Diagnostics()
        {
            var results = new
            {
                Bootstrapper.RestaurantID,
                Logger = TypeOrNull(Bootstrapper.Logger),
                AdminServer = TypeOrNull(Bootstrapper.MiseAdminServer),
                DAL = TypeOrNull(Bootstrapper.RestaurantServerDAL)
            };

            return Json(results);
        }

        private string TypeOrNull(object o)
        {
            return o == null ? "null" : o.GetType().ToString();
        }
    }
}
