using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using MiseVendorManagement.Models;

namespace MiseVendorManagement.Controllers
{
    public class VendorItemForSaleViewModelController : Controller
    {
        private readonly VendorDAL _dal;
        private readonly ICategoriesService _categoriesService;
        private readonly EntityDataTransportObjectFactory _dtoFactory;
        public VendorItemForSaleViewModelController()
        {
            _dal = new VendorDAL();
            _categoriesService = new CategoriesService();
            _dtoFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
        }

        // GET: VendorItemForSaleViewModel
        public async Task<ActionResult> Index(Guid vendorId)
        {
            var vendor = await _dal.GetVendor(vendorId);
            var vm = new VendorViewModel(vendor);

            return View(vm);
        }

        // GET: VendorItemForSaleViewModel/Details/5
        public async Task<ActionResult> Details(Guid vendorId, Guid lineItemId)
        {
            var vendor = await _dal.GetVendor(vendorId);
            var lineItem = vendor.GetItemsVendorSells().FirstOrDefault(li => li.Id == lineItemId);
            var vm = new VendorItemForSaleViewModel(lineItem);
            return View(vm);
        }

        // GET: VendorItemForSaleViewModel/Create
        public ActionResult Create(Guid vendorId)
        {
            //todo check the vendor exists!
            var newVm = new VendorItemForSaleViewModel
            {
                VendorId = vendorId,
                PossibleCategories = GetPossibleCategories(),
                CaseSize = 1,
                ContainerSizeML = 750M
            };
            return View(newVm);
        }


        // POST: VendorItemForSaleViewModel/Create
        [HttpPost]
        public async Task<ActionResult> Create(Guid vendorId, FormCollection collection)
        {
            try
            {
                var lineItem = GetLineItemFromFormCollection(vendorId, collection, Guid.NewGuid());

                var v = await _dal.GetVendor(vendorId);
                var vendor = v as Vendor;
                vendor.VendorBeverageLineItems.Add(lineItem);

                //save it
                await _dal.UpdateVendor(vendor);

                return RedirectToAction("Index", new RouteValueDictionary { {"vendorId", vendorId} });
            }
            catch(Exception e)
            {
                return View();
            }
        }

        // GET: VendorItemForSaleViewModel/Edit/5
        public async Task<ActionResult> Edit(Guid vendorId, Guid lineItemId)
        {
            var vendor = await _dal.GetVendor(vendorId);
            var lineItem = vendor.GetItemsVendorSells().FirstOrDefault(li => li.Id == lineItemId);

            var vm = new VendorItemForSaleViewModel(lineItem)
            {
                PossibleCategories = GetPossibleCategories()
            };
            return View(vm);
        }

        // POST: VendorItemForSaleViewModel/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(Guid vendorId, Guid Id, FormCollection collection)
        {
            try
            {

                var newLineItem = GetLineItemFromFormCollection(vendorId, collection, Id);
                await _dal.UpdateVendorLineItem(newLineItem);
                
                //assign categories
                return RedirectToAction("Index", new RouteValueDictionary { {"vendorId", vendorId} });
            }
            catch
            {
                return View();
            }
        }

        // GET: VendorItemForSaleViewModel/Delete/5
        public async Task<ActionResult> Delete(Guid vendorId, Guid lineItemId)
        {
            var vendor = await _dal.GetVendor(vendorId);
            var li = vendor.GetItemsVendorSells().FirstOrDefault(l => l.Id == lineItemId);
            var vm = new VendorItemForSaleViewModel(li);

            return View(vm);
        }

        // POST: VendorItemForSaleViewModel/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(Guid vendorId, Guid lineItemId, FormCollection collection)
        {
            try
            {
                var vendor = await _dal.GetVendor(vendorId);
                var downCast = vendor as Vendor;
                var lineItem = downCast.VendorBeverageLineItems.FirstOrDefault(li => li.Id == lineItemId);

                downCast.VendorBeverageLineItems.Remove(lineItem);
                await _dal.UpdateVendor(downCast);

                return RedirectToAction("Index", new RouteValueDictionary { {"vendorId", vendorId} });
            }
            catch
            {
                return View();
            }
        }

        private static int _orderId = 0;
        /// <summary>
        /// TODO change this to event at some time?
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        private VendorBeverageLineItem GetLineItemFromFormCollection(Guid vendorId, FormCollection fc, Guid lineItemId)
        {
            var cats = fc["PostedCategoryGuids"].Split(',');
            var guids = cats.Where(s => string.IsNullOrWhiteSpace(s) == false).Select(Guid.Parse).ToList();
            var actualCats = _categoriesService.GetAssignableCategories().Where(c => guids.Contains(c.Id)).Cast<ItemCategory>().ToList();

            var amtInMl = decimal.Parse(fc["ContainerSizeML"]);
            var container = new LiquidContainer {AmountContained = new LiquidAmount {Milliliters = amtInMl} };

            Money price = null;
            var priceV = fc["PublicPrice"];
            if (string.IsNullOrWhiteSpace(priceV) == false)
            {
                price = new Money(decimal.Parse(priceV));
            }

            return new VendorBeverageLineItem
            {
                CaseSize = int.Parse(fc["CaseSize"]),
                Categories = actualCats,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                Id = lineItemId,
                VendorID = vendorId,
                DisplayName = fc["Name"],
                MiseName = fc["Name"],
                UPC = fc["UPC"],
                Container = container,
                PublicPricePerUnit = price,
                LastTimePriceSet = DateTimeOffset.UtcNow,
                Revision = new EventID(MiseAppTypes.VendorManagement, _orderId++)
            };
        }

        private IEnumerable<ICategory> GetPossibleCategories()
        {
            return _categoriesService.GetAssignableCategories().OrderBy(c => c.Name);
        }
    }
}
