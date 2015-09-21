using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation.Serialization;
using MiseReporting.Models;

namespace MiseReporting.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EntityDataTransportObjectFactory _dtoFactory;
        private readonly string _empType;
        private readonly string _restType;
        public EmployeeController()
        {
            _dtoFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _empType = typeof(Employee).ToString();
            _restType = typeof(Restaurant).ToString();
        }

        // GET: Employee
        public async Task<ActionResult> Index()
        {
            var vms = new List<EmployeeViewModel>();
            using (var db = new AzureNonTypedEntities())
            {
                var emps = await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _empType).ToListAsync();
                //populate the vendors?

                foreach (var ai in emps)
                {
                    var vm = await HydrateEmployeeVM(ai, db);
                    vms.Add(vm);
                }
            }
            return View(vms);
        }


        // GET: Employee/Details/5
        public async Task<ActionResult> Details(Guid empId)
        {
            using (var db = new AzureNonTypedEntities())
            {
                var emp =
                    await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _empType && ai.EntityID == empId).FirstOrDefaultAsync();

                if (emp == null)
                {
                    throw new ArgumentException("No employee found for id " + empId);
                }
                var vm = await HydrateEmployeeVM(emp, db);
                return View(vm);
            }
        }

        // GET: Employee/Create
        public async Task<ActionResult> Create()
        {
            IEnumerable<RestaurantViewModel> restVms;
            using (var db = new AzureNonTypedEntities())
            {
                var restAIs = await db.AzureEntityStorages.Where(a => a.MiseEntityType == _restType).ToListAsync();
                var rests = restAIs.Select(ra => _dtoFactory.FromDataStorageObject<Restaurant>(ra.ToRestaurantDTO()));

                restVms = rests.Select(r => new RestaurantViewModel(r));
            }
            var vm = new EmployeeViewModel {PossibleRestaurants = restVms};
            return View(vm);
        }

        // POST: Employee/Create
        [HttpPost]
        public ActionResult Create(EmployeeViewModel emp)
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

        // GET: Employee/Edit/5
        public ActionResult Edit(Guid empId)
        {
            return View();
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid empId, FormCollection collection)
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

        // GET: Employee/Delete/5
        public ActionResult Delete(Guid id)
        {
            return View();
        }

        // POST: Employee/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, FormCollection collection)
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

        private async Task<EmployeeViewModel> HydrateEmployeeVM(AzureEntityStorage ai, AzureNonTypedEntities db)
        {
            var emp = _dtoFactory.FromDataStorageObject<Employee>(ai.ToRestaurantDTO());
            var empRestaurants = emp.GetRestaurantIDs().ToList();
            var restAIs = await db.AzureEntityStorages.Where(
                a => a.MiseEntityType == _restType && empRestaurants.Contains(a.EntityID)).ToListAsync();
            var rests = restAIs.Select(ra => _dtoFactory.FromDataStorageObject<Restaurant>(ra.ToRestaurantDTO()));

            var restVms = rests.Select(r => new RestaurantViewModel(r));
            var vm = new EmployeeViewModel(emp, restVms);
            return vm;
        }
    }
}
