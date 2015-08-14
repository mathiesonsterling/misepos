using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Inventory
{
    public class PurchaseOrderCreatedEvent : BasePurchaseOrderEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.PurchaseOrderCreated; }
        }

        public override bool IsEntityCreation
        {
            get { return true; }
        }

        public override bool IsAggregateRootCreation
        {
            get { return true; }
        }

        public string EmployeeCreatingName{get;set;}
    }
}
