using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation;
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
        public VendorItemForSaleViewModelController()
        {
            _dal = new VendorDAL();
            _categoriesService = new CategoriesService();
        }

        // GET: VendorItemForSaleViewModel
        public async Task<ActionResult> Index(Guid vendorId, string sortOrder)
        {
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.CategorySortParm = sortOrder == "category" ? "category_desc" : "category";
            ViewBag.SizeSortParam = sortOrder == "size" ? "size_desc" : "size";
            var vendor = await _dal.GetVendor(vendorId);
            var vm = new VendorViewModel(vendor);

            switch (sortOrder)
            {
                case "category":
                    vm.ItemsForSale = vm.ItemsForSale.OrderBy(li => li.Categories).ThenBy(li => li.Name);
                    break;
                case "category_desc":
                    vm.ItemsForSale = vm.ItemsForSale.OrderByDescending(li => li.Categories).ThenBy(li => li.Name);
                    break;
                case "container":
                    vm.ItemsForSale = vm.ItemsForSale.OrderBy(li => li.ContainerName).ThenBy(li => li.Name);
                    break;
                case "size":
                    vm.ItemsForSale = vm.ItemsForSale.OrderBy(li => li.ContainerSizeML).ThenBy(li => li.Name);
                    break;
                case "size_desc":
                    vm.ItemsForSale =
                        vm.ItemsForSale.OrderByDescending(li => li.ContainerSizeML).ThenBy(li => li.Name);
                    break;
                case "name_desc":
                    vm.ItemsForSale = vm.ItemsForSale.OrderByDescending(li => li.Name).ThenBy(li => li.Name);
                    break;
                default:
                    vm.ItemsForSale = vm.ItemsForSale.OrderBy(li => li.Name);
                    break;
            }
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
                if (lineItem.Categories.Any() == false)
                {
                    throw new ArgumentException("Cannot create an item without categories!");
                }

                var v = await _dal.GetVendor(vendorId);
                //check if it already exists
                if (v.GetItemsVendorSells()
                    .Any(li => BeverageLineItemEquator.AreSameBeverageLineItem(lineItem, li)))
                {
                    throw new ArgumentException("Item already exists for this vendor!");
                }

                var vendor = (Vendor)v;
                vendor.VendorBeverageLineItems.Add(lineItem);

                //save it
                await _dal.UpdateVendor(vendor);

                return RedirectToAction("Index", new RouteValueDictionary { {"vendorId", vendorId} });
            }
            catch(Exception)
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
                if (downCast == null)
                {
                    throw new InvalidOperationException("Vendor cannot be translated, please contact development");
                }
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

        private static int _orderId;

        /// <summary>
        /// TODO change this to event at some time?
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="fc"></param>
        /// <param name="lineItemId"></param>
        /// <returns></returns>
        private VendorBeverageLineItem GetLineItemFromFormCollection(Guid vendorId, FormCollection fc, Guid lineItemId)
        {
            var cats = fc["PostedCategoryGuids"].Split(',');
            var guids = cats.Where(s => string.IsNullOrWhiteSpace(s) == false).Select(Guid.Parse).ToList();
            var actualCats = _categoriesService.GetAssignableCategories().Where(c => guids.Contains(c.Id)).Cast<ItemCategory>().ToList();

            var amtInMl = decimal.Parse(fc["ContainerSizeML"]);
            var container = GetContainerForAmount(amtInMl);

            Money price = null;
            var priceV = fc["PublicPrice"];
            if (string.IsNullOrWhiteSpace(priceV) == false)
            {
                price = new Money(decimal.Parse(priceV));
            }

            int caseSize;
            int.TryParse(fc["CaseSize"], out caseSize);
            return new VendorBeverageLineItem
            {
                CaseSize = caseSize,
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

        private static LiquidContainer GetContainerForAmount(decimal ml)
        {
            var amt = new LiquidAmount {Milliliters = ml};
            var found = LiquidContainer.GetStandardBarSizes().FirstOrDefault(s => s.AmountContained.Equals(amt));
            return found ?? new LiquidContainer {AmountContained = new LiquidAmount {Milliliters = ml}};
        }
    }
}
