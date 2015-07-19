using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities.Inventory
{
    public class VendorGraphNode : IStorableEntityGraphNode
    {
        public VendorGraphNode() { }

        public VendorGraphNode(IVendor source)
        {
            CreatedByEmployeeID = source.CreatedByEmployeeID;
            CreatedDate = source.CreatedDate;
            ID = source.ID;
            LastUpdatedDate = source.LastUpdatedDate;
            Name = source.Name;
            Verified = source.Verified;
            if (source.PhoneNumber != null)
            {
                AreaCode = source.PhoneNumber.AreaCode;
                PhoneNumber = source.PhoneNumber.Number;
            }
            Revision = source.Revision.ToDatabaseString();
            LastUpdatedDate = source.LastUpdatedDate;
        }

        public IVendor Rehydrate(IEnumerable<Guid> associatedRestaurants, StreetAddress address, IEnumerable<VendorBeverageLineItem> lineItems, EmailAddress email)
        {
            return new Vendor
            {
                CreatedByEmployeeID = CreatedByEmployeeID,
                CreatedDate = CreatedDate,
                ID = ID,
                LastUpdatedDate = LastUpdatedDate,
                Name = Name,
                PhoneNumber = new PhoneNumber {AreaCode = AreaCode, Number = PhoneNumber},
                RestaurantsAssociatedIDs = associatedRestaurants.ToList(),
                Revision = new EventID(Revision),
                Verified = Verified,
                StreetAddress = address,
                VendorBeverageLineItems = lineItems.ToList(),
                EmailToOrderFrom = email
            };
        }

        public bool Verified { get; set; }

        public string Revision { get; set; }

        public string AreaCode { get; set; }

        public Guid? CreatedByEmployeeID { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid ID { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }
    }
}
