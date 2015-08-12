using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;

namespace Mise.Core.Client.Repositories
{
    public class ClientInventoryRepository : BaseEventSourcedClientRepository<IInventory, IInventoryEvent, Inventory>, IInventoryRepository
    {
        private readonly IInventoryWebService _inventoryWebService;
        public ClientInventoryRepository(ILogger logger, IInventoryWebService webService)
            : base(logger, webService)
        {
            _inventoryWebService = webService;
        }

        protected override IInventory CreateNewEntity()
        {
            return new Inventory();
        }


        public override Guid GetEntityID(IInventoryEvent ev)
        {
            return ev.InventoryID;
        }
			

        protected override Task<IEnumerable<Inventory>> LoadFromWebservice(Guid? restaurantID)
        {
			try{
	            if (restaurantID.HasValue)
	            {
	                return _inventoryWebService.GetInventoriesForRestaurant(restaurantID.Value);
	            }
	            return Task.FromResult(new List<Inventory>().AsEnumerable());
			} catch(Exception e){
				Logger.HandleException (e);
				throw;
			}
        }


		public Task<IInventory> GetCurrentInventory(Guid restaurantID)
		{
		    var inv = GetAll().FirstOrDefault(i => i.IsCurrent && i.RestaurantID == restaurantID);
		    return Task.FromResult(inv);
		}
    }
}
