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
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Vendors;
using Mise.VendorManagement.Services;
using Mise.VendorManagement.Services.Implementation;
using MiseReporting.Models;

namespace MiseReporting.Controllers
{
    public class ReceivingOrderController : Controller
    {
        private readonly EntityDataTransportObjectFactory _dtoFactory;
        private readonly ICSVExportService _icsvExportService;

        public ReceivingOrderController()
        {
            _dtoFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _icsvExportService = new CSVExportService(new DummyLogger());
        }

        // GET: ReceivingOrder
        public async Task<ActionResult> Index(Guid restaurantId)
        {
            RestaurantViewModel vm;
            using (var db = new AzureNonTypedEntities())
            {
                var restType = typeof(Restaurant).ToString();
                var restAi = await
                    db.AzureEntityStorages
                        .FirstOrDefaultAsync(ai => ai.EntityID == restaurantId && ai.MiseEntityType == restType);

                if (restAi == null)
                {
                    throw new ArgumentException("Error, cannot find restaurant " + restaurantId);
                }

                var rest = _dtoFactory.FromDataStorageObject<Restaurant>(restAi.ToRestaurantDTO());

                var roType = typeof(ReceivingOrder).ToString();
                var parAIs = await
                    db.AzureEntityStorages.Where(
                        ai =>
                            ai.MiseEntityType == roType && ai.RestaurantID.HasValue &&
                            ai.RestaurantID.Value == restaurantId).ToListAsync();

                var ros = parAIs.Select(GetFromAi);

                var empType = typeof(Employee).ToString();

                var vms = new List<ReceivingOrderViewModel>();
                foreach (var ro in ros)
                {
                    IEmployee emp = null;
                    var empAi = await
                        db.AzureEntityStorages
                            .FirstOrDefaultAsync(ai => ai.MiseEntityType == empType && ai.EntityID == ro.ReceivedByEmployeeID);

                    if (empAi != null)
                    {
                        var empDTO = empAi.ToRestaurantDTO();
                        emp = _dtoFactory.FromDataStorageObject<Employee>(empDTO);
                    }

                    var vendor = await GetVendorForRo(db, ro);
                    vms.Add(new ReceivingOrderViewModel(ro, vendor, emp));
                }

                vm = new RestaurantViewModel(rest, vms.OrderByDescending(inv => inv.DateCreated));
            }

            return View(vm);
        }

        // GET: ReceivingOrder/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            var roType = typeof(ReceivingOrder).ToString();
            AzureEntityStorage roAi;
            using (var db = new AzureNonTypedEntities())
            {
                roAi = await db.AzureEntityStorages.FirstOrDefaultAsync(ai => ai.EntityID == id && ai.MiseEntityType == roType);
            }
            if (roAi == null)
            {
                throw new ArgumentException("No RecevingOrder of id " + id + " found");
            }
            var ro = GetFromAi(roAi);

            var vms = ro.GetBeverageLineItems().Select(li => new ReceivingOrderLineItemViewModel(li));

            return View(vms);
        }

        public async Task<FileResult> GenerateCSV(Guid roId)
        {
            //get the inventory
            var roType = typeof(ReceivingOrder).ToString();
            AzureEntityStorage roAi;
            using (var db = new AzureNonTypedEntities())
            {
                roAi = await db.AzureEntityStorages.FirstOrDefaultAsync(
                    ai =>
                        ai.MiseEntityType == roType && ai.EntityID == roId);
            }
            if (roAi == null)
            {
                throw new ArgumentException("No receiving order of id " + roId + " found");
            }
            var ro = GetFromAi(roAi);

            if (ro.GetBeverageLineItems().Any() == false)
            {
                //do nothing
                return null;
            }

            var fileName = "ReceivingOrder.csv";
            using (var db = new AzureNonTypedEntities())
            {
                var vendor = await GetVendorForRo(db, ro);
                if (vendor != null)
                {
                    fileName = vendor.Name + ".csv";
                }
            }

            //transform inventory to memory stream, then to file
            var bytes = await _icsvExportService.ExportReceivingOrderToCSV(ro);
            var outputStream = new MemoryStream(bytes);
            return new FileStreamResult(outputStream, "text/csv") { FileDownloadName = fileName };
        }

        private IReceivingOrder GetFromAi(AzureEntityStorage ai)
        {
            var dto = ai.ToRestaurantDTO();
            var ro = _dtoFactory.FromDataStorageObject<ReceivingOrder>(dto);
            return ro;
        }

        private async Task<IVendor> GetVendorForRo(AzureNonTypedEntities db, IReceivingOrder ro)
        {
            var vendType = typeof(Vendor).ToString();
            IVendor vendor = null;
            var vendAi = await
                db.AzureEntityStorages.FirstOrDefaultAsync(
                    ai => ai.MiseEntityType == vendType && ai.EntityID == ro.VendorID);
            if (vendAi != null)
            {
                vendor = _dtoFactory.FromDataStorageObject<Vendor>(vendAi.ToRestaurantDTO());
            }
            return vendor;
        }
    }
}
