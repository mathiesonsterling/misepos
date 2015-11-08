using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Entities;
using Mise.Core.ValueItems;
using MiseVendorManagement.Models;

namespace MiseVendorManagement.Controllers
{
    public class VendorController : Controller
    {
        private readonly VendorDAL _dal;
        public VendorController()
        {
            _dal = new VendorDAL();
        }

        [Authorize(Roles = "MiseAdmin")]
        // GET: Vendor
        public async Task<ActionResult> Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParam = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.CitySortParam = sortOrder == "city" ? "city_desc" : "city";
            ViewBag.StateSortParam = sortOrder == "state" ? "state_desc" : "state";
            ViewBag.WebsiteSortParam = sortOrder == "website" ? "website_desc" : "website";
            ViewBag.NumItemsSortParam = sortOrder == "numitems" ? "numitems_desc" : "numitems";

            //get all our vendors
            var vendors = await _dal.GetVendors(searchString);
            var viewModels = vendors.Select(v => new VendorViewModel(v));

            switch (sortOrder)
            {
                case "city":
                    viewModels = viewModels.OrderBy(v => v.City).ThenBy(v => v.Name);
                    break;
                case "city_desc":
                    viewModels = viewModels.OrderByDescending(v => v.City).ThenBy(v => v.Name);
                    break;
                case "state":
                    viewModels = viewModels.OrderBy(v => v.State).ThenBy(v => v.Name);
                    break;
                case "state_desc":
                    viewModels = viewModels.OrderByDescending(v => v.State).ThenBy(v => v.Name);
                    break;
                case "website":
                    viewModels = viewModels.OrderBy(v => v.Website).ThenBy(v => v.Name);
                    break;
                case "website_desc":
                    viewModels = viewModels.OrderByDescending(v => v.Website).ThenBy(v => v.Name);
                    break;
                case "name_desc":
                    viewModels = viewModels.OrderByDescending(v => v.Name);
                    break;
                case "name":
                    viewModels = viewModels.OrderBy(v => v.Name);
                    break;
                case "numitems":
                    viewModels = viewModels.OrderBy(v => v.NumItems).ThenBy(v => v.Name);
                    break;
                case "numitems_desc":
                    viewModels = viewModels.OrderByDescending(v => v.NumItems).ThenBy(v => v.Name);
                    break;
                default:
                    viewModels = viewModels.OrderBy(v => v.State).ThenBy(v => v.City).ThenBy(v => v.Name);
                    break;
            }
            return View(viewModels);
        }

        [Authorize(Roles = "MiseAdmin, VendorAccount")]
        // GET: Vendor/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            var vendor = await _dal.GetVendor(id);
            var vm = new VendorViewModel(vendor);
            return View(vm);
        }

        [Authorize(Roles = "MiseAdmin, VendorAccount")]
        // GET: Vendor/Create
        public ActionResult Create()
        {
            var vm = new VendorViewModel
            {
                Country = Country.UnitedStates.Name
            };
            return View(vm);
        }

        [Authorize(Roles = "MiseAdmin, VendorAccount")]
        // POST: Vendor/Create
        [HttpPost]
        public async Task<ActionResult> Create(VendorViewModel vm)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid == false)
                {
                    return View(vm);
                }
                    var vendor = VendorVMToVendor(vm, Guid.NewGuid());

                    //TODO get geolocation here

                    //store this
                    await _dal.InsertVendor(vendor);
                    return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "MiseAdmin, VendorAccount")]
        // GET: Vendor/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var vendor = await _dal.GetVendor(id);
            var vm = new VendorViewModel(vendor);
            return View(vm);
        }

        [Authorize(Roles = "MiseAdmin, VendorAccount")]
        // POST: Vendor/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(VendorViewModel vm)
        {
            try
            {
                var updated = VendorVMToVendor(vm, vm.Id);
                await _dal.UpdateVendor(updated);
                
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "MiseAdmin, VendorAccount")]
        // GET: Vendor/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            var vendor = await _dal.GetVendor(id);
            var vm = new VendorViewModel(vendor);
            return View(vm);
        }

        [Authorize(Roles = "MiseAdmin, VendorAccount")]
        // POST: Vendor/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(Guid id, FormCollection collection)
        {
            try
            {
                await _dal.DeleteVendor(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private static Vendor VendorVMToVendor(VendorViewModel vm, Guid vendorId)
        {
            //convert this to a vendor!
            var vendor = new Vendor
            {
                CreatedDate = DateTimeOffset.UtcNow,
                EmailToOrderFrom = new EmailAddress(vm.Email),
                Id = vendorId,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                Name = vm.Name,
                PhoneNumber = new PhoneNumber(vm.PhoneAreaCode, vm.PhoneNumber),
                Revision = new EventID(MiseAppTypes.VendorManagement, 1),
                Verified = false,
                StreetAddress =
                    new StreetAddress(vm.StreetAddressNumber, vm.StreetDirection, vm.StreetName, vm.City, vm.State, vm.Country,
                        vm.ZipCode),
                Website = new Uri(vm.Website)
            };
            return vendor;
        }
    }
}
