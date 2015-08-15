using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiseReporting.Models;

namespace MiseReporting.Controllers
{
    public class InventoriesController : Controller
    {
        // GET: Inventories
        public ActionResult Index(Guid inventoryId)
        {
            var viewModels = new List<InventoryViewModel>();
            return View(viewModels);
        }

        // GET: Inventories/Details/5
        public FileResult GenerateCsv(Guid id)
        {
            throw new NotImplementedException("No Csv yet");
        }
    }
}
