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
        public ActionResult Index(Guid restaurantId)
        {
            var viewModels = new List<InventoryViewModel>
            {
                new InventoryViewModel
                {
                    DateCompleted = DateTime.Now,
                    DoneByEmployee = "Yo Mama",
                    Id = Repository.RepositoryFactory.TestInvGuid
                }
            };
            return View(viewModels);
        }

        // GET: Inventories/Details/5
        public FileResult GenerateCsv(Guid id)
        {
            throw new NotImplementedException("No Csv yet");
        }
    }
}
