using System;
using Mise.Core.Entities;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.ApplicationInvitations
{
	public abstract class BaseInvitationEvent : IApplicationInvitationEvent
	{
		public Guid InvitationID{get;set;}

        public abstract MiseEventTypes EventType { get; }

        public virtual bool IsEntityCreation { get { return false; } }
        public virtual bool IsAggregateRootCreation { get { return false; } }

        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public EventID EventOrder { get; set; }
        public Guid CausedById { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string DeviceId { get; set; }
	}
}

