using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mise.Core.ValueItems;
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

        [Authorize(Roles= "MiseAdmin")]
        // GET: Restaurant
        public async Task<ActionResult> Index()
        {
            var rests = await _dal.GetAllRestaurants();
            var vms = rests.Select(r => new RestaurantViewModel(r));

            return View(vms.OrderBy(r => r.Name));
        }

        [Authorize]
        public async Task<ActionResult> IndexForUser()
        {
            var email = new EmailAddress(User.Identity.Name);
            var userManager = new MiseUserServices();
            var restsICanSee = await userManager.GetRestaurantIdsForEmail(email);

            var vms = new List<RestaurantViewModel>();
            foreach (var restId in restsICanSee)
            {
                var rest = await _dal.GetRestaurantById(restId);
                if (rest != null)
                {
                    var vm = new RestaurantViewModel(rest);
                    vms.Add(vm);
                }
            }

            return View("Index", vms.OrderBy(r => r.Name));
        }

        [Authorize]
        // GET: Restaurant/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            var rest = await _dal.GetRestaurantById(id);
            var restVM = new RestaurantViewModel(rest);
            return View(restVM);
        }

        [Authorize]
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

        [Authorize]
        public async Task<ActionResult> Delete(Guid id)
        {
            var ent = await _dal.GetRestaurantById(id);
            var vm = new RestaurantViewModel(ent);
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(RestaurantViewModel vm)
        {
            try
            {
                var id = vm.Id;
                await _dal.DeleteRestaurant(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            } 
        }
    }
}
