using System;
using Microsoft.WindowsAzure.Mobile.Service;

namespace stockboyService.DataObjects
{
    public class AzureEntityStorage : EntityData
    {
        public string MiseEntityType { get; set; }

        public Guid EntityID { get; set; }

        public Guid? RestaurantID { get; set; }

        public string EntityJSON { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }
    }
}
