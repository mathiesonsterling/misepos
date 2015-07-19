using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Entities.Base;
using Mise.Core.Server.Services.DAL;

namespace Mise.InventoryWebService.ServiceInterface.Services
{
    public class FakeEventStore : IEventStorageDAL
    {
        private readonly List<IEntityEventBase> _eventStore = new List<IEntityEventBase>();

        public Task<bool> StoreEventsAsync(IEnumerable<IEntityEventBase> events)
        {
            _eventStore.AddRange(events);
            return Task.FromResult(true);
        }

        public Task<bool> StoreEventsAsync(IEnumerable<EventDataTransportObject> dtos)
        {
            return StoreEventsAsync(dtos.Cast<IEntityEventBase>());
        }

        public Task<IEnumerable<IEntityEventBase>> GetEventsSince(Guid? restaurantID, DateTimeOffset date)
        {
            return Task.Run(() => _eventStore.Where(ev => ev.RestaurantID == restaurantID && ev.CreatedDate > date));
        }
    }
}
