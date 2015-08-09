using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.WindowsAzure.Mobile.Service;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services.Implementation.Serialization;
using stockboymobileserviceService.DataObjects;
using stockboymobileserviceService.Models;

namespace stockboymobileserviceService.Controllers
{
    public class InventoryMonitorController : ApiController
    {
        public ApiServices Services { get; set; }

        private stockboymobileserviceContext _context;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            _context = new stockboymobileserviceContext();

        }

        // GET api/InventoryMonitor
        public IEnumerable<Inventory> GetAllInventories()
        {
            var invType = typeof (Inventory).ToString();
            //get our items
            
            var azureStorageItems = _context.AzureEntityStorages
                .Where(ae => ae.MiseEntityType == invType)
                .ToList();

            //deserialize and return
            var serializer = new JsonNetSerializer();
            var desers = azureStorageItems.Select(ai => ai.JSON)
                .Select(json => serializer.Deserialize<Inventory>(json));

            return desers;
        }

    }
}
