using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;

namespace Mise.Core.Entities.Inventory
{
    public interface IPurchaseOrderLineItem : IBaseBeverageLineItem, ICloneableEntity, IRestaurantEntityBase
    {
		/// <summary>
		/// The vendor we intend to buy from, if any
		/// </summary>
		/// <value>The vendor I.</value>
		Guid? VendorID{ get; }
    }
}
