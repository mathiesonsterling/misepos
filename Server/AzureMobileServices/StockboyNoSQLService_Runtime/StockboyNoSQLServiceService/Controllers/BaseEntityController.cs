using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;
using StockboyNoSQLServiceService.Models;

namespace StockboyNoSQLServiceService.Controllers
{
    public abstract class BaseEntityController<TEntityType> : TableController<TEntityType> where TEntityType : class, ITableData
    {
        protected StockboyNoSQLServiceContext Context { get; private set; }
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Context = new StockboyNoSQLServiceContext();
            DomainManager = new EntityDomainManager<TEntityType>(Context, Request);
        }

        // GET tables/Restaurants
        public IQueryable<TEntityType> GetAllEnities()
        {
            return Query();
        }

        // GET tables/Restaurants/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<TEntityType> GetEntity(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Restaurants/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<TEntityType> PatchEntity(string id, Delta<TEntityType> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Restaurants
        public async Task<IHttpActionResult> PostEntity(TEntityType item)
        {
            var current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Restaurants/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteEntity(string id)
        {
            return DeleteAsync(id);
        }
    }
}