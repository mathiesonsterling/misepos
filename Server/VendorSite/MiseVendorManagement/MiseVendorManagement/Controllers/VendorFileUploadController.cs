using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mise.Core.Common.Services.Implementation;
using Mise.VendorManagement.Services;
using Mise.VendorManagement.Services.Implementation;
using MiseVendorManagement.Database;
using MiseVendorManagement.Models;

namespace MiseVendorManagement.Controllers
{
    public class VendorFileUploadController : Controller
    {
        private readonly VendorDAL _dal;
        private readonly IFileStorage _fileStorage;
        private readonly IVendorCSVImportService _csvImportService;
        public VendorFileUploadController()
        {
            _dal = new VendorDAL();
            _fileStorage = new AzureFileStorage();
            _csvImportService = new VendorCSVImportService(new DummyLogger());
        }

        // GET: VendorFileUpload
        public async Task<ActionResult> Index(Guid vendorId)
        {
            //get the vendor
            var vendor = await _dal.GetVendor(vendorId);
            var vm = new VendorViewModel(vendor);

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(Guid vendorId, HttpPostedFileBase file)
        {
            var vendor = await _dal.GetVendor(vendorId);
            var vm = new VendorCSVImportFileViewModel
            {
                FileKey = vendorId.ToString(),
                Id = vendorId,
                VendorName = vendor.Name
            };
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    var memStream = new MemoryStream();
                    file.InputStream.CopyTo(memStream);
                    await _fileStorage.StoreFile(vendorId.ToString(), memStream);

                    ViewBag.Message = "File uploaded successfully";

                    //get the columns to set
                    vm.ColumnNames = _csvImportService.GetColumnNames(memStream);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            return View(vm);
        }

        // GET: VendorFileUpload/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: VendorFileUpload/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VendorFileUpload/Create
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

        // GET: VendorFileUpload/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VendorFileUpload/Edit/5
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

        // GET: VendorFileUpload/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VendorFileUpload/Delete/5
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
