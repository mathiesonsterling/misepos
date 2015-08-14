using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service;
using Mise.Core.Common.Entities.DTOs;

namespace stockboymobileserviceService.DataObjects
{
    /// <summary>
    /// Weak storage on entities - holds their type and JSON, not much else
    /// </summary>
    public class AzureEntityStorage : EntityData
    {
        public AzureEntityStorage() { }

        public AzureEntityStorage(RestaurantEntityDataTransportObject dto)
        {
            BuildLevel = "SeedData";
            MiseEntityType = dto.SourceType.ToString();
            EntityID = dto.ID;
            RestaurantID = dto.RestaurantID;
            JSON = dto.JSON;
            LastUpdatedDate = dto.LastUpdatedDate;
        }

        public string BuildLevel { get; set; }

        public string MiseEntityType { get; set; }

        public Guid EntityID { get; set; }

        public Guid? RestaurantID { get; set; }

        public string JSON { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }
    }
}
