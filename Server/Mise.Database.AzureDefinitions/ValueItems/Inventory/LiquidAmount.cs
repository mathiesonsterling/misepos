using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Database.AzureDefinitions.ValueItems.Inventory
{
    [ComplexType]
    public class LiquidAmount :Core.ValueItems.Inventory.LiquidAmount, IDbValueItem<Core.ValueItems.Inventory.LiquidAmount>
    {
        public LiquidAmount()
        {
        }

        public LiquidAmount(Core.ValueItems.Inventory.LiquidAmount source) : base(source)
        {
            
        }

        public Core.ValueItems.Inventory.LiquidAmount ToValueItem()
        {
            return this;
        }
    }
}
