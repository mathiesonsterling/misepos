using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Database.AzureDefinitions.Entities.People;

namespace Mise.Database.AzureDefinitions.Entities.Inventory
{
    public class PurchaseOrder : BaseDbEntity<IPurchaseOrder, Core.Common.Entities.Inventory.PurchaseOrder>
    {
        public List<PurchaseOrderPerVendor> PurchaseOrdersPerVendor { get; set; }

        public Employee CreatedBy{ get; set; }

        protected override Core.Common.Entities.Inventory.PurchaseOrder CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.PurchaseOrder
            {
                PurchaseOrdersPerVendor = PurchaseOrdersPerVendor.Select(pv => pv.ToBusinessEntity()).ToList(),
                CreatedByEmployeeID = CreatedBy?.EntityId,
                CreatedByName = CreatedBy?.Name
            };
        }
    }
}
