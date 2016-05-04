using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Mise.Database.AzureDefinitions;
using Mise.Database.AzureDefinitions.Entities.Restaurant;
using StockboyMobileAppServiceService.Models;

namespace StockboyMobileAppServiceService.Controllers
{
    public class RestaurantController : TableController<Restaurant>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new StockboyMobileAppServiceContext();
            DomainManager = new EntityDomainManager<Restaurant>(context, Request);
        }

        // GET tables/TodoItem
        public IQueryable<Restaurant> GetAllRestaurants()
        {
            try
            {
                return Query();
            }
            catch (Exception e)
            {
                var msg = e.Message;
                throw;
            }
        }

        // GET tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Restaurant> GetRestaurant(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Restaurant> PatchRestaurant(string id, Delta<Restaurant> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/TodoItem
        public async Task<IHttpActionResult> PostRestaurant(Restaurant item)
        {
            var current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteRestaurant(string id)
        {
            return DeleteAsync(id);
        }
    }
}