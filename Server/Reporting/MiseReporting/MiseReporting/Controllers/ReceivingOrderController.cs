using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        private readonly IInventoryExportService _inventoryExportService;

        public ReceivingOrderController()
        {
            _dtoFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _inventoryExportService = new InventoryCSVExportService(new DummyLogger());
        }

        // GET: ReceivingOrder
        public ActionResult Index(Guid restaurantId)
        {
            RestaurantViewModel vm;
            using (var db = new AzureNonTypedEntities())
            {
                var roType = typeof(ReceivingOrder).ToString();
                var parAIs =
                    db.AzureEntityStorages.Where(
                        ai =>
                            ai.MiseEntityType == roType && ai.RestaurantID.HasValue &&
                            ai.RestaurantID.Value == restaurantId).ToList();

                var ros = parAIs.Select(GetFromAi);

                var empType = typeof(Employee).ToString();
                var vendType = typeof (Vendor).ToString();

                var vms = new List<ReceivingOrderViewModel>();
                foreach (var ro in ros)
                {
                    IEmployee emp = null;
                    var empAi =
                        db.AzureEntityStorages
                            .FirstOrDefault(ai => ai.MiseEntityType == empType && ai.EntityID == ro.ReceivedByEmployeeID);

                    if (empAi != null)
                    {
                        var empDTO = empAi.ToRestaurantDTO();
                        emp = _dtoFactory.FromDataStorageObject<Employee>(empDTO);
                    }

                    IVendor vendor = null;
                    var vendAi =
                        db.AzureEntityStorages.FirstOrDefault(
                            ai => ai.MiseEntityType == vendType && ai.EntityID == ro.VendorID);
                    if (vendAi != null)
                    {
                        vendor = _dtoFactory.FromDataStorageObject<Vendor>(vendAi.ToRestaurantDTO());
                    }
                    vms.Add(new ReceivingOrderViewModel(ro, vendor, emp));
                }

                var restType = typeof(Restaurant).ToString();
                var restAi =
                    db.AzureEntityStorages
                        .FirstOrDefault(ai => ai.EntityID == restaurantId && ai.MiseEntityType == restType);

                if (restAi == null)
                {
                    throw new ArgumentException("Error, cannot find restaurant " + restaurantId);
                }

                var rest = _dtoFactory.FromDataStorageObject<Restaurant>(restAi.ToRestaurantDTO());
                vm = new RestaurantViewModel(rest, vms.OrderByDescending(inv => inv.DateCreated));
            }

            return View(vm);
        }

        // GET: ReceivingOrder/Details/5
        public ActionResult Details(Guid id)
        {
            var roType = typeof(ReceivingOrder).ToString();
            AzureEntityStorage roAi;
            using (var db = new AzureNonTypedEntities())
            {
                roAi = db.AzureEntityStorages.FirstOrDefault(ai => ai.EntityID == id && ai.MiseEntityType == roType);
            }
            if (roAi == null)
            {
                throw new ArgumentException("No RecevingOrder of id " + id + " found");
            }
            var ro = GetFromAi(roAi);

            var vms = ro.GetBeverageLineItems().Select(li => new ReceivingOrderLineItemViewModel(li));

            return View(vms);
        }


        private IReceivingOrder GetFromAi(AzureEntityStorage ai)
        {
            var dto = ai.ToRestaurantDTO();
            var ro = _dtoFactory.FromDataStorageObject<ReceivingOrder>(dto);
            return ro;
        }
    }
}
