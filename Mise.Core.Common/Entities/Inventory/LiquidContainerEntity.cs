using System;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
    public class LiquidContainerEntity : EntityBase, ILiquidContainerEntity
    {
        public bool ContainsSearchString(string searchString)
        {
            return (AmountContained != null && AmountContained.Milliliters.ToString().Contains(searchString))
|| (DisplayName != null && DisplayName.ToUpper().Contains(searchString.ToUpper()))
|| (Shape != null && Shape.ContainsSearchString(searchString));
        }

        public Guid? BusinessId { get; set; }
        public string DisplayName { get; set; }
        public LiquidAmount AmountContained { get; set; }
        public Weight WeightEmpty { get; set; }
        public Weight WeightFull { get; set; }
        public LiquidContainerShape Shape { get; set; }


    }
}
