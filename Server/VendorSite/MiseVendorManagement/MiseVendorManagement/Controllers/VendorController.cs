using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation.Serialization;
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

        // GET: Vendor
        public async Task<ActionResult> Index()
        {
            //get all our vendors
            var vendors = await _dal.GetAllVendors();
            var viewModels = vendors.Select(v => new VendorViewModel(v));
            return View(viewModels);
        }

        // GET: Vendor/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            var vendor = await _dal.GetVendor(id);
            var vm = new VendorViewModel(vendor);
            return View(vm);
        }

        // GET: Vendor/Create
        public ActionResult Create()
        {
            return View();
        }

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
                    var vendor = VendorVMToVendor(vm);

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


        // GET: Vendor/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var vendor = await _dal.GetVendor(id);
            var vm = new VendorViewModel(vendor);
            return View(vm);
        }

        // POST: Vendor/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(VendorViewModel vm)
        {
            try
            {
                var updated = VendorVMToVendor(vm);
                await _dal.UpdateVendor(updated);
                
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Vendor/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            var vendor = await _dal.GetVendor(id);
            var vm = new VendorViewModel(vendor);
            return View(vm);
        }

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

        private static Vendor VendorVMToVendor(VendorViewModel vm)
        {
            //convert this to a vendor!
            var vendor = new Vendor
            {
                CreatedDate = DateTimeOffset.UtcNow,
                EmailToOrderFrom = new EmailAddress(vm.Email),
                Id = Guid.NewGuid(),
                LastUpdatedDate = DateTimeOffset.UtcNow,
                Name = vm.Name,
                PhoneNumber = new PhoneNumber(vm.PhoneAreaCode, vm.PhoneNumber),
                Revision = new EventID(MiseAppTypes.VendorManagement, 1),
                Verified = false,
                StreetAddress =
                    new StreetAddress(vm.StreetAddressNumber, vm.StreetDirection, vm.StreetName, vm.City, vm.State, vm.Country,
                        vm.ZipCode)
            };
            return vendor;
        }
    }
}
