using System;
using Microsoft.WindowsAzure.Mobile.Service;
using Mise.Core.Common.Entities.DTOs;

namespace stockboymobileserviceService.DataObjects
{
    public class AzureEntityStorage : EntityData
    {
        public string MiseEntityType { get; set; }

        public Guid EntityID { get; set; }

        public Guid? RestaurantID { get; set; }

        public string EntityJSON { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public AzureEntityStorage()
        {
            
        }

        public AzureEntityStorage(RestaurantEntityDataTransportObject dto)
        {
            Id = dto.Id.ToString();
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
    }
}
