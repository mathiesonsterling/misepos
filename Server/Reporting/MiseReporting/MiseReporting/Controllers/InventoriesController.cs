using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.ValueItems;
using Mise.VendorManagement.Services;
using Mise.VendorManagement.Services.Implementation;
using MiseReporting.Models;

namespace MiseReporting.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly ICSVExportService _icsvExportService;
        private readonly ManagementDAL _dal;
        public InventoriesController()
        {
            _icsvExportService = new CSVExportService(new DummyLogger());
            _dal = new ManagementDAL();
        }

        // GET: Inventories
        public async Task<ActionResult> Index(Guid restaurantId)
        {
            var invs = await _dal.GetInventoriesForRestaurant(restaurantId);
            //get the emp

            var vms = new List<InventoryViewModel>();
            foreach (var inv in invs)
            {
                var emp = await _dal.GetEmployeeWhoCreatedInventory(inv);
                vms.Add(new InventoryViewModel(inv, emp));
            }

            var rest = await _dal.GetRestaurantById(restaurantId);
            var vm = new RestaurantViewModel(rest, vms.OrderByDescending(inv => inv.DateCreated));
            
            return View(vm);
        }

        public async Task<ActionResult> Details(Guid inventoryId)
        {
            var inventory = await _dal.GetInventoryById(inventoryId);

            var vms = new List<InventoryLineItemViewModel>();
            foreach (var sec in inventory.GetSections().OrderBy(s => s.Name))
            {
                vms.AddRange(sec.GetInventoryBeverageLineItemsInSection().OrderBy(li => li.DisplayName).Select(li => new InventoryLineItemViewModel(sec.Name, li)));
            }

            var vm = new InventoryViewModel(inventory, null) {LineItems = vms};
            return View(vm);
        }

        public async Task<ActionResult> EditLineItem(Guid inventoryId, Guid lineItemId)
        {
            var li = await _dal.GetInventoryLineItem(inventoryId, lineItemId);
            var vm = new InventoryLineItemViewModel(string.Empty, li);
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> EditLineItem(Guid inventoryId, Guid lineItemId, FormCollection collection)
        {
            try
            {
                decimal price;
                if (decimal.TryParse(collection["PricePaid"], out price))
                {
                    var li = await _dal.GetInventoryLineItem(inventoryId, lineItemId) as InventoryBeverageLineItem;
                    li.PricePaid = new Money(price);
                    li.UPC = collection["UPC"];
                    await _dal.UpdateInventoryLineItem(inventoryId, li);
                    return RedirectToAction("Details", new RouteValueDictionary { { "inventoryId", inventoryId}});
                }
                return View();
            }
            catch (Exception e)
            {
                return View();
            }
        }

        // GET: Inventories/Details/5
        public async Task<FileResult> GenerateRawCSV(Guid id)
        { 
            //get the inventory
            var inv = await _dal.GetInventoryById(id);

            if (inv.GetBeverageLineItems().Any() == false)
            {
                //do nothing
                return null;
            }
            //transform inventory to memory stream, then to file
            var bytes =  await _icsvExportService.ExportInventoryToCsvBySection(inv);
            var outputStream = new MemoryStream(bytes);
            return new FileStreamResult(outputStream, "text/csv") {FileDownloadName = "inventoryBySections.csv"};
        }

        // GET: Inventories/Details/5
        public async Task<FileResult> GenerateAggregatedCSV(Guid id)
        {
            //get the inventory
            var inv = await _dal.GetInventoryById(id);

            if (inv.GetBeverageLineItems().Any() == false)
            {
                //do nothing
                return null;
            }
            //transform inventory to memory stream, then to file
            var bytes = await _icsvExportService.ExportInventoryToCSVAggregated(inv);
            var outputStream = new MemoryStream(bytes);
            return new FileStreamResult(outputStream, "text/csv") { FileDownloadName = "RestaurantInventory.csv" };
        }
    }
}
