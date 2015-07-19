using System;
using Mise.Core.Entities.Payments;

namespace Mise.Database.StorableEntities
{
    /// <summary>
    /// adapter/facade to get around the money serialization issues
    /// </summary>
    sealed class DiscountPercentageAfterMinimumCashTotalGraphNode 
    {
        public DiscountPercentageAfterMinimumCashTotalGraphNode(DiscountPercentageAfterMinimumCashTotal source) 
        {
            ID = source.ID;
            RestaurantID = source.RestaurantID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.OrderingID;

            MinCheckAmount = source.MinCheckAmount.Dollars;
            DiscountType = source.DiscountType;
            Name = source.Name;
            Percentage = source.Percentage;
        }


        public DiscountPercentageAfterMinimumCashTotalGraphNode()
        {
            
        }

        public Guid ID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public long Revision { get; set; }

        public Guid? RestaurantID { get; set; }

        public decimal MinCheckAmount { get; set; }
        public DiscountType DiscountType { get; set; }
        public string Name { get; set; }

        public decimal Percentage { get; set; }
    }
}
