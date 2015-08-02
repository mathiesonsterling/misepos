using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Common.Entities.DTOs.AzureTypes
{
    /// <summary>
    /// Quick non-typed class for storage of any entity
    /// </summary>
    public class AzureEntityStorage
    {
        public AzureEntityStorage()
        {
        }

        public AzureEntityStorage(RestaurantEntityDataTransportObject dto)
        {
            id = EntityID.ToString();
            MiseEntityType = dto.SourceType.ToString();
            EntityID = dto.ID;
            RestaurantID = dto.RestaurantID;
            JSON = dto.JSON;
            LastUpdatedDate = dto.LastUpdatedDate;
        }

        public RestaurantEntityDataTransportObject ToRestaurantDTO()
        {
            return new RestaurantEntityDataTransportObject
            {
                SourceType = Type.GetType(MiseEntityType),
                ID = EntityID,
                RestaurantID = RestaurantID,
                JSON = JSON,
                LastUpdatedDate = LastUpdatedDate
            };
        }

        public string id { get; set; }
        public string MiseEntityType { get; set; }
        public Guid EntityID { get; set; }
        public Guid? RestaurantID { get; set; }
        public string JSON { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
    }
}