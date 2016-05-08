using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Mise.Database.AzureDefinitions.Entities;
using Mise.Database.AzureDefinitions.Entities.Restaurant;
using StockboyMobileAppServiceService.Models;

namespace StockboyMobileAppServiceService.Controllers
{
    public abstract class BaseEntityController<TEntityType> : TableController<TEntityType> where TEntityType : EntityData
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new StockboyMobileAppServiceContext();
            DomainManager = new EntityDomainManager<TEntityType>(context, Request);
        }

        // GET tables/Restaurants
        public IQueryable<TEntityType> GetAllEnities()
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