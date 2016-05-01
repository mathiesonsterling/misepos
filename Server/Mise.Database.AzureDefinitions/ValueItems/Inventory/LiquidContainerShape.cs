using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Database.AzureDefinitions.ValueItems.Inventory
{
    [ComplexType]
    public class LiquidContainerShape 
        : Core.ValueItems.Inventory.LiquidContainerShape, IDbValueItem<Core.ValueItems.Inventory.LiquidContainerShape>
    {
        public Core.ValueItems.Inventory.LiquidContainerShape ToValueItem()
        {
            return this;
        }
    }
}
