using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using stockboymobileserviceService.DataObjects;
using stockboymobileserviceService.Models;

namespace stockboymobileserviceService.Controllers
{
    public class AzureEventStorageController : TableController<AzureEventStorage>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new stockboymobileserviceContext();
            DomainManager = new EntityDomainManager<AzureEventStorage>(context, Request, Services);
        }

        // GET tables/AzureEventStorage
        public IQueryable<AzureEventStorage> GetAllAzureEventStorage()
        {
            return Query(); 
        }

        // GET tables/AzureEventStorage/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<AzureEventStorage> GetAzureEventStorage(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/AzureEventStorage/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<AzureEventStorage> PatchAzureEventStorage(string id, Delta<AzureEventStorage> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/AzureEventStorage/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostAzureEventStorage(AzureEventStorage item)
        {
            AzureEventStorage current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/AzureEventStorage/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteAzureEventStorage(string id)
        {
             return DeleteAsync(id);
        }

    }
}