using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;

namespace Mise.Core.Common.Events.Restaurant
{
    public class RestaurantAssociatedWithAccountEvent : BaseRestaurantEvent, IAccountEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.RestaurantAssociatedWithAccount; }
        }

        public Guid AccountID { get; set; }
    }
}
