using Mise.Core.Common.Entities.DTOs;

namespace MiseVendorManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("stagingstockboymobileservice.AzureEntityStorages")]
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
                ID = EntityID,
                RestaurantID = RestaurantID,
                JSON = EntityJSON,
                LastUpdatedDate = LastUpdatedDate
            };
        }
    }
}
