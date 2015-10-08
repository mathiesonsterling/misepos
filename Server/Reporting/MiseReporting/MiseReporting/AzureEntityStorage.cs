using Mise.Core.Common.Entities.DTOs;

namespace MiseReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("stockboymobileservice.AzureEntityStorages")]
    public partial class AzureEntityStorage
    {
        public string Id { get; set; }

        public string BuildLevel { get; set; }

        public string MiseEntityType { get; set; }

        public Guid EntityID { get; set; }

        public Guid? RestaurantID { get; set; }

        public string EntityJSON { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] Version { get; set; }

    
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public bool Deleted { get; set; }

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

        public AzureEntityStorage() { }

        public AzureEntityStorage(RestaurantEntityDataTransportObject dto)
        {
            Id = dto.Id.ToString();
            MiseEntityType = dto.SourceType.ToString();
            EntityID = dto.Id;
            RestaurantID = dto.RestaurantID;
            EntityJSON = dto.JSON;
            LastUpdatedDate = dto.LastUpdatedDate;
        }
    }
}
