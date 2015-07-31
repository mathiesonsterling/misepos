using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Inventory
{
    public class PARCreatedEvent : BasePAREvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.PARCreated; }
        }

        public override bool IsEntityCreation
        {
            get { return true; }
        }

        public override bool IsAggregateRootCreation
        {
            get { return true; }
        }
    }
}
