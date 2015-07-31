using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Entities.Inventory
{
    /// <summary>
    /// Represents a line item of how much of a beverage item we want to have in the bar
    /// </summary>
    public interface IPARBeverageLineItem : IBaseBeverageLineItem, ICloneableEntity, IRestaurantEntityBase, IEquatable<IPARBeverageLineItem>
    {
        decimal Quantity { get; }
    }
}
