using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mise.Core.Common.Entities;
using MiseReporting.Models;

namespace MiseReporting.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ManagementDAL _dal;
        public RestaurantController()
        {
            _dal = new ManagementDAL();
        }

        // GET: Restaurant
        public async Task<ActionResult> Index()
        {
            var rests = await _dal.GetAllRestaurants();
            var vms = rests.Select(r => new RestaurantViewModel(r));

            return View(vms.OrderBy(r => r.Name));
        }

        // GET: Restaurant/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            var rest = await _dal.GetRestaurantById(id);
            var restVM = new RestaurantViewModel(rest);
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
                await _dal.InsertRestaurant(ent);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            var ent = await _dal.GetRestaurantById(id);
            var vm = new RestaurantViewModel(ent);
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
                    var emps = (await _dal.GetAllEmployeesContaining(id.ToString())).Cast<Employee>();
                    foreach (var emp in emps.Where(emp => emp.RestaurantsAndAppsAllowed.ContainsKey(id)))
                    {
                        emp.RestaurantsAndAppsAllowed.Remove(id);
                    }

                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            } 
        }
    }
}
