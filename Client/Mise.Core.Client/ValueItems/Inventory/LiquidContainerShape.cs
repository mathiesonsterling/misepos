using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Client.ValueItems.Inventory
{
    public class LiquidContainerShape 
        : Core.ValueItems.Inventory.LiquidContainerShape, IDbValueItem<Core.ValueItems.Inventory.LiquidContainerShape>
    {
        public LiquidContainerShape() { }

        public LiquidContainerShape(Core.ValueItems.Inventory.LiquidContainerShape source) : base(source) { }

        public Core.ValueItems.Inventory.LiquidContainerShape ToValueItem()
        {
            return this;
        }
    }
}
