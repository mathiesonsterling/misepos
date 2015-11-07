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
    public class AzureEntityStorageController : TableController<AzureEntityStorage>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            stockboymobileserviceContext context = new stockboymobileserviceContext();
            DomainManager = new EntityDomainManager<AzureEntityStorage>(context, Request, Services);
        }

        // GET tables/AzureEntityStorage
        public IQueryable<AzureEntityStorage> GetAllAzureEntityStorage()
        {
            return Query(); 
        }

        // GET tables/AzureEntityStorage/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<AzureEntityStorage> GetAzureEntityStorage(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/AzureEntityStorage/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<AzureEntityStorage> PatchAzureEntityStorage(string id, Delta<AzureEntityStorage> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/AzureEntityStorage
        public async Task<IHttpActionResult> PostAzureEntityStorage(AzureEntityStorage item)
        {
            AzureEntityStorage current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/AzureEntityStorage/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteAzureEntityStorage(string id)
        {
             return DeleteAsync(id);
        }

    }
}