using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Server.Windows.Services;
using Mise.Core.Server.Windows.Services.Implementation;
using Mise.Core.ValueItems;
using MiseWebsite.Database.Implementation;
using MiseWebsite.Models;
using MiseWebsite.Services.Implementation;

namespace MiseWebsite.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly IGeoCodingService _geoCodingService;
        private readonly ManagementDAL _dal;
        public RestaurantController()
        {
            _dal = new ManagementDAL();
            _geoCodingService = new GoogleMapsGeoCodingService();
        }

        [Authorize(Roles = "MiseAdmin")]
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

                vm.Id = Guid.NewGuid();
                var ent = vm.ToEntity();

                //check we're not duplicating
                await AddLocation(ent);

                var existings = await _dal.GetAllRestaurants();
                var existing = existings.FirstOrDefault(r => r.StreetAddress.Equals(ent.StreetAddress));
                if (existing != null)
                {
                    throw new ArgumentException("A restaurant named " + existing.Name.FullName + " already exists in this location!");
                }
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

        private async Task<IRestaurant> AddLocation(IRestaurant rest)
        {
            if (rest.StreetAddress?.StreetAddressNumber != null)
            {
                var location = await _geoCodingService.GetLocationForAddress(rest.StreetAddress);
                if (location != null)
                {
                    rest.StreetAddress.StreetAddressNumber.Latitude = location.Latitude;
                    rest.StreetAddress.StreetAddressNumber.Longitude = location.Longitude;
                }
            }

            return rest;
        }
    }
}