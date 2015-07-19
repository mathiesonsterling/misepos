using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Inventory
{
    /// <summary>
    /// Represents a division of the restaurant for inventory purposes.  Example would be "Bar Rail", "Stock Room", etc
    /// </summary>
    public interface IRestaurantInventorySection : IRestaurantEntityBase, ICloneableEntity
    {
        string Name { get; }

        bool AllowsPartialBottles { get; }

        /// <summary>
        /// If set, a Beacon set to broadcast this section's location
        /// </summary>
        Beacon Beacon { get; }
    }
}
