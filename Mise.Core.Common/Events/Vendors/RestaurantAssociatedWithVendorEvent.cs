using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Vendors
{
    /// <summary>
    /// Shows that a restaurant is now a customer of a vendor
    /// </summary>
    /// <remarks>We assume the restaurantID of who requested it is also the restaurant now associated with the vendor</remarks>
    public class RestaurantAssociatedWithVendorEvent : BaseVendorEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.RestaurantAssociatedWithVendor; }
        }
    }
}
