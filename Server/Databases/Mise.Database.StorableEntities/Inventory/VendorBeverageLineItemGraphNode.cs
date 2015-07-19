using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Database.StorableEntities.Inventory
{
    public class VendorBeverageLineItemGraphNode : IStorableEntityGraphNode
    {
        public VendorBeverageLineItemGraphNode() { }

        public VendorBeverageLineItemGraphNode(IVendorBeverageLineItem source)
        {
            CreatedDate = source.CreatedDate;
            ID = source.ID;
            LastUpdatedDate = source.LastUpdatedDate;
            MiseName = source.MiseName;
            NameInVendor = source.NameInVendor;
            PublicPricePerUnit = source.PublicPricePerUnit != null? source.PublicPricePerUnit.Dollars:(decimal?)null;
            Revision = source.Revision.ToDatabaseString();
            UPC = source.UPC;
            VendorID = source.VendorID;
            DisplayName = source.DisplayName;
            CaseSize = source.CaseSize;
        }

        public VendorBeverageLineItem Rehydrate(LiquidContainer container, IEnumerable<ItemCategory> categories, IDictionary<Guid, decimal> pricesPerRestaurant)
        {
            //TODO - get the prices per restaurant here as well!
            var item = new VendorBeverageLineItem
            {
                Container = container,
                CreatedDate = CreatedDate,
                ID = ID,
                LastUpdatedDate = LastUpdatedDate,
                MiseName = MiseName,
                NameInVendor = NameInVendor,
                PublicPricePerUnit = PublicPricePerUnit.HasValue? new Money(PublicPricePerUnit.Value) : null,
                Revision = new EventID(Revision),
                UPC = UPC,
                VendorID = VendorID,
                DisplayName = DisplayName,
                CaseSize = CaseSize,
                Categories = categories.ToList()
            };

            foreach (var kv in pricesPerRestaurant)
            {
                item.PricePerUnitForRestaurant.Add(kv.Key, new Money(kv.Value));
            }
            return item;
        }

        public int? CaseSize { get; set; }

        public string DisplayName { get; set; }

        public Guid VendorID { get; set; }

        public string UPC { get; set; }

        public decimal? PublicPricePerUnit { get; set; }

        public string NameInVendor { get; set; }

        public string MiseName { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid ID { get; set; }
        public string Revision { get; set; }
    }
}
