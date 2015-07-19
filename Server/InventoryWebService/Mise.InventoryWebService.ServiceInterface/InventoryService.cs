using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Mise.Core.Repositories;
using Mise.InventoryWebService.ServiceModelPortable.Responses;
using ServiceStack;
using Inventory = Mise.InventoryWebService.ServiceModelPortable.Responses.Inventory;

namespace Mise.InventoryWebService.ServiceInterface
{
    public class InventoryService : Service
    {
        private readonly IInventoryRepository _respository;

        public InventoryService(IInventoryRepository repos)
        {
            _respository = repos;
        }

        public InventoryResponse Get(Inventory request)
        {
            if (_respository.Loading)
            {
                Thread.Sleep(100);
                if (_respository.Loading)
                {
                    throw new HttpError(HttpStatusCode.ServiceUnavailable, "Server has not yet loaded");
                }
            }

            IEnumerable<Core.Common.Entities.Inventory.Inventory> items;
            if (request.InventoryID.HasValue)
            {
                items = new List<Core.Common.Entities.Inventory.Inventory>
                {
                    _respository.GetByID(request.InventoryID.Value) as Core.Common.Entities.Inventory.Inventory
                };
            }
            else
            {
                items = _respository.GetAll().Where(ri => ri.RestaurantID == request.RestaurantID).Cast<Core.Common.Entities.Inventory.Inventory>();
            }

            return new InventoryResponse
                {
                    Results = items
                };
        }
    }
}
