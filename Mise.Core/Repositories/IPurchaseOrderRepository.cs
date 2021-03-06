﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;

namespace Mise.Core.Repositories
{
    public interface IPurchaseOrderRepository : IEventSourcedEntityRepository<IPurchaseOrder, IPurchaseOrderEvent>
    {
    }
}
