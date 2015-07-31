using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Accounts
{
    public abstract class BaseAccountEvent : IAccountEvent
    {
        public abstract MiseEventTypes EventType { get; }
        public virtual bool IsEntityCreation { get { return false; } }
        public virtual bool IsAggregateRootCreation { get { return false; }}

        public Guid ID { get; set; }

        /// <summary>
        /// Should likely be ignored?
        /// </summary>
        public Guid RestaurantID { get{throw new InvalidOperationException("RestaurantID on AccountEvent");} }

        public EventID EventOrderingID { get; set; }
        public Guid CausedByID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string DeviceID { get; set; }
        public Guid AccountID { get; set; }
    }
}
