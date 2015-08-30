using System;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Inventory
{
    public class PurchaseOrderSentToVendorEvent : BasePurchaseOrderEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.PurchaseOrderSentToVendor; }
        }

		public Guid VendorID{get;set;}
    }
}
