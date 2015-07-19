using Mise.Core.Entities.Base;

namespace Mise.Core.Common.Entities.DTOs
{
    public interface IEntityDataTransportObjectFactory
    {
        RestaurantEntityDataTransportObject ToDataTransportObject(IEntityBase entity);
        T FromDataStorageObject<T>(RestaurantEntityDataTransportObject dto) where T:class;
    }
}