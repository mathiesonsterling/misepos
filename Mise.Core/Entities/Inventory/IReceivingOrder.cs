using System;
using System.Collections.Generic;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Entities.Inventory
{
    /// <summary>
    /// Represents the vendor giving items to the restaurant.  Constructed as we take in a shipment
    /// </summary>
    public interface IReceivingOrder : IRestaurantEntityBase, ICloneableEntity, 
		IEventStoreEntityBase<IReceivingOrderEvent>, ITextSearchable
    {
        /// <summary>
        /// The vendor we're taking stuff from
        /// </summary>
        Guid VendorID { get; }

        /// <summary>
        /// If true, we haven't taken this item fully in yet
        /// </summary>
        ReceivingOrderStatus Status { get; }

        Guid ReceivedByEmployeeID { get;  }

		/// <summary>
		/// External ID from the vendor.  If a Mise on all sides, might be empty 
		/// </summary>
		/// <value>The invoid I.</value>
		string InvoiceID{get;}

        /// <summary>
        /// Notes for anything the employee wants to mention
        /// </summary>
        string Notes { get; }

        /// <summary>
        /// The POs this is fulfilling, if any
        /// </summary>
        Guid? PurchaseOrderID { get; }

        DateTimeOffset? PurchaseOrderDate { get; }

        IEnumerable<IReceivingOrderLineItem> GetBeverageLineItems();

    }
}
