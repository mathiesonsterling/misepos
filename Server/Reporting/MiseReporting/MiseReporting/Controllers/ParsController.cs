using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;
using Mise.VendorManagement.Services;
using Mise.VendorManagement.Services.Implementation;
using MiseReporting.Models;

namespace MiseReporting.Controllers
{
    public class ParsController : Controller
    {
        private readonly EntityDataTransportObjectFactory _dtoFactory;
        private readonly ICSVExportService _icsvExportService;

        public ParsController()
        {
            _dtoFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _icsvExportService = new CSVExportService(new DummyLogger());
        }

        [Authorize]
        // GET: Pars
        public async Task<ActionResult> Index(Guid restaurantId)
        {
            RestaurantViewModel vm;
            using (var db = new AzureNonTypedEntities())
            {
                var parType = typeof(Par).ToString();
                var parAIs = await db.AzureEntityStorages.Where(
                        ai =>
                            ai.MiseEntityType == parType && ai.RestaurantID.HasValue &&
                            ai.RestaurantID.Value == restaurantId).ToListAsync();

                var pars = parAIs.Select(GetFromAi);

                var empType = typeof(Employee).ToString();
                //get the emp

                var vms = new List<ParViewModel>();
                foreach (var par in pars)
                {
                    IEmployee emp = null;
                    var empAi = await
                        db.AzureEntityStorages
                            .FirstOrDefaultAsync(ai => ai.MiseEntityType == empType && ai.EntityID == par.CreatedByEmployeeID);

                    if (empAi != null)
                    {
                        var empDTO = empAi.ToRestaurantDTO();
                        emp = _dtoFactory.FromDataStorageObject<Employee>(empDTO);
                    }

                    vms.Add(new ParViewModel(par, emp));
                }

                var restType = typeof(Restaurant).ToString();
                var restAi = await
                    db.AzureEntityStorages
                        .FirstOrDefaultAsync(ai => ai.EntityID == restaurantId && ai.MiseEntityType == restType);

                if (restAi == null)
                {
                    throw new ArgumentException("Error, cannot find restaurant " + restaurantId);
                }

                var rest = _dtoFactory.FromDataStorageObject<Restaurant>(restAi.ToRestaurantDTO());
                vm = new RestaurantViewModel(rest, vms.OrderByDescending(inv => inv.DateCreated));
            }

            return View(vm);
        }

        private IPar GetFromAi(AzureEntityStorage ai)
        {
            var dto = ai.ToRestaurantDTO();
            var par = _dtoFactory.FromDataStorageObject<Par>(dto);
            return par;
        }

        [Authorize]
        // GET: Pars/Details/5
        public async Task<ActionResult> Details(Guid parId)
        {
            var parType = typeof(Par).ToString();
            AzureEntityStorage parAi;
            using (var db = new AzureNonTypedEntities())
            {
                parAi = await db.AzureEntityStorages.FirstOrDefaultAsync(ai => ai.EntityID == parId && ai.MiseEntityType == parType);

            }
            if (parAi == null)
            {
                throw new ArgumentException("No par of id " + parId + " found");
            }
            var par = GetFromAi(parAi);

            var vms = par.GetBeverageLineItems().Select(li => new ParLineItemViewModel(li));

            return View(vms);
        }

        [Authorize]
        public async Task<FileResult> GenerateCSV(Guid parId)
        {
            //get the inventory
            var parType = typeof(Par).ToString();
            AzureEntityStorage parAi;
            using (var db = new AzureNonTypedEntities())
            {
                parAi = await db.AzureEntityStorages.FirstOrDefaultAsync(
                    ai =>
                        ai.MiseEntityType == parType && ai.EntityID == parId);

            }
            if (parAi == null)
            {
                throw new ArgumentException("No par of id " + parId + " found");
            }
            var par = GetFromAi(parAi);

            if (par.GetBeverageLineItems().Any() == false)
            {
                //do nothing
                return null;
            }
            //transform inventory to memory stream, then to file
            var bytes = await _icsvExportService.ExportParToCSV(par);
            var outputStream = new MemoryStream(bytes);
            return new FileStreamResult(outputStream, "text/csv") { FileDownloadName = "Par.csv" };
        }
        /*
        // GET: Pars/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pars/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Pars/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Pars/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Pars/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Pars/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }*/
    }
}
