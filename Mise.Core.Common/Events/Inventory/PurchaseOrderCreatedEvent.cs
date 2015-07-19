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

		public string EmployeeCreatingName{get;set;}
    }
}
