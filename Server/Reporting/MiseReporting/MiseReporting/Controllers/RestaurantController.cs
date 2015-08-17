using System;
using System.Collections.Generic;
using System.Linq;
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
        public RestaurantController()
        {
            _dtoFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
        }

        // GET: Restaurant
        public ActionResult Index()
        {
            var restType = typeof (Restaurant).ToString();
            var viewModels = new List<RestaurantViewModel>();
            using (var db = new AzureNonTypedEntities())
            {
                var restAIs = db.AzureEntityStorages.Where(ai => ai.MiseEntityType == restType).ToList();
                var dtos = restAIs.Select(ai => ai.ToRestaurantDTO());
                var rests = dtos.Select(dto => _dtoFactory.FromDataStorageObject<Restaurant>(dto));
                var vms = rests.Select(r => new RestaurantViewModel(r));
                viewModels.AddRange(vms);
            }

            return View(viewModels);
        }

        // GET: Restaurant/Details/5
        public ActionResult Details(Guid id) 
        {
            return View();
        }
    }
}
