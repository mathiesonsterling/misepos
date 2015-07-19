using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Employee
{
    public class EmployeeLoggedOutOfInventoryAppEvent : BaseEmployeeEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.EmployeeLoggedOutOfInventoryApp; }
        }
    }
}
