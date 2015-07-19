using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Inventory
{
    public class InventoryMadeCurrentEvent : BaseInventoryEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.InventoryMadeCurrent; }
        }
    }
}
