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
    public interface IParBeverageLineItem : IBaseBeverageLineItem, ICloneableEntity, IRestaurantEntityBase, IEquatable<IParBeverageLineItem>
    {
        decimal Quantity { get; }
    }
}
