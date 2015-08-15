using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mise.Core.ValueItems;
using MiseReporting.Models;
using MiseReporting.Repository;

namespace MiseReporting.Controllers
{
    public class RestaurantController : Controller
    {
        // GET: Restaurant
        public ActionResult Index()
        {
            var restViewModels = new List<RestaurantViewModel>
            {
                new RestaurantViewModel
                {
                    Address = StreetAddress.TestStreetAddress.ToString(),
                    Id = RepositoryFactory.TestRestGUID,
                    Name = "Dummy"
                }
            };
            return View(restViewModels);
        }

        // GET: Restaurant/Details/5
        public ActionResult Details(Guid id) 
        {
            return View();
        }
    }
}
