using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public class ReceivingOrderBeverageLineItem : BaseDbEntity<IReceivingOrderLineItem, Core.Common.Entities.Inventory.ReceivingOrderLineItem>
    {
        protected override ReceivingOrderLineItem CreateConcreteSubclass()
        {
            throw new NotImplementedException();
        }
    }
}
