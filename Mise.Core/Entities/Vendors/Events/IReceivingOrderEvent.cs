using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;

namespace Mise.Core.Entities.Vendors.Events
{
    public interface IReceivingOrderEvent : IEntityEventBase
    {
        Guid ReceivingOrderID { get; }
    }
}
