using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems.Inventory
{
    /// <summary>
    /// The different states a purchase order can be in
    /// </summary>
    public enum PurchaseOrderStatus
    {
        Created,
		/// <summary>
		/// Approved by an employee and sent
		/// </summary>
		Approved,
        /// <summary>
        /// Cancelled on device - when user creates, doesn't send, then creates another
        /// </summary>
        Cancelled,
        SentToVendor,
        ReceivedWithAlterations,
        ReceivedTotally,
        RejectedByRestaurant,
        RejectedByVendor
    }
}
