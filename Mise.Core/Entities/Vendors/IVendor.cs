using System;
using System.Collections.Generic;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Inventory;
namespace Mise.Core.Entities.Vendors
{
    /// <summary>
    /// Represents someone that sells stuff to restaurants - distributor, supplier, etc
    /// </summary>
    public interface IVendor : IEventStoreEntityBase<IVendorEvent>, ITextSearchable
    {
        string Name { get; }

		/// <summary>
		/// Displays details, such as city and state
		/// </summary>
		/// <value>The detail.</value>
		string Detail{get;}

        StreetAddress StreetAddress { get; }
        /// <summary>
        /// The email that people send their PO orders to currently
        /// </summary>
        EmailAddress EmailToOrderFrom { get; }
        PhoneNumber PhoneNumber { get; }
        Guid? CreatedByEmployeeID { get; set; }

        /// <summary>
        /// Whether or not Mise has verified this is a real Vendor
        /// </summary>
        bool Verified { get; }

        /// <summary>
        /// Get all possible beverage items we can order from this vendor
        /// </summary>
        /// <returns></returns>
        IEnumerable<IVendorBeverageLineItem> GetItemsVendorSells();

        IEnumerable<Guid> GetRestaurantIDsAssociatedWithVendor();

		/// <summary>
		/// Lets the vendor state what price they'll supply a restaurnt for
		/// Done here so each vendor can offer volume discounts, specific restaurant discounts, etc
		/// </summary>
		/// <returns>The price for line item, per unit</returns>
		/// <param name="li">Li.</param>
		/// <param name="quantity">Quantity.</param>
		/// <param name="restaurantID"></param>
		Money GetPriceForLineItem (IBaseBeverageLineItem li, decimal quantity, Guid? restaurantID);

		bool DoesCarryItem (IBaseBeverageLineItem li, decimal quantity);

        /// <summary>
        /// Slightly different from Equals, because our IDs could be different
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        bool IsSameVendor(IVendor v);
    }
}
