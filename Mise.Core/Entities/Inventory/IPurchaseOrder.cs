using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Vendors;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Entities.Inventory
{
    /// <summary>
    /// Represents a purchase order made from a restaurant.  Can contain many vendors, will be split on server
    /// </summary>
    public interface IPurchaseOrder : IRestaurantEntityBase, 
		IEventStoreEntityBase<IPurchaseOrderEvent>, ICloneableEntity, ITextSearchable
    {
        /// <summary>
        /// Employee that created this order
        /// </summary>
        Guid CreatedByEmployeeID { get; }

        string CreatedByName { get; set; }

        IEnumerable<IPurchaseOrderLineItem> GetPurchaseOrderLineItems();

		IEnumerable<IPurchaseOrderPerVendor> GetPurchaseOrderPerVendors();

    }
}
