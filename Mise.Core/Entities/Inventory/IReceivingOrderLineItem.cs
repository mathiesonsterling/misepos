using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Inventory
{
    public interface IReceivingOrderLineItem: IRestaurantEntityBase, IBaseBeverageLineItem, ICloneableEntity
    {

		/// <summary>
		/// How much the entire line item costs
		/// </summary>
		/// <value>The line item price.</value>
		Money LineItemPrice{get;}

        /// <summary>
        /// How much we paid, per bottle
        /// </summary>
        Money UnitPrice { get;}

		bool ZeroedOut{get;}
    }
}
