using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory
{
    public class ReceivingOrder : BaseDbEntity<IReceivingOrder, Core.Common.Entities.Inventory.ReceivingOrder>
    {
        protected override Core.Common.Entities.Inventory.ReceivingOrder CreateConcreteSubclass()
        {
            throw new NotImplementedException();
        }
    }
}
