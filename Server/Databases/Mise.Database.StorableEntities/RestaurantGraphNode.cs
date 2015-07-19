using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Restaurant;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities
{
    public sealed class RestaurantGraphNode : IStorableEntityGraphNode
    {
        public Guid ID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public string Revision { get; set; }

        public string FriendlyID { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }

        //public Guid AccountID;
        public string RestaurantServerLocation { get; set; }

        public bool IsPlaceholder { get; set; }


        public RestaurantGraphNode()
        {
            
        }

        public RestaurantGraphNode(IRestaurant source) 
        {
            FriendlyID = source.FriendlyID;
            FullName = source.Name.FullName;
            ShortName = source.Name.ShortName;
            RestaurantServerLocation = source.RestaurantServerLocation != null ? source.RestaurantServerLocation.ToString():string.Empty;

            ID = source.ID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();
            IsPlaceholder = source.IsPlaceholder;
        }

        public IRestaurant Rehydrate(StreetAddress address, IEnumerable<MiseTerminalDevice> terms, IAccount account, 
            IEnumerable<RestaurantInventorySection> inventorySections, Guid? currentInventoryID, Guid? lastMeasuredInventoryID)
        {
            var rest = new Restaurant
            {
                FriendlyID = FriendlyID,
                Name =new RestaurantName(FullName, ShortName),
                StreetAddress = address,
                Terminals = terms.ToList(),
                AccountID = account != null ? (Guid?)account.ID : null,
                RestaurantServerLocation = string.IsNullOrEmpty(RestaurantServerLocation)?null : new Uri(RestaurantServerLocation),

                ID = ID,
                RestaurantID = ID,
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID(Revision),
                InventorySections = inventorySections.ToList(),
                CurrentInventoryID = currentInventoryID,
                LastMeasuredInventoryID = lastMeasuredInventoryID
            };

            return rest;
        }

    }
}
