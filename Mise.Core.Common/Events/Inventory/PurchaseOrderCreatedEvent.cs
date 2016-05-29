using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Inventory
{
    public class PurchaseOrderCreatedEvent : BasePurchaseOrderEvent
    {
        public override MiseEventTypes EventType => MiseEventTypes.PurchaseOrderCreated;

        public override bool IsEntityCreation => true;

        public override bool IsAggregateRootCreation => true;

        public PersonName EmployeeCreatingName{get;set;}
    }
}
