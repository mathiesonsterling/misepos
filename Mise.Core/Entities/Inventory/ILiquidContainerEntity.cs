using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Entities.Inventory
{
    public interface ILiquidContainerEntity : IEntityBase, ITextSearchable
    {
        /// <summary>
        /// If set, this container only shows up for this restaurant
        /// </summary>
        Guid? BusinessId { get;  }
        string DisplayName { get; }
        LiquidAmount AmountContained { get; }
        Weight WeightEmpty { get; }
        Weight WeightFull { get; }
        LiquidContainerShape Shape { get; }
    }
}
