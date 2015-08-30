﻿using System;
using System.Collections.Generic;
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
    public class InventoriesController : Controller
    {
        private readonly EntityDataTransportObjectFactory _dtoFactory;
        private readonly IInventoryExportService _inventoryExportService;
        public InventoriesController()
        {
            _dtoFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _inventoryExportService = new InventoryCSVExportService(new DummyLogger());
        }

        // GET: Inventories
        public ActionResult Index(Guid restaurantId)
        {
            RestaurantViewModel vm;
            using (var db = new AzureNonTypedEntities())
            {
                var invType = typeof (Inventory).ToString();
                var invAIs =
                    db.AzureEntityStorages.Where(
                        ai =>
                            ai.MiseEntityType == invType && ai.RestaurantID.HasValue &&
                            ai.RestaurantID.Value == restaurantId).ToList();

                var invs = invAIs.Select(GetFromAi);

                var empType = typeof (Employee).ToString();
                //get the emp

                var vms = new List<InventoryViewModel>();
                foreach (var inv in invs)
                {
                    IEmployee emp = null;
                    var empAi =
                        db.AzureEntityStorages
                            .FirstOrDefault(ai => ai.MiseEntityType == empType && ai.EntityID == inv.CreatedByEmployeeID);

                    if (empAi != null)
                    {
                        var empDTO = empAi.ToRestaurantDTO();
                        emp = _dtoFactory.FromDataStorageObject<Employee>(empDTO);
                    }

                    vms.Add(new InventoryViewModel(inv, emp));
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

        public ActionResult Details(Guid inventoryId)
        {
            var invType = typeof(Inventory).ToString();
            AzureEntityStorage invAi;
            using (var db = new AzureNonTypedEntities())
            {
                invAi = db.AzureEntityStorages.FirstOrDefault(ai => ai.EntityID == inventoryId && ai.MiseEntityType == invType);

            }
            if (invAi == null)
            {
                throw new ArgumentException("No inventory of id " + inventoryId + " found");
            }
            var inventory = GetFromAi(invAi);

            var vms = inventory.GetBeverageLineItems().Select(li => new InventoryLineItemViewModel(li));

            return View(vms);
        }

        // GET: Inventories/Details/5
        public async Task<FileResult> GenerateRawCSV(Guid id)
        { 
            //get the inventory
            var invType = typeof (Inventory).ToString();
            AzureEntityStorage invAi;
            using (var db = new AzureNonTypedEntities())
            {
                invAi = db.AzureEntityStorages.FirstOrDefault(
                    ai =>
                        ai.MiseEntityType == invType && ai.EntityID == id);

            }
            if (invAi == null)
            {
                throw new ArgumentException("No inventory of id " + id + " found");
            }
            var inv = GetFromAi(invAi);

            if (inv.GetBeverageLineItems().Any() == false)
            {
                //do nothing
                return null;
            }
            //transform inventory to memory stream, then to file
            var bytes =  await _inventoryExportService.ExportInventoryToCsvBySection(inv);
            var outputStream = new MemoryStream(bytes);
            return new FileStreamResult(outputStream, "text/csv") {FileDownloadName = "inventoryBySections.csv"};
        }

        // GET: Inventories/Details/5
        public async Task<FileResult> GenerateAggregatedCSV(Guid id)
        {
            //get the inventory
            var invType = typeof(Inventory).ToString();
            AzureEntityStorage invAi;
            using (var db = new AzureNonTypedEntities())
            {
                invAi = db.AzureEntityStorages.FirstOrDefault(
                    ai =>
                        ai.MiseEntityType == invType && ai.EntityID == id);

            }
            if (invAi == null)
            {
                throw new ArgumentException("No inventory of id " + id + " found");
            }
            var inv = GetFromAi(invAi);

            if (inv.GetBeverageLineItems().Any() == false)
            {
                //do nothing
                return null;
            }
            //transform inventory to memory stream, then to file
            var bytes = await _inventoryExportService.ExportInventoryToCSVAggregated(inv);
            var outputStream = new MemoryStream(bytes);
            return new FileStreamResult(outputStream, "text/csv") { FileDownloadName = "RestaurantInventory.csv" };
        }

        private IInventory GetFromAi(AzureEntityStorage ai)
        {
            var dto = ai.ToRestaurantDTO();
            var inv = _dtoFactory.FromDataStorageObject<Inventory>(dto);
            return inv;
        }
    }
}
