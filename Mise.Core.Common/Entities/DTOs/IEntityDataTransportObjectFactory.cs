using Mise.Core.Entities.Base;

namespace Mise.Core.Common.Entities.DTOs
{
    public interface IEntityDataTransportObjectFactory
    {
        RestaurantEntityDataTransportObject ToDataTransportObject<T>(T entity) where T :IEntityBase, new();
        T FromDataStorageObject<T>(RestaurantEntityDataTransportObject dto) where T:class;
    }
}