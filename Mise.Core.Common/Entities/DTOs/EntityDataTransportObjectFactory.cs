using System;
using Mise.Core.Entities.Base;
using Mise.Core.Services.UtilityServices;

namespace Mise.Core.Common.Entities.DTOs
{
    public class EntityDataTransportObjectFactory : IEntityDataTransportObjectFactory
    {
        private readonly IJSONSerializer _serializer;
        public EntityDataTransportObjectFactory(IJSONSerializer serializer)
        {
            _serializer = serializer;
        }

        public RestaurantEntityDataTransportObject ToDataTransportObject<T>(T entity) where T:IEntityBase, new()
        {
            var json = _serializer.Serialize(entity);
            return new RestaurantEntityDataTransportObject
            {
                CreatedDate = entity.CreatedDate,
                ID = entity.ID,
                JSON = json,
                LastUpdatedDate = entity.LastUpdatedDate,
				RestaurantID = entity is IRestaurantEntityBase? ((IRestaurantEntityBase)entity).RestaurantID : (Guid?)null,
                Revision = entity.Revision,
                SourceType = entity.GetType()
            };
        }

        public T FromDataStorageObject<T>(RestaurantEntityDataTransportObject dto) where T:class
        {
            var json = dto.JSON;
            return _serializer.Deserialize<T>(json);
        }
    }
}
