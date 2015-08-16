using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation.Serialization;
using MiseVendorManagement.Models;

namespace MiseVendorManagement.Controllers
{
    public class VendorController : Controller
    {
        private readonly EntityDataTransportObjectFactory _entityFactory;
        public VendorController()
        {
            _entityFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
        }

        // GET: Vendor
        public ActionResult Index()
        {
            //get all our vendors
            using (var db = new AzureDB())
            {
                var vendorType = typeof (Vendor).ToString();
                var ais = db.AzureEntityStorages.Where(ai => ai.MiseEntityType == vendorType).ToList();
                var dtos = ais.Select(ai => ai.ToRestaurantDTO());
                var vendors = dtos.Select(dto => _entityFactory.FromDataStorageObject<Vendor>(dto));
                var viewModels = vendors.Select(v => new VendorViewModel(v));
                return View(viewModels);
            }
        }

        // GET: Vendor/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Vendor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Vendor/Create
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

        // GET: Vendor/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Vendor/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
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

        // GET: Vendor/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Vendor/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
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
        }
    }
}
