using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Accounts
{
    public class RestaurantAccountRegisteredOnWebsiteEvent : AccountRegisteredFromMobileDeviceEvent
    {
        public override MiseEventTypes EventType => MiseEventTypes.RestaurantAccountRegisteredOnWebsite;
    }
}
