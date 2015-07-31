using System;
using System.Collections.Generic;
using Mise.Core.Entities;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Common.Entities.Inventory;
namespace Mise.Core.Common.Events.Inventory
{
    public class InventoryLineItemAddedEvent : BaseInventoryEvent, ICreateLineItemEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.InventoryLineItemAdded; }
        }

        public override bool IsEntityCreation
        {
            get { return true; }
        }

        public Guid LineItemID { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// Reconciled name in our system, so we can combat vendor's differning
        /// </summary>
        public string MiseName { get; set; }

        /// <summary>
        /// UPC for this item, if we know it
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// Size of the container it comes in, which also has other information in it
        /// </summary>
        public LiquidContainer Container { get; set; }

        /// <summary>
        /// If not null, this is how many units are in a case.  Allows us to convert cases to bottles
        /// </summary>
        /// <value>The size of the case.</value>
        public int? CaseSize { get; set; }

        public int Quantity { get; set; }

        public Money PricePaid { get; set; }

        public Guid? RestaurantInventorySectionID { get; set; }

        public Guid InventorySectionID { get; set; }

        public Guid? VendorBoughtFrom { get; set; }

		public IEnumerable<ItemCategory> Categories{get;set;}

        public int InventoryPosition { get; set; }
    }
}
