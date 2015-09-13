using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Vendors;
using Mise.VendorManagement.Services;
using Mise.VendorManagement.Services.Implementation;
using MiseReporting.Models;

namespace MiseReporting.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private readonly EntityDataTransportObjectFactory _dtoFactory;
        private readonly IInventoryExportService _inventoryExportService;
        private readonly string poType;
        public PurchaseOrderController()
        {
            _dtoFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _inventoryExportService = new InventoryCSVExportService(new DummyLogger());
            poType = typeof (PurchaseOrder).ToString();
        }

        // GET: PurchaseOrder
        public async Task<ActionResult> Index(Guid restaurantId)
        {
            //get the purchase orders, but display them per vendor
            var empType = typeof (Employee).ToString();
            var vendorType = typeof (Vendor).ToString();
            var roType = typeof (ReceivingOrder).ToString();
            var restType = typeof(Restaurant).ToString();
            IRestaurant rest;
            var vms = new List<PurchaseOrderViewModel>();
            using (var db = new AzureNonTypedEntities())
            {

                var restAi = await db.AzureEntityStorages
                        .FirstOrDefaultAsync(ai => ai.EntityID == restaurantId && ai.MiseEntityType == restType);

                if (restAi == null)
                {
                    throw new ArgumentException("Error, cannot find restaurant " + restaurantId);
                }

                rest = _dtoFactory.FromDataStorageObject<Restaurant>(restAi.ToRestaurantDTO());

                var poAIs = await db.AzureEntityStorages.Where(ai =>
                    ai.MiseEntityType == poType && ai.RestaurantID.HasValue &&
                    ai.RestaurantID.Value == restaurantId).ToListAsync();

                var pos = poAIs.Select(GetFromAi);

                foreach (var po in pos)
                {
                    //get the employee
                    IEmployee emp = null;
                    var empAi = await db.AzureEntityStorages.FirstOrDefaultAsync(
                                ai => ai.EntityID == po.CreatedByEmployeeID && ai.MiseEntityType == empType);
                    if (empAi != null)
                    {
                        emp = _dtoFactory.FromDataStorageObject<Employee>(empAi.ToRestaurantDTO());
                    }

                    //get each vendor's po
                    foreach (var poForVendor in po.GetPurchaseOrderPerVendors())
                    {
                        //get the vendor
                        IVendor vendor = null;
                        var vendorAi =
                            await
                                db.AzureEntityStorages.FirstOrDefaultAsync(
                                    ai => ai.EntityID == poForVendor.VendorID && ai.MiseEntityType == vendorType);
                        if (vendorAi != null)
                        {
                            vendor = _dtoFactory.FromDataStorageObject<Vendor>(vendorAi.ToRestaurantDTO());
                        }

                        //do we have a RO linked to this?
                        //TODO lookup ROs when we have strong typed data!

                        var poVM = new PurchaseOrderViewModel(poForVendor, vendor, emp, null);
                        vms.Add(poVM);
                    }
                }
            }

            var vm = new RestaurantViewModel(rest, vms);

            return View(vm);
        }

        // GET: PurchaseOrder/Details/fjdjsk-ekd
        public async Task<ActionResult> Details(Guid poID, Guid poForVendorId)
        {
            AzureEntityStorage poAi;
            using (var db = new AzureNonTypedEntities())
            {
                poAi =
                    await
                        db.AzureEntityStorages.FirstOrDefaultAsync(
                            ai => ai.EntityID == poID && ai.MiseEntityType == poType);
            }
            if (poAi == null)
            {
                throw new ArgumentException("No po of poID " + poID + " found");
            }
            var po = GetFromAi(poAi);

            var forVendor = po.GetPurchaseOrderPerVendors().FirstOrDefault(poV => poV.ID == poForVendorId);
            if (forVendor == null)
            {
                throw new ArgumentException("Can't find specific vendor PO for " + poForVendorId);
            }

            var vms = forVendor.GetLineItems().Select(li => new PurchaseOrderLineItemViewModel(li));

            return View(vms);
        }

        /*
        // GET: PurchaseOrder/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PurchaseOrder/Create
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

        // GET: PurchaseOrder/Edit/5
        public ActionResult Edit(int poID)
        {
            return View();
        }

        // POST: PurchaseOrder/Edit/5
        [HttpPost]
        public ActionResult Edit(int poID, FormCollection collection)
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

        // GET: PurchaseOrder/Delete/5
        public ActionResult Delete(int poID)
        {
            return View();
        }

        // POST: PurchaseOrder/Delete/5
        [HttpPost]
        public ActionResult Delete(int poID, FormCollection collection)
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

        private IPurchaseOrder GetFromAi(AzureEntityStorage ai)
        {
            var dto = ai.ToRestaurantDTO();
            var ro = _dtoFactory.FromDataStorageObject<PurchaseOrder>(dto);
            return ro;
        }
    }
}
