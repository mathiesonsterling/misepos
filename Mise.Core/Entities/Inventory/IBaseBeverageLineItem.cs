using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Inventory
{
    /// <summary>
    /// Represents a beverage item, whether from a vendor, in the inventory, desired in the PAR, etc
    /// </summary>
    public interface IBaseBeverageLineItem : IBaseLineItem
    {
        /// <summary>
        /// Size of the container it comes in, which also has other information in it
        /// </summary>
        LiquidContainer Container { get; }
	}
}
