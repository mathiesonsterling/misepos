using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory.Events;

namespace Mise.Core.Entities.Inventory
{
    public interface IPar : IRestaurantEntityBase, IEventStoreEntityBase<IPAREvent>, ICloneableEntity
    {
        Guid CreatedByEmployeeID { get; }

        /// <summary>
        /// If this is the current par for the restuarant - for now, only one per, so always true!
        /// </summary>
        bool IsCurrent { get; }

        IEnumerable<IPARBeverageLineItem> GetBeverageLineItems();
    }
}
