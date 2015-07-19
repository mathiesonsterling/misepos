using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Entities.Vendors
{
    /// <summary>
    /// Represents some amount of a beverage that a vendor sells (usually liquor, beer, or wine)
    /// </summary>
    public interface IVendorBeverageLineItem : IBaseBeverageLineItem, ICloneableEntity
    {
        /// <summary>
        /// ID of the vendor that carries this
        /// </summary>
        Guid VendorID { get;}

        /// <summary>
        /// Name of the item as the Vendor calls it
        /// </summary>
        string NameInVendor { get; }

        /// <summary>
        /// How much the item is being sold for - might not include tax, but it's the list price
        /// </summary>
        Money PublicPricePerUnit { get; }

        /// <summary>
        /// Set by the server, the last price given to this restaurant
        /// </summary>
        /// <value>The last price paid by restaurant per unit.</value>
        Money GetLastPricePaidByRestaurantPerUnit(Guid restaurantID);

        Dictionary<Guid, Money> GetPricesForRestaurants();

		DateTimeOffset? LastTimePriceSet{get;}
    }
}
