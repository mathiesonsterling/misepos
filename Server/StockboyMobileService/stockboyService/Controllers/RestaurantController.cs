using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using stockboyService.DataObjects;
using stockboyService.Models;

namespace stockboyService.Controllers
{
    public class RestaurantController : TableController<Restaurant>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            stockboyContext context = new stockboyContext();
            DomainManager = new EntityDomainManager<Restaurant>(context, Request, Services);
        }

        // GET tables/Restaurant
        public IQueryable<Restaurant> GetAllRestaurant()
        {
            return Query(); 
        }

        // GET tables/Restaurant/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Restaurant> GetRestaurant(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Restaurant/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Restaurant> PatchRestaurant(string id, Delta<Restaurant> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Restaurant
        public async Task<IHttpActionResult> PostRestaurant(Restaurant item)
        {
            Restaurant current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Restaurant/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteRestaurant(string id)
        {
             return DeleteAsync(id);
        }

    }
}