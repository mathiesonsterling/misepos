using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiseReporting.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Restaurant management for users of Stockboy";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Mise";

            return View();
        }
    }
}