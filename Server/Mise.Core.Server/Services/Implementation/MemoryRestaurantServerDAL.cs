using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;
using Mise.Core.Common.Services.DAL;
namespace Mise.Core.Server.Services.Implementation
{
    public class MemoryRestaurantServerDAL : IRestaurantServerDAL
    {
        private readonly Dictionary<Guid, IEntityBase> _entities;
        private readonly Dictionary<Guid, IEntityEventBase> _events;

        public MemoryRestaurantServerDAL()
        {
            _entities = new Dictionary<Guid, IEntityBase>();
            _events = new Dictionary<Guid, IEntityEventBase>();
        }

        public bool StoreEvents(IEnumerable<IEntityEventBase> events)
        {
            foreach (var ev in events)
            {
                if (_events.ContainsKey(ev.ID) == false)
                {
                    _events.Add(ev.ID, ev);
                }
                else
                {
                    _events[ev.ID] = ev;
                }
            }

            return true;
        }

        public Task<bool> StoreEventsAsync(IEnumerable<IEntityEventBase> events)
        {
            return Task.Factory.StartNew(() => StoreEvents(events));
        }

        public bool UpsertEntities(IEnumerable<IEntityBase> entities)
        {
            foreach (var ent in entities)
            {
                if (_entities.ContainsKey(ent.ID))
                {
                    _entities[ent.ID] = ent;
                }
                else
                {
                    _entities.Add(ent.ID, ent);
                }
            }

            return true;
        }

        public Task<bool> UpsertEntitiesAsync(IEnumerable<IEntityBase> entities)
        {
            return Task.Factory.StartNew(() => UpsertEntities(entities));
        }

        public IEnumerable<T> GetEntities<T>(Guid? restaurantID) where T : class, IEntityBase
        {
            return _entities.Values.OfType<T>();
        }

        public Task<IEnumerable<T>> GetEntitiesAsync<T>(Guid? restaurantID) where T : class, IEntityBase
        {
            return Task.Factory.StartNew(() => _entities.Values.OfType<T>());
        }

        public IEnumerable<T> GetEntities<T>() where T : class, IEntityBase
        {
            return _entities.Values.OfType<T>();
        }

        public Task<IEnumerable<T>> GetEntitiesAsync<T>() where T : class, IEntityBase
        {
            return Task.Factory.StartNew(() => _entities.Values.OfType<T>());
        }

        public T GetEntityByID<T>(Guid id) where T : class, IEntityBase
        {
            if (_entities.ContainsKey(id))
            {
                return _entities[id] as T;
            }

            return null;
        }

        public Task<T> GetEntityByIDAsync<T>(Guid id) where T : class, IEntityBase
        {
            return Task.Factory.StartNew(() => GetEntityByID<T>(id));
        }

        public T GetEntityByID<T>(Guid? restaurantID, Guid id) where T : class, IEntityBase 
        {
            return GetEntityByID<T>(id);
        }

        public Task<T> GetEntityByIDAsync<T>(Guid? restaurantID, Guid id) where T : class, IEntityBase
        {
            return Task.Factory.StartNew(() => GetEntityByID<T>(restaurantID, id));
        }

        public bool Delete<T>(T entity) where T:class, IEntityBase
        {
            if (entity == null)
            {
                return false;
            }

            if (_entities.ContainsKey(entity.ID))
            {
                return _entities.Remove(entity.ID);
            }
            return false;
        }

        public Task<bool> UpdateEventStatusesAsync(IEnumerable<IEntityEventBase> events, ItemCacheStatus status)
        {
            return Task.Factory.StartNew(() => true);
        }
    }
}
