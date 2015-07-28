using System;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Repositories;
using Mise.Core.Services;
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

        protected override bool IsEventACreation(IEntityEventBase ev)
        {
            return ev is InventoryCreatedEvent;
        }

        public override Guid GetEntityID(IInventoryEvent ev)
        {
            return ev.InventoryID;
        }

        public override async Task Load(Guid? restaurantID)
        {
            Loading = true;
            if (restaurantID.HasValue)
            {
				var items = await _inventoryWebService.GetInventoriesForRestaurant (restaurantID.Value);
				Cache.UpdateCache (items);
            }
            Loading = false;
        }

		public Task<IInventory> GetCurrentInventory(Guid restaurantID)
		{
		    var inv = GetAll().FirstOrDefault(i => i.IsCurrent && i.RestaurantID == restaurantID);
		    return Task.FromResult(inv);
		}
    }
}
