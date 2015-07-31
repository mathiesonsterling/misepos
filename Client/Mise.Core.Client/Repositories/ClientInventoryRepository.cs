using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Services;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Services.WebServices;

namespace Mise.Core.Client.Repositories
{
    public class ClientInventoryRepository : BaseEventSourcedClientRepository<IInventory, IInventoryEvent>, IInventoryRepository
    {
        private readonly IInventoryWebService _inventoryWebService;
        public ClientInventoryRepository(ILogger logger, IClientDAL dal, IInventoryWebService webService, IResendEventsWebService resend)
            : base(logger, dal, webService, resend)
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

        protected override async Task<IEnumerable<IInventory>> LoadFromDB(Guid? restaurantID)
        {
            var items = await DAL.GetEntitiesAsync<Inventory>();
            return items;
        }

        protected override Task<IEnumerable<IInventory>> LoadFromWebservice(Guid? restaurantID)
        {
            if (restaurantID.HasValue)
            {

                return _inventoryWebService.GetInventoriesForRestaurant(restaurantID.Value);
            }
            return Task.FromResult(new List<IInventory>().AsEnumerable());
        }


		public Task<IInventory> GetCurrentInventory(Guid restaurantID)
		{
		    var inv = GetAll().FirstOrDefault(i => i.IsCurrent && i.RestaurantID == restaurantID);
		    return Task.FromResult(inv);
		}
    }
}
