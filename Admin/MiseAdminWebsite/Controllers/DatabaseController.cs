using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiseAdminWebsite.Controllers
{
    public class DatabaseController : Controller
    {
        public ActionResult Database()
        {
            return View();
        }
        // GET: Database
        public ActionResult ResetDev()
        {
            return View();
        }

        public ActionResult ResetQA()
        {
            throw new NotImplementedException();
        }

        public ActionResult ResetProduction()
        {
            throw new NotImplementedException();
        }
    }
}