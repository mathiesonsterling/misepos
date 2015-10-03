using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Services.Implementation.Serialization;
using MiseReporting.Models;

namespace MiseReporting.Controllers
{
    public class RestaurantController : Controller
    {

        private readonly EntityDataTransportObjectFactory _dtoFactory;
        private readonly string _restType;
        public RestaurantController()
        {
            _restType = typeof(Restaurant).ToString();
            _dtoFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
        }

        // GET: Restaurant
        public async Task<ActionResult> Index()
        {
            var viewModels = new List<RestaurantViewModel>();
            using (var db = new AzureNonTypedEntities())
            {
                var restAIs = await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _restType).ToListAsync();
                var dtos = restAIs.Select(ai => ai.ToRestaurantDTO());
                var rests = dtos.Select(dto => _dtoFactory.FromDataStorageObject<Restaurant>(dto));
                var vms = rests.Select(r => new RestaurantViewModel(r));
                viewModels.AddRange(vms);
            }

            return View(viewModels.OrderBy(r => r.Name));
        }

        // GET: Restaurant/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            var restVM = await GetVMForId(id);

            return View(restVM);
        }


        public ActionResult Create()
        {
            var vm = new RestaurantViewModel();
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Create(RestaurantViewModel vm)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return View(vm);
                }

                //TODO check we're not duplicating
                vm.Id = Guid.NewGuid();
                var ent = vm.ToEntity();
                var dto = _dtoFactory.ToDataTransportObject(ent);
                var ai = new AzureEntityStorage(dto);

                using (var db = new AzureNonTypedEntities())
                {
                    db.AzureEntityStorages.Add(ai);
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            var vm = await GetVMForId(id);

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(RestaurantViewModel vm)
        {
            try
            {
                var id = vm.Id;
                using (var db = new AzureNonTypedEntities())
                {//delete all items for the restaurant as well
                    var items = await db.AzureEntityStorages.Where(ai => ai.RestaurantID == id).ToListAsync();
                    foreach (var item in items)
                    {
                        item.Deleted = true;
                        db.Entry(item).State = EntityState.Modified;
                        db.AzureEntityStorages.Remove(item);
                    }

                    //also get all employees that are listed here and remove the call for them
                    var empType = typeof (Employee).ToString();
                    var empAIs = await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == empType && ai.EntityJSON.Contains(id.ToString())).ToListAsync();
                    var dtos = empAIs.Select(ai => ai.ToRestaurantDTO());
                    var emps = dtos.Select(dto => _dtoFactory.FromDataStorageObject<Employee>(dto));
                    foreach (var emp in emps.Where(emp => emp.RestaurantsAndAppsAllowed.ContainsKey(id)))
                    {
                        emp.RestaurantsAndAppsAllowed.Remove(id);
                    }

                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            } 
        }

        private async Task<RestaurantViewModel> GetVMForId(Guid id)
        {
            RestaurantViewModel restVM;
            using (var db = new AzureNonTypedEntities())
            {
                var restAI = await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _restType && ai.EntityID == id)
                    .FirstOrDefaultAsync();
                var dto = restAI.ToRestaurantDTO();
                restVM = new RestaurantViewModel(_dtoFactory.FromDataStorageObject<Restaurant>(dto));
            }
            return restVM;
        }
    }
}
