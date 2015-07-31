using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Restaurant
{
    public abstract class BaseRestaurantEvent : IRestaurantEvent
    {
        public abstract MiseEventTypes EventType { get; }

        public Guid ID { get; set; }
        public virtual bool IsEntityCreation { get { return false; } }
        public virtual bool IsAggregateRootCreation { get { return false; } }

        public Guid RestaurantID { get; set; }
        public EventID EventOrderingID { get; set; }
        public Guid CausedByID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string DeviceID { get; set; }
    }
}
