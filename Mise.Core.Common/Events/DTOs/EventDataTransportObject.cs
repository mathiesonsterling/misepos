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
            CausedByID = toClone.CausedByID;
            CreatedDate = toClone.CreatedDate;
            DeviceID = toClone.DeviceID;
            EntityID = toClone.EntityID;
            EventOrderingID = toClone.EventOrderingID;
            EventType = toClone.EventType;
            ID = toClone.ID;
            ItemCacheStatus = toClone.ItemCacheStatus;
            JSON = toClone.JSON;
            LastUpdatedDate = toClone.LastUpdatedDate;
            RestaurantID = toClone.RestaurantID;
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

		public Guid ID { get; set; }

		public Guid RestaurantID { get; set; }

		public EventID EventOrderingID { get; set; }

		public Guid CausedByID { get; set; }

		public DateTimeOffset CreatedDate { get; set; }

		public string DeviceID { get; set; }

		public DateTimeOffset LastUpdatedDate { get; set; }

		/// <summary>
		/// Status of the object across the layers
		/// </summary>
		public ItemCacheStatus ItemCacheStatus { get; set; }
	}
}