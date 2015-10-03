using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services.Implementation;
using Mise.VendorManagement.Services;
using Mise.VendorManagement.Services.Implementation;
using MiseReporting.Models;

namespace MiseReporting.Controllers
{
    public class ReceivingOrderController : Controller
    {private readonly ICSVExportService _icsvExportService;

        private readonly ManagementDAL _dal;
        public ReceivingOrderController()
        {
            _dal = new ManagementDAL();
            _icsvExportService = new CSVExportService(new DummyLogger());
        }

        // GET: ReceivingOrder
        public async Task<ActionResult> Index(Guid restaurantId)
        {
            var rest = await _dal.GetRestaurantById(restaurantId);
            var ros = await _dal.GetReceivingOrdersForRestaurant(restaurantId);

            var vms = new List<ReceivingOrderViewModel>();
            foreach (var ro in ros)
            {
                var emp = await _dal.GetEmployeeById(ro.ReceivedByEmployeeID);

                var vendor = await _dal.GetVendorById(ro.VendorID);
                vms.Add(new ReceivingOrderViewModel(ro, vendor, emp));
            }

            var vm = new RestaurantViewModel(rest, vms.OrderByDescending(inv => inv.DateCreated));
            

            return View(vm);
        }

        // GET: ReceivingOrder/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            var ro = await _dal.GetReceivingOrderById(id);

            var vms = ro.GetBeverageLineItems().Select(li => new ReceivingOrderLineItemViewModel(li));

            return View(vms);
        }

        public async Task<FileResult> GenerateCSV(Guid roId)
        {
            var ro = await _dal.GetReceivingOrderById(roId);

            if (ro.GetBeverageLineItems().Any() == false)
            {
                //do nothing
                return null;
            }

            var fileName = "ReceivingOrder.csv";

            var vendor = await _dal.GetVendorById(ro.VendorID);
            if (vendor != null)
            {
                fileName = vendor.Name + ".csv";
            }

            //transform inventory to memory stream, then to file
            var bytes = await _icsvExportService.ExportReceivingOrderToCSV(ro);
            var outputStream = new MemoryStream(bytes);
            return new FileStreamResult(outputStream, "text/csv") { FileDownloadName = fileName };
        }

    }
}
