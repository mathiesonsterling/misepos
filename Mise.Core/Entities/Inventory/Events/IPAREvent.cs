using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;

namespace Mise.Core.Entities.Inventory.Events
{
    public interface IPAREvent : IEntityEventBase
    {
        Guid ParID { get; }
    }
}
