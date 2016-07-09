using System;
using Mise.Core.Common.Entities.DTOs;
using Newtonsoft.Json;

namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure
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
            id = dto.Id.ToString();
            MiseEntityType = dto.SourceType.ToString();
            EntityID = dto.Id;
            RestaurantID = dto.RestaurantID;
            EntityJSON = dto.JSON;
            LastUpdatedDate = dto.LastUpdatedDate;
        }

        public RestaurantEntityDataTransportObject ToRestaurantDTO()
        {
            return new RestaurantEntityDataTransportObject
            {
                SourceType = Type.GetType(MiseEntityType),
                Id = EntityID,
                RestaurantID = RestaurantID,
                JSON = EntityJSON,
                LastUpdatedDate = LastUpdatedDate
            };
        }

        public string id { get; set; }
        public string MiseEntityType { get; set; }
        public Guid EntityID { get; set; }
        public Guid? RestaurantID { get; set; }
        public string EntityJSON { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }


		[JsonProperty(PropertyName = "__version")]
		public string Version { set; get; }
    }
}