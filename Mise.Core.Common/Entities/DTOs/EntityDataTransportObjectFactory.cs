using System;
using Mise.Core.Entities.Base;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Entities.Restaurant;

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
			Guid? restaurantID = null;
			if(entity is IRestaurantEntityBase)
			{
				var restEnt = ((IRestaurantEntityBase)entity);
				if(restEnt.RestaurantID == Guid.Empty && entity is IRestaurant){
					restaurantID = entity.Id;
				} else{
					restaurantID = restEnt.RestaurantID;
				}
			}
            return new RestaurantEntityDataTransportObject
            {
                CreatedDate = entity.CreatedDate,
                Id = entity.Id,
                JSON = json,
                LastUpdatedDate = entity.LastUpdatedDate,
				RestaurantID = restaurantID,
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
