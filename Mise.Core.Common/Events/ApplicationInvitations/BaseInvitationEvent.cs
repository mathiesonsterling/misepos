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

        public Guid ID { get; set; }
        public Guid RestaurantID { get; set; }
        public EventID EventOrderingID { get; set; }
        public Guid CausedByID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string DeviceID { get; set; }
	}
}

