using System;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.DTOs
{
	public class EventDataTransportObject : IEntityEventBase
	{
        public EventDataTransportObject() { }

        /// <summary>
        /// Clone constructor
        /// </summary>
        /// <param name="toClone"></param>
	    public EventDataTransportObject(EventDataTransportObject toClone)
        {
            CausedById = toClone.CausedById;
            CreatedDate = toClone.CreatedDate;
            DeviceId = toClone.DeviceId;
            EntityID = toClone.EntityID;
            EventOrder = toClone.EventOrder;
            EventType = toClone.EventType;
            Id = toClone.Id;
            ItemCacheStatus = toClone.ItemCacheStatus;
            JSON = toClone.JSON;
            LastUpdatedDate = toClone.LastUpdatedDate;
            RestaurantId = toClone.RestaurantId;
            SourceType = toClone.SourceType;
            IsAggregateRootCreation = toClone.IsAggregateRootCreation;
            IsEntityCreation = toClone.IsEntityCreation;
        }
		/// <summary>
		/// Class this event was serailized from
		/// </summary>
		public Type SourceType { get; set; }

        public bool IsAggregateRootCreation { get; set; }
        public bool IsEntityCreation { get; set; }

		/// <summary>
		/// JSON representation of this event
		/// </summary>
		public string JSON { get; set; }

		public Guid EntityID { get; set; }

		public MiseEventTypes EventType { get; set; }

		public Guid Id { get; set; }

		public Guid RestaurantId { get; set; }

		public EventID EventOrder { get; set; }

		public Guid CausedById { get; set; }

		public DateTimeOffset CreatedDate { get; set; }

		public string DeviceId { get; set; }

		public DateTimeOffset LastUpdatedDate { get; set; }

		/// <summary>
		/// Status of the object across the layers
		/// </summary>
		public ItemCacheStatus ItemCacheStatus { get; set; }
	}
}